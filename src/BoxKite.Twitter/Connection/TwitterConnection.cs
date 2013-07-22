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
#elif (WINDOWS || WIN8RT)
        public TwitterConnection(string twitterConsumerKey,string twitterConsumerSecret)
        {
            _twitterConsumerKey = twitterConsumerKey;
            CheckClientKey(twitterConsumerKey);
            _twitterConsumerSecret = twitterConsumerSecret;
             _eventAggregator = new EventAggregator();
            _twitterauth = new TwitterAuthenticator(_twitterConsumerKey, _twitterConsumerSecret);
        }
#endif


#if (PORTABLE)
        public TwitterConnection(IPlatformAdaptor platformAdaptor)
        {
            _eventAggregator = new EventAggregator();
            _platformAdaptor = platformAdaptor;
        }
#elif (WINDOWS)
       public TwitterConnection()
       {
            _eventAggregator = new EventAggregator();           
           _platformAdaptor = new DesktopPlatformAdaptor();
       }
#elif (WIN8RT)
       public TwitterConnection()
       {
            _eventAggregator = new EventAggregator();
           _platformAdaptor = new Win8RTPlatformAdaptor();
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

        // or just bypass and add an account from a re-hydrated credentials
        public async Task<TwitterAccount> AddTwitterAccount(TwitterCredentials twitterCredentials)
        {
            if (!twitterCredentials.Valid) return null;

#if (PORTABLE)
            var newaccount = new TwitterAccount(twitterCredentials, twitterConnectionEvents, _platformAdaptor);
#elif (WINDOWS || WIN8RT)
            var newaccount = new TwitterAccount(twitterCredentials, twitterConnectionEvents);
#endif
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
