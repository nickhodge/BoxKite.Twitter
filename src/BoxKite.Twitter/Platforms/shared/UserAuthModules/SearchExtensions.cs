// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public static class SearchExtensions
    {
        /// <summary>
        /// dedicated API for running searches against the real-time index of recent Tweets. 6-9 days of historical data
        /// </summary>
        /// <param name="searchText">search query of 1,000 characters maximum, including operators. Queries may additionally be limited by complexity.</param>
        /// <param name="maxId"></param>
        /// <param name="sinceId"></param>
        /// <param name="untilDate">YYYY-MM-DD format</param>
        /// <param name="count">Tweets to return Default 20</param>
        /// <param name="searchResponseType">SearchResult.Mixed (default), SearchResult.Recent, SearchResult.Popular</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/search/tweets </remarks>
        public static async Task<SearchResponse> SearchFor(this ITwitterSession session, string searchText, SearchResultType searchResponseType, long maxId = 0, long sinceId = 0, string untilDate = "", int count = 20)
        {
            var parameters = new TwitterParametersCollection
                                 {
                                     {"q", searchText.TrimAndTruncate(1000).UrlEncode()},
                                     {"result_type", SearchResultString(searchResponseType)},
                                 };
            parameters.Create(since_id:sinceId,max_id:maxId,count:count,include_entities:true);

            if (!string.IsNullOrWhiteSpace(untilDate))
            {
                parameters.Add("until", untilDate);
            }

            return await session.GetAsync(TwitterApi.Resolve("/1.1/search/tweets.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<SearchResponse>());
        }

        /// <summary>
        /// Geotagged dedicated API for running searches against the real-time index of recent Tweets. 6-9 days
        /// </summary>
        /// <param name="searchText">search query of 1,000 characters maximum, including operators. Queries may additionally be limited by complexity.</param>
        /// <param name="latitude">Returns tweets by users located within a given radius of the given latitude/longitude.</param>
        /// <param name="longitude">Returns tweets by users located within a given radius of the given latitude/longitude.</param>
        /// <param name="distance">Returns tweets by users located within a given radius of the given latitude/longitude.</param>
        /// <param name="distanceUnits">km (default) or mi</param>
        /// <param name="maxId"></param>
        /// <param name="sinceId"></param>
        /// <param name="untilDate">YYYY-MM-DD format</param>
        /// <param name="count">Tweets to return Default 20</param>
        /// <param name="searchResponseType">SearchResult.Mixed (default), SearchResult.Recent, SearchResult.Popular</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/search/tweets </remarks>
        public static async Task<SearchResponse> SearchFor(this ITwitterSession session, string searchText, SearchResultType searchResponseType, double latitude, double longitude, double distance, string distanceUnits="km", long maxId = 0, long sinceId = 0, string untilDate = "", int count = 20)
        {
            var parameters = new TwitterParametersCollection
                                 {
                                     {"q", searchText.TrimAndTruncate(1000).UrlEncode()},
                                     {"result_type", SearchResultString(searchResponseType)},
                                 };
            parameters.Create(since_id: sinceId, max_id: maxId, count: count, include_entities: true);

            if (!string.IsNullOrWhiteSpace(untilDate))
            {
                parameters.Add("until", untilDate);
            }

            parameters.Add("geocode", $"{latitude},{longitude},{distance}{distanceUnits}");

            return await session.GetAsync(TwitterApi.Resolve("/1.1/search/tweets.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<SearchResponse>());
        }

        /// <summary>
        /// Returns the authenticated user's saved search queries.
        /// </summary>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/saved_searches/list </remarks>
        public static async Task<TwitterResponseCollection<SavedSearch>> GetSavedSearches(this IUserSession session)
        {
            var parameters = new TwitterParametersCollection();
            return await session.GetAsync(TwitterApi.Resolve("/1.1/saved_searches/list.json"), parameters)
                          .ContinueWith(c => c.MapToMany<SavedSearch>());
        }

        /// <summary>
        /// Retrieve the information for the saved search represented by the given id.
        /// </summary>
        /// <param name="id">The ID of the saved search.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/saved_searches/show/%3Aid </remarks>
        public static async Task<SavedSearch> GetSaveASearch(this IUserSession session, string id)
        {
            var parameters = new TwitterParametersCollection();
            var url = TwitterApi.Resolve("/1.1/saved_searches/show/{0}.json", id); 
            return await session.GetAsync(url, parameters)
                .ContinueWith(c => c.MapToSingle<SavedSearch>());
        }

        /// <summary>
        /// Saves a search
        /// </summary>
        /// <param name="searchText">search query of 1,000 characters maximum, including operators. Queries may additionally be limited by complexity.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/saved_searches/create </remarks>
        public static async Task<SavedSearch> CreateSaveSearch(this IUserSession session, string searchText)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"query", searchText.TrimAndTruncate(1000).UrlEncode()},
                             };
            return await session.PostAsync(TwitterApi.Resolve("/1.1/saved_searches/create.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<SavedSearch>());
        }

        /// <summary>
        /// Creates a saved search
        /// </summary>
        /// <param name="SavedSearchId">The ID of the saved search.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/saved_searches/destroy/%3Aid </remarks>
        public static async Task<SavedSearch> DeleteSavedSearch(this IUserSession session, string SavedSearchId)
        {
            var parameters = new TwitterParametersCollection();
            var savedSearch = TwitterApi.Resolve("/1.1/saved_searches/destroy/{0}.json", SavedSearchId);
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


    public enum SearchResultType
    {
        Mixed = 1,
        Recent = 2,
        Popular = 3
     }
}
