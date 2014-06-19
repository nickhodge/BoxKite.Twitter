// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BoxKite.Twitter.Authentication;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public partial class TwitterConnection
    {
        public IPlatformAdaptor PlatformAdaptor { get; set; }

        public User AccountDetails { get; set; }
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

#if(PORTABLE)
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, IPlatformAdaptor platformAdaptor = null)
        {
            PlatformAdaptor = platformAdaptor;
            ApplicationSession = BuildApplicationSession(twitterConsumerKey, twitterConsumerSecret);
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
        }

        public TwitterConnection(TwitterCredentials twitterCredentials, IPlatformAdaptor platformAdaptor = null)
        {
            PlatformAdaptor = platformAdaptor;
            TwitterCredentials = twitterCredentials;
            UserSession = BuildUserSession();
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername, string xauthpassword, IPlatformAdaptor platformAdaptor = null)
        {
            PlatformAdaptor = platformAdaptor;
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
        }
#endif
#if (WINDOWSDESKTOP)
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret)
        {
            PlatformAdaptor = new DesktopPlatformAdaptor();
            ApplicationSession = BuildApplicationSession(twitterConsumerKey, twitterConsumerSecret);
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
         }

       public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, IPlatformAdaptor platformAdaptor = null)
        {
            PlatformAdaptor = platformAdaptor;
            ApplicationSession = BuildApplicationSession(twitterConsumerKey, twitterConsumerSecret);
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
        }

        public TwitterConnection(TwitterCredentials twitterCredentials)
        {
            PlatformAdaptor = new DesktopPlatformAdaptor();
            TwitterCredentials = twitterCredentials;
            UserSession = BuildUserSession();
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername, string xauthpassword)
        {
            PlatformAdaptor = new DesktopPlatformAdaptor();
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
        }
#endif
#if(WIN8RT)
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret)
        {
            PlatformAdaptor = new Win8RTPlatformAdaptor();
            ApplicationSession = BuildApplicationSession(twitterConsumerKey, twitterConsumerSecret);
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
         }

       public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, IPlatformAdaptor platformAdaptor = null)
        {
            PlatformAdaptor = platformAdaptor;
            ApplicationSession = BuildApplicationSession(twitterConsumerKey, twitterConsumerSecret);
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
        }

        public TwitterConnection(TwitterCredentials twitterCredentials)
        {
            PlatformAdaptor = new Win8RTPlatformAdaptor();
            TwitterCredentials = twitterCredentials;
            UserSession = BuildUserSession();
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername, string xauthpassword)
        {
            PlatformAdaptor = new Win8RTPlatformAdaptor();
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
         } 
#endif

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

        // auth happens when no creds are present
        public async Task<string> BeginUserAuthentication()
        {
            return await UserSession.StartUserAuthentication();
        }

        // second stage of auth confirms the pin is OK
        public async Task<TwitterCredentials> CompleteUserAuthentication(string pin, string oAuthToken)
        {
            var twittercredentials = await UserSession.ConfirmPin(pin, oAuthToken);
            return !twittercredentials.Valid ? null : twittercredentials;
        }

        // another method using xauth as the authentication flow
        public async Task<TwitterCredentials> XAuthentication(string xauthusername, string xauthpassword)
        {
            var twittercredentials = await UserSession.XAuthentication(xauthusername, xauthpassword);
            if (!twittercredentials.Valid) return null;
            return twittercredentials;
        }

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

#if(WIN8RT)
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
