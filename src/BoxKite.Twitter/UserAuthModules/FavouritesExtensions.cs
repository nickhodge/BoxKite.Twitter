// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public static class FavouritesExtensions
    {
        /// <summary>
        /// https://dev.twitter.com/docs/api/1.1/get/favorites/list
        /// Returns the count most recent Tweets favorited by the authenticating or specified user.
        /// If user_id and screen_name is left blank, current auth'd user favourites are returned
        /// Entities are always returned
        /// </summary>
        /// <param name="userId">The ID of the user for whom to return results for</param>
        /// <param name="screenName">The screen name of the user for whom to return results for</param>
        /// <param name="sinceId">Returns results with an ID greater than</param>
        /// <param name="count">Specifies the number of records to retrieve. Must be less than or equal to 200. Defaults to 20.</param>
        /// <param name="maxId">Returns results with an ID less than (that is, older than) or equal to the specified </param>
        /// <returns></returns>
        public async static Task<TwitterResponseCollection<Tweet>> GetFavourites(this ITwitterSession session, string screenName = "", int userId = 0, long sinceId = 0, long maxId = 0, int count = 20 )
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(count:count,include_entities:true,since_id:sinceId,max_id:maxId, user_id:userId, screen_name:screenName);

            var url = TwitterApi.Resolve("/1.1/favorites/list.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToMany<Tweet>());
        }

        /// <summary>
        /// https://dev.twitter.com/docs/api/1.1/post/favorites/create
        /// Favourites a given tweet
        /// </summary>
        /// <param name="tweet">Tweet for to favourite</param>
        /// <returns></returns>
        public async static Task<Tweet> CreateFavourite(this IUserSession session, Tweet tweet)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(id: tweet.Id);

            var url = TwitterApi.Resolve("/1.1/favorites/create.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// https://dev.twitter.com/docs/api/1.1/post/favorites/destroy
        /// Un-favourites a given tweet
        /// </summary>
        /// <param name="tweet">Tweet for to favourite</param>
        /// <returns></returns>
        public async static Task<Tweet> DeleteFavourite(this IUserSession session, Tweet tweet)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(id: tweet.Id);

            var url = TwitterApi.Resolve("/1.1/favorites/destroy.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToSingle<Tweet>());
        }
    }
}
