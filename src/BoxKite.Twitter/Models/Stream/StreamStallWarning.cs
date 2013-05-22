using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Stream
{
    public class StreamStallWarning
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("percent_full")]
        public int Percent { get; set; }

    }
}
