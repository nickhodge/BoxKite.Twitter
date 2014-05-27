// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class UserStatus : TwitterControlBase
    {
        [JsonProperty("relationship")]
        public UserStatusRelationship Relationship { get; set; }
    }

    public class UserStatusRelationship
    {
        [JsonProperty("target")]
        public UserStatusTarget Target { get; set; }

        [JsonProperty("source")]
        public UserStatusSource Source { get; set; }
    }

    public class UserStatusTarget
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("followed_by")]
        public bool FollowedBy { get; set; }

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        [JsonProperty("following")]
        public bool? Following { get; set; }
    }

    public class UserStatusSource
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("followed_by")]
        public bool? FollowedBy { get; set; }

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        [JsonProperty("following")]
        public bool? Following { get; set; }

        [JsonProperty("can_dm")]
        public bool? CanDM { get; set; }

    }
}
