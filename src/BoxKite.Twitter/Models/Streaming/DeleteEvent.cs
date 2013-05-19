using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class DeleteEvent : Event
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
