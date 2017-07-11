// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class DeleteEvent
    {
        [JsonProperty("status")]
        public DeleteEventStatus DeleteEventStatus { get; set; }
    }

    public class DeleteEventStatus
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("user_id")]
        public int UserId { get; set; }
    }

}
