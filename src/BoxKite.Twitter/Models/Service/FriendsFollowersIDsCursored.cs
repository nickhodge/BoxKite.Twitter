using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Service
{
    public class FriendsFollowersIDsCursored : TwitterControlBase
    {
        [JsonProperty(PropertyName = "ids")]
        public IEnumerable<long> IDs { get; set; }
        public string previous_cursor_str { get; set; }
        public string next_cursor_str { get; set; }
    }
}
