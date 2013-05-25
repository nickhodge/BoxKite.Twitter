// // (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public static class TimelineExtensions
    {
        /// <summary>
        /// Returns the count (default) 20 most recent mentions (tweets containing a users's @screen_name) for the authenticating user
        /// </summary>
        /// <param name="since_id">Returns results with an ID greater than (that is, more recent than) the specified ID.</param>
        /// <param name="count">Specifies the number of tweets to try and retrieve, up to a maximum of 200.</param>
        /// <param name="max_id">Returns results with an ID less than (that is, older than) or equal to the specified ID.</param>
        /// <returns></returns>
        /// <remarks> ref :https://dev.twitter.com/docs/api/1.1/get/statuses/mentions_timeline </remarks>
        public async static Task<TwitterResponseCollection<Tweet>> GetMentions(this IUserSession session, long since_id = 0, long max_id = 0, int count = 20)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true, count: count, since_id: since_id, max_id: max_id);

            return await session.GetAsync(Api.Resolve("/1.1/statuses/mentions_timeline.json"), parameters)
                          .ContinueWith(c => c.MapToMany<Tweet>());
        }

        /// <summary>
        /// Returns a collection of the most recent Tweets posted by the user indicated by the screen_name or user_id parameters.
        /// </summary>
        /// <param name="user_id">The ID of the user for whom to return results for.</param>
        /// <param name="screen_name">The screen name of the user for whom to return results for.</param>
        /// <param name="since_id">Returns results with an ID greater than (that is, more recent than) the specified ID.</param>
        /// <param name="count">Specifies the number of tweets to try and retrieve, up to a maximum of 200 per distinct request</param>
        /// <param name="max_id">Returns results with an ID less than (that is, older than) or equal to the specified ID.</param>
        /// <param name="excludereplies">This parameter will prevent replies from appearing in the returned timeline. </param>
        /// <param name="include_rts">When set to false, the timeline will strip any native retweets</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/statuses/user_timeline </remarks>
        public async static Task<TwitterResponseCollection<Tweet>> GetUserTimeline(this IUserSession session, string screen_name = "", int user_id = 0, long since_id = 0, long max_id = 0, int count = 200, bool excludereplies = true, bool include_rts = true)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true, include_rts: true, count: count, since_id: since_id, max_id: max_id);

            return await session.GetAsync(Api.Resolve("/1.1/statuses/user_timeline.json"), parameters)
                          .ContinueWith(c => c.MapToMany<Tweet>());
        }

        /// <summary>
        /// Returns a collection of the most recent Tweets and retweets posted by the authenticating user and the users they follow. The home timeline is central to how most users interact with the Twitter service.
        /// </summary>
        /// <param name="since_id">Returns results with an ID greater than (that is, more recent than) the specified ID.</param>
        /// <param name="count">Specifies the number of records to retrieve. Must be less than or equal to 200. Defaults to 20.</param>
        /// <param name="max_id">Returns results with an ID less than (that is, older than) or equal to the specified ID.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/statuses/home_timeline </remarks>
        public async static Task<TwitterResponseCollection<Tweet>> GetHomeTimeline(this IUserSession session, long since_id = 0, int count = 20, long max_id = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true, include_rts:true,  count:count, since_id: since_id, max_id: max_id);

            return await session.GetAsync(Api.Resolve("/1.1/statuses/home_timeline.json"), parameters)
                          .ContinueWith(c => c.MapToMany<Tweet>());
        }

        /// <summary>
        /// Returns the most recent tweets authored by the authenticating user that have been retweeted by others. 
        /// </summary>
        /// <param name="since_id">Returns results with an ID greater than (that is, more recent than) the specified ID.</param>
        /// <param name="count">Specifies the number of records to retrieve. Must be less than or equal to 100. If omitted, 20 will be assumed.</param>
        /// <param name="max_id">Returns results with an ID less than (that is, older than) or equal to the specified ID.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/statuses/retweets_of_me </remarks>
        public async static Task<TwitterResponseCollection<Tweet>> GetRetweetsOfMe(this IUserSession session, long since_id = 0, long max_id = 0, int count = 20)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true, include_rts: true, count: count, since_id: since_id, max_id: max_id);

            return await session.GetAsync(Api.Resolve("/1.1/statuses/retweets_of_me.json"), parameters)
                          .ContinueWith(c => c.MapToMany<Tweet>());
        }
    }
}
