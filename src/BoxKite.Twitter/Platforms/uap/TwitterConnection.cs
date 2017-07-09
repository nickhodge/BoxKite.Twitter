using System;
using System.Collections.Generic;
using System.Text;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public partial class TwitterConnection
    {
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret)
        {
            PlatformAdaptor = new WinUWPPlatformAdaptor();
            ApplicationSession = BuildApplicationSession(twitterConsumerKey, twitterConsumerSecret);
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret,
            IPlatformAdaptor platformAdaptor = null)
        {
            PlatformAdaptor = platformAdaptor;
            ApplicationSession = BuildApplicationSession(twitterConsumerKey, twitterConsumerSecret);
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
        }

        public TwitterConnection(TwitterCredentials twitterCredentials)
        {
            PlatformAdaptor = new WinUWPPlatformAdaptor();
            TwitterCredentials = twitterCredentials;
            UserSession = BuildUserSession();
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername,
            string xauthpassword)
        {
            PlatformAdaptor = new WinUWPPlatformAdaptor();
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
        }
    }
}