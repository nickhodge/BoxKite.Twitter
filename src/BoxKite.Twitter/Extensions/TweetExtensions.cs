// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Models
{
    public static class TweetExtensions
    {
        public static bool HasEntities(this Tweet tweet)
        {
            if (tweet.Entities.Hashtags.HasAny())
                return true;

            if (tweet.Entities.Media.HasAny())
                return true;

            if (tweet.Entities.Mentions.HasAny())
                return true;

            if (tweet.Entities.Urls.HasAny())
                return true;

            return false;
        }

        public static bool IsAReply(this Tweet tweet)
        {
            return tweet.InReplyToId != null;
        }

        public static bool IsARetweet(this Tweet tweet)
        {
            return tweet.RetweetedStatus != null;
        }
    }
}
