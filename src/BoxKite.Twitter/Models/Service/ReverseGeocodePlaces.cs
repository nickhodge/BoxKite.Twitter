// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class ReverseGeoCodePlaces : TwitterControlBase
    {
        public ReverseGeoCodePlacesQuery query { get; set; }
        public Result result { get; set; }
    }

    public class ReverseGeoCodePlacesQuery
    {
        [JsonProperty("params")]
        public ReverseGeoCodePlacesQueryParams _params { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }

    public class ReverseGeoCodePlacesQueryParams
    {
        public int? accuracy { get; set; }
        public Coordinates coordinates { get; set; }
        public string granularity { get; set; }
        public string query { get; set; }
        public string contained_within { get; set; }
    }

    public class Result
    {
        public IEnumerable<Place> places { get; set; }
        public string token { get; set; }
    }

}
