// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

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
        /// <param name="place_id">A place in the world.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/geo/id/%3Aplace_id </remarks>
        public static async Task<Place> GetPlaceInfo(this IUserSession session, string place_id)
        {
            var parameters = new SortedDictionary<string, string>();
            var url = Api.Resolve("/1.1/geo/id/{0}.json", place_id);
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
        /// <param name="max_results">A hint as to the number of results to return.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/geo/reverse_geocode </remarks>
        public static async Task<ReverseGeoCodePlaces> GetPlaceIDFromGeocode(this IUserSession session, double latitude = 0.0,
            double longitude = 0.0, string accuracy = "10m", string granularity = "neighborhood", int max_results=20)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"lat", latitude.ToString()},
                                 {"long", longitude.ToString()},
                                 {"accuracy", accuracy},
                                 {"granularity", granularity},
                                 {"max_results", max_results.ToString()}
                             };

            return await session.GetAsync(Api.Resolve("/1.1/geo/reverse_geocode.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<ReverseGeoCodePlaces>());
 
        }

        /// <summary>
        /// Search for places that can be attached to a statuses/update. Given a latitude and a longitude pair, an IP address, or a name, this request will return a list of all the valid places that can be used as the place_id when updating a status
        /// </summary>
        /// <param name="query">Free-form text to match against while executing a geo-based query, best suited for finding nearby locations by name. Remember to URL encode the query.</param>
        /// <param name="latitude">The latitude to search around.</param>
        /// <param name="longitude">The longitude to search around.</param>
        /// <param name="accuracy">A hint on the "region" in which to search. If a number, then this is a radius in meters, but it can also take a string that is suffixed with ft to specify feet. If this is not passed in, then it is assumed to be 0m.</param>
        /// <param name="contained_within">This is the place_id which you would like to restrict the search results to. Setting this value means only places within the given place_id will be found.</param>
        /// <param name="granularity">This is the minimal granularity of place types to return and must be one of: poi, neighborhood, city, admin or country. If no granularity is provided for the request neighborhood is assumed.</param>
        /// <param name="max_results">A hint as to the number of results to return.</param>
        /// <param name="ip_address">An IP address. Used when attempting to fix geolocation based off of the user's IP address.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/geo/search </remarks>
        public static async Task<ReverseGeoCodePlaces> GetPlaceIDFromInfo(this IUserSession session, string query="", double latitude = 0.0,
     double longitude = 0.0, string accuracy = "10m", string contained_within ="", string granularity = "neighborhood", int max_results = 20, string ip_address="")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"max_results", max_results.ToString()}
                             };

            if (!string.IsNullOrWhiteSpace(query))
            {
                parameters.Add("query", query);
            }

            if (!string.IsNullOrWhiteSpace(contained_within))
            {
                parameters.Add("contained_within", contained_within);
            }

            if (!string.IsNullOrWhiteSpace(granularity))
            {
                parameters.Add("granularity", granularity);
            }

            if (!string.IsNullOrWhiteSpace(ip_address))
            {
                parameters.Add("ip", ip_address);
            }

            if (latitude != 0.0 && longitude != 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());

                if (!string.IsNullOrWhiteSpace(accuracy)) // nested because I think this only matters when lat/long is set
                {
                    parameters.Add("accuracy", accuracy);
                }
            }

            return await session.GetAsync(Api.Resolve("/1.1/geo/search.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<ReverseGeoCodePlaces>());

        }

        /// <summary>
        /// Locates places near the given coordinates which are similar in name.
        /// NOTE: The token contained in the response is the token needed to be able to create a new place.
        /// </summary>
        /// <param name="name">The name a place is known as.</param>
        /// <param name="latitude">The latitude to search around</param>
        /// <param name="longitude">The longitude to search around</param>
        /// <param name="contained_within">This is the place_id which you would like to restrict the search results to. Setting this value means only places within the given place_id will be found.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/geo/similar_places </remarks>
        public static async Task<ReverseGeoCodePlaces> GetPlaceSimilarName(this IUserSession session,
            string name = "", double latitude = 0.0,
            double longitude = 0.0, string contained_within = "")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"lat", latitude.ToString()},
                                 {"long", longitude.ToString()},
                                 {"name",name}
                             };
            if (!string.IsNullOrWhiteSpace(contained_within))
            {
                parameters.Add("contained_within", contained_within);
            }

            return await session.GetAsync(Api.Resolve("/1.1/geo/similar_places.json"), parameters)
                .ContinueWith(c => c.MapToSingle<ReverseGeoCodePlaces>());
        }

        /// <summary>
        /// Creates a new place object at the given latitude and longitude.
        /// </summary>
        /// <param name="name">The name a place is known as.</param>
        /// <param name="latitude">The latitude the place is located at.</param>
        /// <param name="longitude">The longitude the place is located at.</param>
        /// <param name="contained_within">The place_id within which the new place can be found. Try and be as close as possible with the containing place. For example, for a room in a building, set the contained_within as the building place_id.</param>
        /// <param name="token">The token found in the response from geo/similar_places.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/geo/place </remarks>
        public static async Task<AddPlaceResponse> CreatePlace(this IUserSession session,
            string name, double latitude,
            double longitude, string contained_within, string token)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"lat", latitude.ToString()},
                                 {"long", longitude.ToString()},
                                 {"name", name},
                                 {"contained_within", contained_within},
                                 {"token", token}
                             };

            return await session.PostAsync(Api.Resolve("/1.1/geo/create.json"), parameters)
                .ContinueWith(c => c.MapToSingle<AddPlaceResponse>());
        }
    }
}
