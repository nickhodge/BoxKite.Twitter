// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class StreamScrubGeo
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("up_to_status_id")]
        public long UpToStatusId { get; set; }
    }

}
