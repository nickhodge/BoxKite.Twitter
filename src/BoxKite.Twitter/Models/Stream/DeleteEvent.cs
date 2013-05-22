using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Stream
{
    public class DeleteEvent
    {
        [JsonProperty("status")]
        public Status Status { get; set; }
    }

    public class Status
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("user_id")]
        public int UserId { get; set; }
    }

}
