// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BoxKite.Twitter.Authentication;
using BoxKite.Twitter.Models;
using Reactive.EventAggregator;

namespace BoxKite.Twitter
{
    public partial class TwitterConnection
    {
        public IEventAggregator TwitterConnectionEvents { get; set; }
        public IPlatformAdaptor PlatformAdaptor { get; set; }

        public User AccountDetails { get; set; }
        public TwitterCredentials TwitterCredentials { get; set; }
        public AccountSettings AccountSettings { get; set; }

        public IApplicationSession ApplicationSession { get; set; }
        public IUserSession UserSession { get; set; }
        public IUserStream UserStream { get; set; }
        public ISearchStream SearchStream { get; set; }

#if(PORTABLE)
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, IEventAggregator eventAggregator = null, IPlatformAdaptor platformAdaptor = null)
        {
            TwitterConnectionEvents = eventAggregator ?? new EventAggregator();
            PlatformAdaptor = platformAdaptor;
            ApplicationSession = ApplicationSessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        }

        public TwitterConnection(TwitterCredentials twitterCredentials, IEventAggregator eventAggregator = null, IPlatformAdaptor platformAdaptor = null)
        {
            TwitterCredentials = twitterCredentials;
            PlatformAdaptor = platformAdaptor;
            TwitterConnectionEvents = eventAggregator ?? new EventAggregator();
            UserSession = SessionBuilder();
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername, string xauthpassword, IEventAggregator eventAggregator = null, IPlatformAdaptor platformAdaptor = null)
        {
            TwitterConnectionEvents = eventAggregator ?? new EventAggregator();
            PlatformAdaptor = platformAdaptor;
            UserSession = SessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        }
#endif
#if (WINDOWSDESKTOP)
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, IEventAggregator eventAggregator = null)
        {
            TwitterConnectionEvents = eventAggregator ?? new EventAggregator();
            PlatformAdaptor = new DesktopPlatformAdaptor();
            Session = SessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        }

        public TwitterConnection(TwitterCredentials twitterCredentials, IEventAggregator eventAggregator = null)
        {
            TwitterCredentials = twitterCredentials;
            PlatformAdaptor = new DesktopPlatformAdaptor();
            TwitterConnectionEvents = eventAggregator ?? new EventAggregator();
            Session = SessionBuilder();
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername, string xauthpassword, IEventAggregator eventAggregator = null)
        {
            TwitterConnectionEvents = eventAggregator ?? new EventAggregator();
            PlatformAdaptor = new DesktopPlatformAdaptor();
            Session = SessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        }
#endif
#if(WIN8RT)
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, IEventAggregator eventAggregator = null)
        {
            TwitterConnectionEvents = eventAggregator ?? new EventAggregator();
            PlatformAdaptor = new Win8RTPlatformAdaptor();
            Session = SessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        }

        public TwitterConnection(TwitterCredentials twitterCredentials, IEventAggregator eventAggregator = null)
        {
            TwitterCredentials = twitterCredentials;
            PlatformAdaptor = new Win8RTPlatformAdaptor();
            TwitterConnectionEvents = eventAggregator ?? new EventAggregator();
            Session = SessionBuilder();
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername, string xauthpassword, IEventAggregator eventAggregator = null)
        {
            TwitterConnectionEvents = eventAggregator ?? new EventAggregator();
            PlatformAdaptor = new Win8RTPlatformAdaptor();
            Session = SessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        } 
#endif

        private IApplicationSession ApplicationSessionBuilder(string twitterConsumerKey, string twitterConsumerSecret)
        {
            return ApplicationSession ?? new ApplicationSession(twitterConsumerKey, twitterConsumerSecret);
        }

        private IUserSession SessionBuilder()
        {
            // todo insert oauth2 from ApplicationSession
            return UserSession ?? new UserSession(TwitterCredentials, PlatformAdaptor);
        }

        private IUserSession SessionBuilder(string twitterConsumerKey, string twitterConsumerSecret)
        {
            return UserSession ?? new UserSession(twitterConsumerKey, twitterConsumerSecret, PlatformAdaptor);
        }

        // auth happens when no creds are present
        public async Task<string> BeginUserAuthentication()
        {
            var oAuthToken = await UserSession.StartUserAuthentication();
            return oAuthToken;
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

        public async Task<bool> VerifyCredentials()
        {
            AccountDetails = await UserSession.GetVerifyCredentials();
            if (AccountDetails.OK) // go deeper
            {
                AccountSettings = await UserSession.GetAccountSettings();
                AccountDetails = await UserSession.GetUserProfile(user_id: AccountDetails.UserId);
                return AccountDetails.OK;
            }
            else
                return false; // return false here will abandon all hope
        }

#if(WIN8RT)
        public async Task<TwitterCredentials> Authenticate(string _callbackuri)
        {
            var twittercredentials = await Session.Authentication(_callbackuri);
            if (!twittercredentials.Valid) return null;
            return twittercredentials;
        }
#endif

    }
}
