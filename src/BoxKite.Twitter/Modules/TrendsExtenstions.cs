// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

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
        public static async Task<TwitterResponseCollection<TrendsForPlaceResponse>> GetTrendsForPlace(this IUserSession session, int place_id = 1, bool exclude = false)
        {
            var parameters = new TwitterParametersCollection
                        {{"id",place_id.ToString()}};
            if (exclude)
                parameters.Add("exclude","hashtags");

            return await session.GetAsync(Api.Resolve("/1.1/trends/place.json"), parameters)
                .ContinueWith(c => c.MapToMany<TrendsForPlaceResponse>());
        }

        /// <summary>
        /// Returns the locations that Twitter has trending topic information for.
        /// </summary>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/trends/available </remarks>
        public static async Task<TwitterResponseCollection<TrendsAvailableLocationsResponse>> GetTrendsAvailableLocations(this IUserSession session)
        {
            var parameters = new TwitterParametersCollection();

            return await session.GetAsync(Api.Resolve("/1.1/trends/available.json"), parameters)
                .ContinueWith(c => c.MapToMany<TrendsAvailableLocationsResponse>());
        }

        /// <summary>
        /// Returns the locations that Twitter has trending topic information for, closest to a specified location.
        /// </summary>
        /// <param name="latitude">If provided with a long parameter the available trend locations will be sorted by distance, nearest to furthest, to the co-ordinate pair.</param>
        /// <param name="longitude">If provided with a lat parameter the available trend locations will be sorted by distance, nearest to furthest, to the co-ordinate pair.</param>
        /// <returns></returns>
        /// <remarks> ref:  https://dev.twitter.com/docs/api/1.1/get/trends/closest </remarks>
        public static async Task<TwitterResponseCollection<TrendsAvailableLocationsResponse>> GetTrendsByLocation(
            this IUserSession session, double latitude = 0.0,
            double longitude = 0.0)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"lat", latitude.ToString()},
                                 {"long", longitude.ToString()},
                             };

            return await session.GetAsync(Api.Resolve("/1.1/trends/closest.json"), parameters)
                .ContinueWith(c => c.MapToMany<TrendsAvailableLocationsResponse>());
        }
    }
}