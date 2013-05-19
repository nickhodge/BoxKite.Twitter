using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxKite.Twitter.Models.Service;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Service
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
