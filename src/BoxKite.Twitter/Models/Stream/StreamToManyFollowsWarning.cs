using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Stream
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
