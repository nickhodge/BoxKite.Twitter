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

        public IUserSession Session { get; set; }
        public IUserStream UserStream { get; set; }
        public ISearchStream SearchStream { get; set; }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, IEventAggregator eventAggregator, IPlatformAdaptor platformAdaptor)
        {
            TwitterConnectionEvents = eventAggregator;
            PlatformAdaptor = platformAdaptor;
            Session = SessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        }

        public TwitterConnection(TwitterCredentials twitterCredentials, IEventAggregator eventAggregator, IPlatformAdaptor platformAdaptor)
        {
            TwitterCredentials = twitterCredentials;
            PlatformAdaptor = platformAdaptor;
            TwitterConnectionEvents = eventAggregator;
            Session = SessionBuilder();
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername, string xauthpassword, IEventAggregator eventAggregator, IPlatformAdaptor platformAdaptor)
        {
            TwitterConnectionEvents = eventAggregator;
            PlatformAdaptor = platformAdaptor;
            Session = SessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        }

#if (WINDOWSDESKTOP)
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, IEventAggregator eventAggregator)
        {
            TwitterConnectionEvents = eventAggregator;
            PlatformAdaptor = new DesktopPlatformAdaptor();
            Session = SessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        }

        public TwitterConnection(TwitterCredentials twitterCredentials, IEventAggregator eventAggregator)
        {
            TwitterCredentials = twitterCredentials;
            PlatformAdaptor = new DesktopPlatformAdaptor();
            TwitterConnectionEvents = eventAggregator;
            Session = SessionBuilder();
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername, string xauthpassword, IEventAggregator eventAggregator)
        {
            TwitterConnectionEvents = eventAggregator;
            PlatformAdaptor = new DesktopPlatformAdaptor();
            Session = SessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        }
#endif
#if(WIN8RT)
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, IEventAggregator eventAggregator)
        {
            TwitterConnectionEvents = eventAggregator;
            PlatformAdaptor = new Win8RTPlatformAdaptor();
            Session = SessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        }

        public TwitterConnection(TwitterCredentials twitterCredentials, IEventAggregator eventAggregator)
        {
            TwitterCredentials = twitterCredentials;
            PlatformAdaptor = new Win8RTPlatformAdaptor();
            TwitterConnectionEvents = eventAggregator;
            Session = SessionBuilder();
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername, string xauthpassword, IEventAggregator eventAggregator)
        {
            TwitterConnectionEvents = eventAggregator;
            PlatformAdaptor = new Win8RTPlatformAdaptor();
            Session = SessionBuilder(twitterConsumerKey, twitterConsumerSecret);
        } 
#endif

        private IUserSession SessionBuilder()
        {
            return Session ?? new UserSession(TwitterCredentials, PlatformAdaptor);
        }

        private IUserSession SessionBuilder(string twitterConsumerKey, string twitterConsumerSecret)
        {
            return Session ?? new UserSession(twitterConsumerKey, twitterConsumerSecret, PlatformAdaptor);
        }

        // auth happens when no creds are present
        public async Task<string> BeginAuthentication()
        {
            var oAuthToken = await Session.StartAuthentication();
            return oAuthToken;
        }

        // second stage of auth confirms the pin is OK
        public async Task<TwitterCredentials> CompleteAuthentication(string pin, string oAuthToken)
        {
            var twittercredentials = await Session.ConfirmPin(pin, oAuthToken);
            return !twittercredentials.Valid ? null : twittercredentials;
        }

        // another method using xauth as the authentication flow
        public async Task<TwitterCredentials> XAuthentication(string xauthusername, string xauthpassword)
        {
            var twittercredentials = await Session.XAuthentication(xauthusername, xauthpassword);
            if (!twittercredentials.Valid) return null;
            return twittercredentials;
        }

        public async Task<bool> VerifyCredentials()
        {
            AccountDetails = await Session.GetVerifyCredentials();
            if (AccountDetails.OK) // go deeper
            {
                AccountSettings = await Session.GetAccountSettings();
                AccountDetails = await Session.GetUserProfile(user_id: AccountDetails.UserId);
                return AccountDetails.OK;
            }
            else
                return false; // return false here will abandon all hope
        }

#if(WIN8RT)
        public async Task<TwitterCredentials> Authenticate()
        {
            var twittercredentials =  await Session.Authentication();
            if (!twittercredentials.Valid) return null;
            return twittercredentials;
        }
#endif

    }
}
