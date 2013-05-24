// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Service
{
    public class UserStatus : TwitterControlBase
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
        public bool Following { get; set; }
    }

    public class UserStatusSource
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("followed_by")]
        public bool FollowedBy { get; set; }

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        [JsonProperty("following")]
        public bool Following { get; set; }

        [JsonProperty("can_dm")]
        public bool CanDM { get; set; }

        [JsonProperty("blocking")]
        public bool Blocking { get; set; }

        [JsonProperty("all_replies")]
        public bool AllReplies { get; set; }

        [JsonProperty("want_retweets")]
        public bool WantRetweets { get; set; }

        [JsonProperty("marked_spam")]
        public bool MarkedSpam { get; set; }

        [JsonProperty("notifications_enabled")]
        public bool NotificationsEnabled { get; set; }
    }
}
