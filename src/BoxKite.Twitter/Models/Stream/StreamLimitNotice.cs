using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Stream
{
    public class StreamLimitNotice
    {
        [JsonProperty("track")]
        public int Track { get; set; }
    }
}
