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
        /// <returns></returns>
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

    }
}
