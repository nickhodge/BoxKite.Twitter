// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using System.Threading.Tasks;

namespace BoxKite.Twitter
{
    public static class PlacesGeoExtensions
    {
        /// <summary>
        /// Returns all the information about a known place.
        /// </summary>
        /// <param name="placeId">A place in the world.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/geo/id/%3Aplace_id </remarks>
        public static async Task<Place> GetPlaceInfo(this IUserSession session, string placeId)
        {
            var parameters = new SortedDictionary<string, string>();
            var url = TwitterApi.Resolve("/1.1/geo/id/{0}.json", placeId);
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToSingle<Place>());
        }

        /// <summary>
        /// Given a latitude and a longitude, searches for up to 20 places that can be used as a place_id when updating a status.
        /// </summary>
        /// <param name="latitude">The latitude to search around.</param>
        /// <param name="longitude">The longitude to search around.</param>
        /// <param name="accuracy">A hint on the "region" in which to search. If a number, then this is a radius in meters, but it can also take a string that is suffixed with ft to specify feet. If this is not passed in, then it is assumed to be 0m.</param>
        /// <param name="granularity">This is the minimal granularity of place types to return and must be one of: poi, neighborhood, city, admin or country.</param>
        /// <param name="maxResults">A hint as to the number of results to return.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/geo/reverse_geocode </remarks>
        public static async Task<ReverseGeoCodePlaces> GetPlaceIDFromGeocode(this IUserSession session, double latitude = 0.0,
            double longitude = 0.0, string accuracy = "10m", string granularity = "neighborhood", int maxResults=20)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"lat", latitude.ToString()},
                                 {"long", longitude.ToString()},
                                 {"accuracy", accuracy},
                                 {"granularity", granularity},
                                 {"max_results", maxResults.ToString()}
                             };

            return await session.GetAsync(TwitterApi.Resolve("/1.1/geo/reverse_geocode.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<ReverseGeoCodePlaces>());
 
        }

        /// <summary>
        /// Search for places that can be attached to a statuses/update. Given a latitude and a longitude pair, an IP address, or a name, this request will return a list of all the valid places that can be used as the place_id when updating a status
        /// </summary>
        /// <param name="query">Free-form text to match against while executing a geo-based query, best suited for finding nearby locations by name. Remember to URL encode the query.</param>
        /// <param name="latitude">The latitude to search around.</param>
        /// <param name="longitude">The longitude to search around.</param>
        /// <param name="accuracy">A hint on the "region" in which to search. If a number, then this is a radius in meters, but it can also take a string that is suffixed with ft to specify feet. If this is not passed in, then it is assumed to be 0m.</param>
        /// <param name="containedWithin">This is the place_id which you would like to restrict the search results to. Setting this value means only places within the given place_id will be found.</param>
        /// <param name="granularity">This is the minimal granularity of place types to return and must be one of: poi, neighborhood, city, admin or country. If no granularity is provided for the request neighborhood is assumed.</param>
        /// <param name="maxResults">A hint as to the number of results to return.</param>
        /// <param name="ipAddress">An IP address. Used when attempting to fix geolocation based off of the user's IP address.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/geo/search </remarks>
        public static async Task<ReverseGeoCodePlaces> GetPlaceIDFromInfo(this IUserSession session, string query="", double latitude = 0.0,
     double longitude = 0.0, string accuracy = "10m", string containedWithin ="", string granularity = "neighborhood", int maxResults = 20, string ipAddress="")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"max_results", maxResults.ToString()}
                             };

            if (!string.IsNullOrWhiteSpace(query))
            {
                parameters.Add("query", query);
            }

            if (!string.IsNullOrWhiteSpace(containedWithin))
            {
                parameters.Add("contained_within", containedWithin);
            }

            if (!string.IsNullOrWhiteSpace(granularity))
            {
                parameters.Add("granularity", granularity);
            }

            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                parameters.Add("ip", ipAddress);
            }

            if (Math.Abs(latitude) > 0.0 && Math.Abs(longitude) > 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());

                if (!string.IsNullOrWhiteSpace(accuracy)) // nested because I think this only matters when lat/long is set
                {
                    parameters.Add("accuracy", accuracy);
                }
            }

            return await session.GetAsync(TwitterApi.Resolve("/1.1/geo/search.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<ReverseGeoCodePlaces>());

        }

        /// <summary>
        /// Locates places near the given coordinates which are similar in name.
        /// NOTE: The token contained in the response is the token needed to be able to create a new place.
        /// </summary>
        /// <param name="name">The name a place is known as.</param>
        /// <param name="latitude">The latitude to search around</param>
        /// <param name="longitude">The longitude to search around</param>
        /// <param name="containedWithin">This is the place_id which you would like to restrict the search results to. Setting this value means only places within the given place_id will be found.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/geo/similar_places </remarks>
        public static async Task<ReverseGeoCodePlaces> GetPlaceSimilarName(this IUserSession session,
            string name = "", double latitude = 0.0,
            double longitude = 0.0, string containedWithin = "")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"lat", latitude.ToString()},
                                 {"long", longitude.ToString()},
                                 {"name",name}
                             };
            if (!string.IsNullOrWhiteSpace(containedWithin))
            {
                parameters.Add("contained_within", containedWithin);
            }

            return await session.GetAsync(TwitterApi.Resolve("/1.1/geo/similar_places.json"), parameters)
                .ContinueWith(c => c.MapToSingle<ReverseGeoCodePlaces>());
        }

        /// <summary>
        /// Creates a new place object at the given latitude and longitude.
        /// </summary>
        /// <param name="name">The name a place is known as.</param>
        /// <param name="latitude">The latitude the place is located at.</param>
        /// <param name="longitude">The longitude the place is located at.</param>
        /// <param name="containedWithin">The place_id within which the new place can be found. Try and be as close as possible with the containing place. For example, for a room in a building, set the contained_within as the building place_id.</param>
        /// <param name="token">The token found in the response from geo/similar_places.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/geo/place </remarks>
        public static async Task<AddPlaceResponse> CreatePlace(this IUserSession session,
            string name, double latitude,
            double longitude, string containedWithin, string token)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"lat", latitude.ToString()},
                                 {"long", longitude.ToString()},
                                 {"name", name},
                                 {"contained_within", containedWithin},
                                 {"token", token}
                             };

            return await session.PostAsync(TwitterApi.Resolve("/1.1/geo/create.json"), parameters)
                .ContinueWith(c => c.MapToSingle<AddPlaceResponse>());
        }
    }
}
