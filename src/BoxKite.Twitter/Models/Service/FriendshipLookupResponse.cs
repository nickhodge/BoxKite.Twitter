using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Service
{
    public class FriendshipLookupResponse : TwitterControlBase 
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("connections")]
        public IEnumerable<string> Connections { get; set; }
    }
}
