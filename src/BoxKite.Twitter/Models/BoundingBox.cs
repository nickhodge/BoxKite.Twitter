using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class BoundingBox
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<List<List<double>>> Coordinates { get; set; }
    }
}