// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class StreamUserWithheld
    {
        [JsonProperty("id")]
        public int UserId { get; set; }

        [JsonProperty("withheld_in_countries")]
        public string[] WithheldInCountries { get; set; }
    }
}
