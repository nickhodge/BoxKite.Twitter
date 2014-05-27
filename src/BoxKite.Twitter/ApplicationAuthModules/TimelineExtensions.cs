﻿// // (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public static partial class TimelineExtensions
    {
        /// <summary>
        /// Returns a collection of the most recent Tweets posted by the user indicated by the screen_name or user_id parameters.
        /// Using ApplicationAuth
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
        public async static Task<TwitterResponseCollection<Tweet>> GetUserTimeline(this IApplicationSession appsession, string screen_name = "", long user_id = 0, long since_id = 0, long max_id = 0, int count = 200, bool excludereplies = true, bool include_rts = true)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true, include_rts: true, count: count, since_id: since_id, max_id: max_id, screen_name:screen_name);

            return await appsession.GetAsync(TwitterApi.Resolve("/1.1/statuses/user_timeline.json"), parameters)
                          .ContinueWith(c => c.MapToMany<Tweet>());
        }
    }
}