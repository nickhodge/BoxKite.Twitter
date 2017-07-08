// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class StreamLimitNotice
    {
        [JsonProperty("track")]
        public int Track { get; set; }
    }
}
