// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;
using Newtonsoft.Json.Schema;
using Reactive.EventAggregator;

namespace BoxKite.Twitter
{
    public class TwitterConnection : BindableBase
    {
        private TwitterAccountsDictionary _twitterAccounts = new TwitterAccountsDictionary();
        public TwitterAccountsDictionary TwitterAccounts { get { return _twitterAccounts; } set { _twitterAccounts = value; } }

        private readonly string _twitterConsumerKey;
        private readonly string _twitterConsumerSecret;
        private readonly string _callbackURI;
        private readonly TwitterAuthenticator _twitterauth;
        private readonly IPlatformAdaptor _platformAdaptor;

        private IEventAggregator _eventAggregator;
        public IEventAggregator twitterConnectionEvents { get { return _eventAggregator; } set { _eventAggregator = value;} }

#if (PORTABLE)
        public TwitterConnection(string twitterConsumerKey,string twitterConsumerSecret, IPlatformAdaptor platformAdaptor)
        {
            _twitterConsumerKey = twitterConsumerKey;
            CheckClientKey(twitterConsumerKey);
            _twitterConsumerSecret = twitterConsumerSecret;
            _eventAggregator = new EventAggregator();
            _platformAdaptor = platformAdaptor;
            _twitterauth = new TwitterAuthenticator(_twitterConsumerKey, _twitterConsumerSecret, platformAdaptor);
        }

        public TwitterConnection(IPlatformAdaptor platformAdaptor)
        {
            _eventAggregator = new EventAggregator();
            _platformAdaptor = platformAdaptor;
        }
#endif

#if(WIN8RT)
        public TwitterConnection(string twitterConsumerKey,string twitterConsumerSecret, string callbackuri)
        {
            _twitterConsumerKey = twitterConsumerKey;
            CheckClientKey(twitterConsumerKey);
            _twitterConsumerSecret = twitterConsumerSecret;
            _callbackURI = callbackuri;
             _eventAggregator = new EventAggregator();
            _twitterauth = new TwitterAuthenticator(_twitterConsumerKey, _twitterConsumerSecret, _callbackURI);
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
             _twitterauth = new TwitterAuthenticator(_twitterConsumerKey, _twitterConsumerSecret, platformAdaptor);
        }

#if (WINDOWSDESKTOP)
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, IPlatformAdaptor platformAdaptor)
        {
            _twitterConsumerKey = twitterConsumerKey;
            CheckClientKey(twitterConsumerKey);
            _twitterConsumerSecret = twitterConsumerSecret;
            _eventAggregator = new EventAggregator();
            _platformAdaptor = platformAdaptor;
            _twitterauth = new TwitterAuthenticator(_twitterConsumerKey, _twitterConsumerSecret, platformAdaptor);
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret)
        {
            _twitterConsumerKey = twitterConsumerKey;
            CheckClientKey(twitterConsumerKey);
            _twitterConsumerSecret = twitterConsumerSecret;
            _eventAggregator = new EventAggregator();
            _platformAdaptor = new DesktopPlatformAdaptor();
            _twitterauth = new TwitterAuthenticator(_twitterConsumerKey, _twitterConsumerSecret, _platformAdaptor);
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
            var authstartok = await _twitterauth.StartAuthentication();
            return authstartok;
        }

        // second stage of auth confirms the pin is OK
        public async Task<TwitterAccount> CompleteAuthentication(string pin)
        {
            var twittercredentials = await _twitterauth.ConfirmPin(pin);
            if (!twittercredentials.Valid) return null;
            return await AddTwitterAccount(twittercredentials);
        }

        // another method using xauth as the authentication flow
        public async Task<TwitterAccount> XAuthentication(string xauthusername, string xauthpassword)
        {
            var twittercredentials = await _twitterauth.XAuthentication(xauthusername, xauthpassword);
            if (!twittercredentials.Valid) return null;
            return await AddTwitterAccount(twittercredentials);
        }

#if(WIN8RT)
        public async Task<TwitterAccount> Authenticate()
        {
            var twittercredentials =  await _twitterauth.Authentication();
            if (!twittercredentials.Valid) return null;
            return await AddTwitterAccount(twittercredentials);
        }
#endif

        // or just bypass and add an account from a re-hydrated credentials
        public async Task<TwitterAccount> AddTwitterAccount(TwitterCredentials twitterCredentials)
        {
            if (!twitterCredentials.Valid) return null;
            var newaccount = new TwitterAccount(twitterCredentials, twitterConnectionEvents, _platformAdaptor);
            var checkedcreds = await newaccount.VerifyCredentials();
            if (!checkedcreds) return null;

            // all ok, add to valid Twitter Accounts

            TwitterAccounts.Add(twitterCredentials.UserID, newaccount);
            return newaccount;
        }

    }

    public class TwitterUserStreamDisconnectEvent
    {

    }

    public class TwitterSearchStreamDisconnectEvent
    {
        
    }

}
