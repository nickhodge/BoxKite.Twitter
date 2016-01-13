// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class TrendsAvailableLocationsResponse : TwitterControlBase
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("countrycode")]
        public string CountryCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parentid")]
        public int ParentId { get; set; }

        [JsonProperty("placetype")]
        public PlaceType PlaceType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("woeid")]
        public int WOEID { get; set; }
    }

    public class PlaceType
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

}
