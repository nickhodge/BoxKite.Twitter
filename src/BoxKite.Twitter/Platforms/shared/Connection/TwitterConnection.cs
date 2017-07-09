// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BoxKite.Twitter.Authentication;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public partial class TwitterConnection : ITwitterConnection
    {
        /// <summary>
        /// The PlatformAdaptor provides an interface to operating-system specific functions including encryption
        /// and web browser launching
        /// </summary>
        public IPlatformAdaptor PlatformAdaptor { get; set; }

        /// <summary>
        /// Details of Current Logged in User
        /// </summary>
        public User AccountDetails { get; set; }

        /// <summary>
        /// Full Credentials for the current session
        /// </summary>
        public TwitterCredentials TwitterCredentials { get; set; }
        public AccountSettings AccountSettings { get; set; }

        public ITwitterSession TwitterSession
        {
            get
            {
                // return the highest level if the dev asks for a generic TwitterSession
                return UserSession.IsActive ? UserSession : ApplicationSession;
            }
        }
        public IApplicationSession ApplicationSession { get; set; }
        public IUserSession UserSession { get; set; }
        public IUserStream UserStream { get; set; }
        public ISearchStream SearchStream { get; set; }

        private IApplicationSession BuildApplicationSession(string twitterConsumerKey, string twitterConsumerSecret)
        {
            return ApplicationSession ?? new ApplicationSession(twitterConsumerKey, twitterConsumerSecret);
        }

        private IUserSession BuildUserSession()
        {
            return UserSession ?? new UserSession(TwitterCredentials, PlatformAdaptor);
        }

        private IUserSession BuildUserSession(string twitterConsumerKey, string twitterConsumerSecret)
        {
            return UserSession ?? new UserSession(twitterConsumerKey, twitterConsumerSecret, PlatformAdaptor);
        }

        /// <summary>
        /// First stage of PIN-based Authentication (and Authorisation)
        /// This will display a web browser, asking the User to log in to Twitter with their account (Authentication)
        /// and Authorise this client application to access their Twitter Stream
        /// </summary>
        /// <returns>OAuth to be used in CompleteUserAuthentication</returns>
        public async Task<string> BeginUserAuthentication()
        {
            return await UserSession.StartUserAuthentication();
        }

        /// <summary>
        /// Second stage of PIN-based Authentication (and Authorisation) confirms the supplied PIN is correct
        /// for this user, application, session
        /// </summary>
        /// <param name="pin">PIN as entered by the user</param>
        /// <param name="oAuthToken">OAuth token supplied by BeginUserAuthentication</param>
        /// <returns>TwitterCredentials</returns>
        public async Task<TwitterCredentials> CompleteUserAuthentication(string pin, string oAuthToken)
        {
            var twittercredentials = await UserSession.ConfirmPin(pin, oAuthToken);
            return !twittercredentials.Valid ? null : twittercredentials;
        }

        /// <summary>
        /// XAuthentication is a method of creating a set of Credentials for Twitter based on Username and Password
        /// Whilst this is in BoxKite.Twitter, it has not been fully tested
        /// </summary>
        /// <param name="xauthusername">user supplied Username</param>
        /// <param name="xauthpassword">user supplied Password</param>
        /// <returns></returns>
        public async Task<TwitterCredentials> XAuthentication(string xauthusername, string xauthpassword)
        {
            var twittercredentials = await UserSession.XAuthentication(xauthusername, xauthpassword);
            return !twittercredentials.Valid ? null : twittercredentials;
        }

        /// <summary>
        /// Given the current session, this will Verify that the current User's credentials are OK
        /// And update this TwitterConnection's AccountSettings and AccountDetails
        /// </summary>
        /// <returns></returns>
        public async Task<bool> VerifyUserCredentials()
        {
            AccountDetails = await UserSession.GetVerifyCredentials();
            if (AccountDetails.OK) // go deeper
            {
                UserSession.IsActive = true;
                AccountSettings = await UserSession.GetAccountSettings();
                AccountDetails = await UserSession.GetUserProfile(userId: AccountDetails.UserId);
                return AccountDetails.OK;
            }
            else
                return false; // return false here will abandon all hope
        }

#if(WINDOWS_UWP)
        public async Task<TwitterCredentials> AuthenticateUser(string _callbackuri)
        {
            var twittercredentials = await UserSession.Authentication(_callbackuri);
            if (!twittercredentials.Valid) return null;
            UserSession.IsActive = true;
            return twittercredentials;
        }
#endif

    }
}
