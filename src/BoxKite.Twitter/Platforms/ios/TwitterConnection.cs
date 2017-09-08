using BoxKite.Twitter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxKite.Twitter
{
    public partial class TwitterConnection
    {
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret)
        {
            PlatformAdaptor = new IOSPlatformAdaptor();
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
            PlatformAdaptor = new IOSPlatformAdaptor();
            TwitterCredentials = twitterCredentials;
            UserSession = BuildUserSession();
        }

        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername,
            string xauthpassword)
        {
            PlatformAdaptor = new IOSPlatformAdaptor();
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
        }
    }
}