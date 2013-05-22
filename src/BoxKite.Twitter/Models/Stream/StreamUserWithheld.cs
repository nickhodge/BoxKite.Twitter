using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Stream
{
    public class StreamUserWithheld
    {
        [JsonProperty("id")]
        public int UserId { get; set; }

        [JsonProperty("withheld_in_countries")]
        public string[] WithheldInCountries { get; set; }
    }
}
