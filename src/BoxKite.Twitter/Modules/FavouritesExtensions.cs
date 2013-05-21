using System.Collections.Generic;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Modules
{
    public static class FavouritesExtensions
    {
        /// <summary>
        /// https://dev.twitter.com/docs/api/1.1/get/favorites/list
        /// Returns the count most recent Tweets favorited by the authenticating or specified user.
        /// If user_id and screen_name is left blank, current auth'd user favourites are returned
        /// Entities are always returned
        /// </summary>
        /// <param name="user_id">The ID of the user for whom to return results for</param>
        /// <param name="screen_name">The screen name of the user for whom to return results for</param>
        /// <param name="since_id">Returns results with an ID greater than</param>
        /// <param name="count">Specifies the number of records to retrieve. Must be less than or equal to 200. Defaults to 20.</param>
        /// <param name="max_id">Returns results with an ID less than (that is, older than) or equal to the specified </param>
        /// <returns></returns>
        public async static Task<TwitterResponseCollection<Tweet>> GetFavourites(this IUserSession session, long user_id = 0, string screen_name = "", long since_id = 0, int count = 20, long max_id = 0)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"count", count.ToString()},
                                     {"include_entities", true.ToString()},
                                 };

            if (since_id != 0)
            {
                parameters.Add("since_id", since_id.ToString());
            }

            if (max_id !=0)
            {
                parameters.Add("max_id", max_id.ToString());
            }

            if (user_id != 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            var url = Api.Resolve("/1.1/favorites/list.json");
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
            var parameters = new SortedDictionary<string, string>
                                 {
                                      { "id", tweet.Id.ToString() }
                                 };

            var url = Api.Resolve("/1.1/favorites/create.json");
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
            var parameters = new SortedDictionary<string, string>
                                 {
                                      { "id", tweet.Id.ToString() }
                                 };

            var url = Api.Resolve("/1.1/favorites/destroy.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToSingle<Tweet>());
        }
    }
}
