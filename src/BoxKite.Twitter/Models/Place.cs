// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class Place : TwitterControlBase
    {
         [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
         public string Url { get; set; }

       [JsonProperty("place_type")]
        public string PlaceType { get; set; }

        [JsonProperty("name")]
       public string Name { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("bounding_box")]
        public BoundingBox BoundingBox { get; set; }

        [JsonProperty("attributes")]
        public object Attributes { get; set; }
    }
}