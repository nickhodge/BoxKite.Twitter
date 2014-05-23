// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
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
