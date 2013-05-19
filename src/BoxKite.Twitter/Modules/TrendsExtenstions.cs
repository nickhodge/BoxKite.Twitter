using System.Collections.Generic;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models.Service;

namespace BoxKite.Twitter
{
    public static class TrendsExtensions
    {
        /// <summary>
        /// Returns the top 10 trending topics for a specific WOEID, if trending information is available for it.
        /// </summary>
        /// <param name="place_id">The Yahoo! Where On Earth ID of the location to return trending information for. Global information is available by using 1 as the WOEID.</param>
        /// <param name="exclude">If true will remove all hashtags from the trends list.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/trends/place </remarks>
        public static async Task<IEnumerable<TrendsForPlaceResponse>> GetTrendsForPlace(this IUserSession session, string place_id, bool exclude = false)
        {
            var parameters = new SortedDictionary<string, string>{{"id",place_id},};
            if (exclude)
                parameters.Add("exclude","hashtags");

            var url = Api.Resolve("/1.1/trends/place.json");
            return await session.GetAsync(url, parameters)
                .ContinueWith(c => c.MapToTrendsForPlaceResponse());
        }

        /// <summary>
        /// Returns the locations that Twitter has trending topic information for.
        /// </summary>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/trends/available </remarks>
        public static async Task<IEnumerable<TrendsAvailableLocationsResponse>> GetTrendsAvailableLocations(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>();

            var url = Api.Resolve("/1.1/trends/available.json");
            return await session.GetAsync(url, parameters)
                .ContinueWith(c => c.MapToTrendsAvailableLocationsResponse());
        }


        public static async Task<IEnumerable<TrendsAvailableLocationsResponse>> GetTrendsByLocation(
            this IUserSession session, double latitude = 0.0,
            double longitude = 0.0)
        {
            var parameters = new SortedDictionary<string, string>
                             {
                                 {"lat", latitude.ToString()},
                                 {"long", longitude.ToString()},
                             };
            var url = Api.Resolve("/1.1/trends/closest.json");
            return await session.GetAsync(url, parameters)
                .ContinueWith(c => c.MapToTrendsAvailableLocationsResponse());
        }
    }
}