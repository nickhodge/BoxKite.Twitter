// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{

    public class AccountSettings : TwitterControlBase
    {
        [JsonProperty("always_use_https")]
        public bool AlwaysUseHTTPS { get; set; }

        [JsonProperty("discoverable_by_email")]
        public bool DiscoverableByEmail { get; set; }

        [JsonProperty("geo_enabled")]
        public bool GeoEnabled { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "protected")]
        public bool _Protected { get; set; } // protected is a reserved word in C#

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        [JsonProperty("show_all_inline_media")]
        public bool ShowAllInlineMedia { get; set; }

        [JsonProperty("sleep_time")]
        public SleepTime SleepTime { get; set; }

        [JsonProperty("time_zone")]
        public TimeZone TimeZone { get; set; }

        [JsonProperty("trend_location")]
        public IEnumerable<TrendLocation> TrendLocation { get; set; }

        [JsonProperty("use_cookie_personalization")]
        public bool UseCookiePersonalisation { get; set; }
    }

    public class SleepTime
    {
        public bool enabled { get; set; }
        public int? end_time { get; set; }
        public int? start_time { get; set; }
    }

    public class TimeZone
    {
        public string name { get; set; }
        public string tzinfo_name { get; set; }
        public int? utc_offset { get; set; }
    }

    public class TrendLocation
    {
        public string country { get; set; }
        public string countryCode { get; set; }
        public string name { get; set; }
        public int? parentid { get; set; }
        public PlaceType placeType { get; set; }
        public string url { get; set; }
        public int? woeid { get; set; }
    }


}
