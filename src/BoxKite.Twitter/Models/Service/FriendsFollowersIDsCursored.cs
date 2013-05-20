using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Service
{
    public class FriendsFollowersIDsCursored : TwitterControlBase
    {
        [JsonProperty(PropertyName = "ids")]
        public IEnumerable<long> IDs { get; set; }
        public long previous_cursor { get; set; }
        public long next_cursor { get; set; }
    }
}
