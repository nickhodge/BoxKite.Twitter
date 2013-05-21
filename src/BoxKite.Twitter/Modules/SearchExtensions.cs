using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;
using SearchResponse = BoxKite.Twitter.Models.SearchResponse;

namespace BoxKite.Twitter
{
    public static class SearchExtensions
    {
        /// <summary>
        /// dedicated API for running searches against the real-time index of recent Tweets. 6-9 days
        /// </summary>
        /// <param name="searchtext">search query of 1,000 characters maximum, including operators. Queries may additionally be limited by complexity.</param>
        /// <param name="max_id"></param>
        /// <param name="since_id"></param>
        /// <param name="untilDate">YYYY-MM-DD format</param>
        /// <param name="count">Tweets to return Default 20</param>
        /// <param name="searchResponseType">SearchResult.Mixed (default), SearchResult.Recent, SearchResult.Popular</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/search/tweets </remarks>
        public async static Task<SearchResponse> SearchFor(this IUserSession session, string searchtext, SearchResultType searchResponseType, long max_id = 0, long since_id = 0, string untilDate = "", int count = 20)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"q", searchtext.UrlEncode()},
                                     {"count",count.ToString()},
                                     {"include_entities", true.ToString()},
                                     {"result_type", SearchResultString(searchResponseType)},
                                 };

            if (since_id > 0)
            {
                parameters.Add("since_id", since_id.ToString());
            }

            if (max_id > 0)
            {
                parameters.Add("max_id", max_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(untilDate))
            {
                parameters.Add("until", untilDate);
            }

            var url = Api.Resolve("/1.1/search/tweets.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToSingle<SearchResponse>());
        }

        /// <summary>
        /// Geotagged dedicated API for running searches against the real-time index of recent Tweets. 6-9 days
        /// </summary>
        /// <param name="latitude">Returns tweets by users located within a given radius of the given latitude/longitude.</param>
        /// <param name="longitude">Returns tweets by users located within a given radius of the given latitude/longitude.</param>
        /// <param name="distance">Returns tweets by users located within a given radius of the given latitude/longitude.</param>
        /// <param name="distanceUnits">km (default) or mi</param>
        /// <param name="searchtext">search query of 1,000 characters maximum, including operators. Queries may additionally be limited by complexity.</param>
        /// <param name="max_id"></param>
        /// <param name="since_id"></param>
        /// <param name="untilDate">YYYY-MM-DD format</param>
        /// <param name="count">Tweets to return Default 20</param>
        /// <param name="searchResponseType">SearchResult.Mixed (default), SearchResult.Recent, SearchResult.Popular</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/search/tweets </remarks>
        public async static Task<SearchResponse> SearchFor(this IUserSession session, string searchtext, SearchResultType searchResponseType, double latitude, double longitude, double distance, string distanceUnits="km", long max_id = 0, long since_id = 0, string untilDate = "", int count = 20)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"q", searchtext.UrlEncode()},
                                     {"count",count.ToString()},
                                     {"include_entities", true.ToString()},
                                     {"result_type", SearchResultString(searchResponseType)},
                                 };

            if (since_id > 0)
            {
                parameters.Add("since_id", since_id.ToString());
            }

            if (max_id > 0)
            {
                parameters.Add("max_id", max_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(untilDate))
            {
                parameters.Add("until", untilDate);
            }

            parameters.Add("geocode",String.Format("{0},{1},{2}{3}",latitude,longitude,distance,distanceUnits));

            var url = Api.Resolve("/1.1/search/tweets.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToSingle<SearchResponse>());
        }

        /// <summary>
        /// Returns the authenticated user's saved search queries.
        /// </summary>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/saved_searches/list </remarks>
        public async static Task<TwitterResponseCollection<SavedSearch>> GetSavedSearches(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>();
            return await session.GetAsync(Api.Resolve("/1.1/saved_searches/list.json"), parameters)
                          .ContinueWith(c => c.MapToMany<SavedSearch>());
        }

        /// <summary>
        /// Retrieve the information for the saved search represented by the given id.
        /// </summary>
        /// <param name="id">The ID of the saved search.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/saved_searches/show/%3Aid </remarks>
        public async static Task<SavedSearch> GetSaveASearch(this IUserSession session, string id)
        {
            var url = Api.Resolve("/1.1/saved_searches/show/{0}.json", id);
            var parameters = new SortedDictionary<string, string>();
            return await session.GetAsync(url, parameters)
                .ContinueWith(c => c.MapToSingle<SavedSearch>());
        }

        /// <summary>
        /// Saves a search
        /// </summary>
        /// <param name="searchtext">search query of 1,000 characters maximum, including operators. Queries may additionally be limited by complexity.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/saved_searches/create </remarks>
        public async static Task<SavedSearch> CreateSaveSearch(this IUserSession session, string searchtext)
        {
            var savedSearch = Api.Resolve("/1.1/saved_searches/create.json");
            var parameters = new SortedDictionary<string, string>
                             {
                                 {"query", searchtext.UrlEncode()},
                             };
            return await session.PostAsync(savedSearch, parameters)
                          .ContinueWith(c => c.MapToSingle<SavedSearch>());
        }

        /// <summary>
        /// Creates a saved search
        /// </summary>
        /// <param name="searchtext">The ID of the saved search.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/saved_searches/destroy/%3Aid </remarks>
        public async static Task<SavedSearch> DeleteSavedSearch(this IUserSession session, string id)
        {
            var savedSearch = Api.Resolve("/1.1/saved_searches/destroy/{0}.json", id);
            var parameters = new SortedDictionary<string, string>();
            return await session.PostAsync(savedSearch, parameters)
                          .ContinueWith(c => c.MapToSingle<SavedSearch>());
        }

        private static string SearchResultString(SearchResultType searchResult)
        {
            if (searchResult == SearchResultType.Recent)
                return "recent";
            return searchResult == SearchResultType.Popular ? "popular" : "mixed";
        }

    }


    public enum SearchResultType : int
    {
        Mixed = 1,
        Recent = 2,
        Popular = 3
     }
}
