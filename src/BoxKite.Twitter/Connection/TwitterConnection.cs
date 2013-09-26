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
        private IEventAggregator _eventAggregator;
        public IEventAggregator TwitterConnectionEvents { get { return _eventAggregator; } set { _eventAggregator = value;} }
        
        public IPlatformAdaptor _platformAdaptor { get; set;}
        public IPlatformAdaptor PlatformAdaptor { get { return _platformAdaptor; } set { _platformAdaptor = value; } }

        public User accountDetails { get; set; }
        public TwitterCredentials TwitterCredentials { get; set; }
        public AccountSettings accountSettings { get; set; }

        public IUserSession Session;
        public IUserStream UserStream;
        public ISearchStream SearchStream;

        private readonly TimeSpan _multiFetchBackoffTimer = new TimeSpan(1200);

        public TwitterConnection(TwitterCredentials twitterCredentials, IEventAggregator eventAggregator, IPlatformAdaptor platformAdaptor)
        {
            TwitterCredentials = twitterCredentials;
            _platformAdaptor = platformAdaptor;
            _eventAggregator = eventAggregator;
            Session = new UserSession(TwitterCredentials, _platformAdaptor);
        }

        public async Task<bool> VerifyCredentials()
        {
            var checkedUser = await Session.GetVerifyCredentials();
            if (checkedUser.OK) // go deeper
            {
                accountSettings = await Session.GetAccountSettings();
                accountDetails = await Session.GetUserProfile(user_id: checkedUser.UserId);
                return accountDetails.OK;
            }
            else
                return false; // return false here will abandon all hope
        }

#if(WIN8RT)
        public TwitterConnection(string twitterConsumerKey,string twitterConsumerSecret, string callbackuri)
        {
            _twitterConsumerKey = twitterConsumerKey;
            CheckClientKey(twitterConsumerKey);
            _twitterConsumerSecret = twitterConsumerSecret;
            _callbackURI = callbackuri;
             _eventAggregator = new EventAggregator();
        }

       public TwitterConnection()
       {
            _eventAggregator = new EventAggregator();
           _platformAdaptor = new Win8RTPlatformAdaptor();
       }
#endif

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string callbackuri, string xauthusername, string xauthpassword, IPlatformAdaptor platformAdaptor)
        {
            _twitterConsumerKey = twitterConsumerKey;
            CheckClientKey(twitterConsumerKey);
            _twitterConsumerSecret = twitterConsumerSecret;
             _eventAggregator = new EventAggregator();
            _platformAdaptor = platformAdaptor;
        }

#if (WINDOWSDESKTOP)
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, IPlatformAdaptor platformAdaptor)
        {
            _twitterConsumerKey = twitterConsumerKey;
            CheckClientKey(twitterConsumerKey);
            _twitterConsumerSecret = twitterConsumerSecret;
            _eventAggregator = new EventAggregator();
            _platformAdaptor = platformAdaptor;
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret)
        {
            _twitterConsumerKey = twitterConsumerKey;
            CheckClientKey(twitterConsumerKey);
            _twitterConsumerSecret = twitterConsumerSecret;
            _eventAggregator = new EventAggregator();
            _platformAdaptor = new DesktopPlatformAdaptor();
         }


       public TwitterConnection()
       {
            _eventAggregator = new EventAggregator();           
           _platformAdaptor = new DesktopPlatformAdaptor();
       }
#endif

        private void CheckClientKey(string ck)
        {
            if (ck.ToLower().Contains("dev.twitter.com"))
            {
                Debug.WriteLine("You need to obtain a valid Client Secret & Client Key from dev.windows.com, and put it into the source code somewhere, dude.");;
            }
        }

        // auth happens when no creds are present
        public async Task<bool> BeginAuthentication()
        {
            var authstartok = await Session.StartAuthentication();
            return authstartok;
        }

        // second stage of auth confirms the pin is OK
        public async Task<TwitterCredentials> CompleteAuthentication(string pin)
        {
            var twittercredentials = await Session.ConfirmPin(pin);
            if (!twittercredentials.Valid) return null;
            return twittercredentials;
        }

        // another method using xauth as the authentication flow
        public async Task<TwitterCredentials> XAuthentication(string xauthusername, string xauthpassword)
        {
            var twittercredentials = await Session.XAuthentication(xauthusername, xauthpassword);
            if (!twittercredentials.Valid) return null;
            return twittercredentials;
        }

#if(WIN8RT)
        public async Task<TwitterCredentials> Authenticate()
        {
            var twittercredentials =  await Session.Authentication();
            if (!twittercredentials.Valid) return null;
            return twittercredentials;
        }
#endif

        // or just bypass and add an account from a re-hydrated credentials
        public async Task<TwitterAccount> AddTwitterAccount(TwitterCredentials twitterCredentials)
        {
            if (!twitterCredentials.Valid) return null;
            var newaccount = new TwitterAccount(twitterCredentials, twitterConnectionEvents, _platformAdaptor);
            var checkedcreds = await Session.VerifyCredentials();
            if (!checkedcreds) return null;

            // all ok, add to valid Twitter Accounts

            TwitterAccounts.Add(twitterCredentials.UserID, newaccount);
            return newaccount;
        }

    }
}
