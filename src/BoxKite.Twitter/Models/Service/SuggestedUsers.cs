using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Service
{

    public class SuggestedUsers : TwitterControlBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("users")]
        public IEnumerable<User> Users { get; set; }
    }

}
