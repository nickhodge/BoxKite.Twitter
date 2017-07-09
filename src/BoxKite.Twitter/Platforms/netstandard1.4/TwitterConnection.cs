using System;
using System.Collections.Generic;
using System.Text;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public partial class TwitterConnection
    {
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

        // ReSharper disable once UnusedParameter.Local
        public TwitterConnection(string twitterConsumerKey, string twitterConsumerSecret, string xauthusername, string xauthpassword, IPlatformAdaptor platformAdaptor = null)
        {
            PlatformAdaptor = platformAdaptor;
            UserSession = BuildUserSession(twitterConsumerKey, twitterConsumerSecret);
        }
    }
}
