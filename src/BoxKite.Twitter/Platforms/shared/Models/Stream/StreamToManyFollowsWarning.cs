// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class StreamToManyFollowsWarning
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }
    }
}
