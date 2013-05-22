using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Stream
{
    public class StreamStatusWithheld
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("withheld_in_countries")]
        public string[] WithheldInCountries { get; set; }
    }
}
