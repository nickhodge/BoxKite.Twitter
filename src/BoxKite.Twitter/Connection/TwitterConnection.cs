// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Threading.Tasks;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public class TwitterConnection : BindableBase
    {
        private string _publicState;
        public string PublicState
        {
            get { return _publicState; }
            set { SetProperty(ref _publicState, value); }
        }        
        public TwitterAccountsDictionary TwitterAccounts { get; set; }

        private readonly string _twitterConsumerKey;
        private readonly string _twitterConsumerSecret;
        private readonly TwitterAuthenticator _twitterauth;
        private readonly IPlatformAdaptor _platformAdaptor;

#if (PORTABLE)
        public TwitterConnection(string twitterConsumerKey,string twitterConsumerSecret, IPlatformAdaptor platformAdaptor)
        {
            _twitterConsumerKey = twitterConsumerKey;
            _twitterConsumerSecret = twitterConsumerSecret;
            _platformAdaptor = platformAdaptor;
            _twitterauth = new TwitterAuthenticator(_twitterConsumerKey, _twitterConsumerSecret, platformAdaptor);
        }
#elif (WINDOWS)
       public TwitterConnection(string twitterConsumerKey,string twitterConsumerSecret)
        {
            _twitterConsumerKey = twitterConsumerKey;
            _twitterConsumerSecret = twitterConsumerSecret;
            _twitterauth = new TwitterAuthenticator(_twitterConsumerKey, _twitterConsumerSecret);
        }
#endif

        // auth happens when no creds are present
        public async Task<bool> BeginAuthentication()
        {
            PublicState = "Authenticating with Twitter";
            var authstartok = await _twitterauth.StartAuthentication();
            PublicState = "";
            return authstartok;
        }

        // second stage of auth confirms the pin is OK
        public async Task<bool> CompleteAuthentication(string pin)
        {
            PublicState = "Validating PIN";
            var twittercredentials = await _twitterauth.ConfirmPin(pin);
            PublicState = "";
            if (!twittercredentials.Valid) return false;
            return await AddTwitterAccount(twittercredentials);
        }

        // or just bypass and add an account from a re-hydrated credentials
        public async Task<bool> AddTwitterAccount(TwitterCredentials twitterCredentials)
        {
            if (!twitterCredentials.Valid) return false;

            //var newaccount = new TwitterAccount(twitterCredentials);
            PublicState = String.Format("Verifying Credentials for {0}", twitterCredentials.ScreenName);
            //var checkedcreds = await newaccount.VerifyCredentials();
            PublicState = "";
            //if (!checkedcreds) return false;

            // all ok, add to valid Twitter Accounts

            // TODO: REMOVE INT32
            //TwitterAccounts.Add(twitterCredentials.UserID, newaccount);
            return true;
        }

    }
}
