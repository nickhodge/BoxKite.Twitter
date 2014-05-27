// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.IO;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    // TODO: manage https://support.twitter.com/articles/14020-twitter-for-sms-basic-features

    public static partial class TweetExtensions
    {
        /// <summary>
        /// Gets a particular Tweet
        /// </summary>
        /// <param name="tweetID">The tweet ID to return</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/statuses/show/%3Aid  </remarks>
        public async static Task<Tweet> GetTweet(this IApplicationSession appsession, long tweetID)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"trim_user",false.ToString()},
                                 {"include_my_retweet",true.ToString()},
                             };
            parameters.Create(id: tweetID, include_entities:true);

            return await appsession.GetAsync(TwitterApi.Resolve("/1.1/statuses/show.json"), parameters)
                .ContinueWith(c => c.MapToSingle<Tweet>());
        }


        /// <summary>
        /// Gets the Retweets of a particular tweet
        /// </summary>
        /// <param name="tweet"></param>
        /// <param name="count"></param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/statuses/retweets/%3Aid  </remarks>
        public async static Task<TwitterResponseCollection<Tweet>> GetRetweets(this IApplicationSession appsession, Tweet tweet, int count = 20)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"count", count.ToString()},
                             };
            var path = TwitterApi.Resolve("/1.1/statuses/retweets/{0}.json", tweet.Id);
            return await appsession.GetAsync(path, parameters)
                .ContinueWith(c => c.MapToMany<Tweet>());
        }
    }
}
