// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster

using System;
using BoxKite.Twitter.Helpers;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class TweetStreamEvent : StreamEvent
    {
        [JsonProperty("target_object")]
        public Tweet Tweet { get; set; }
    }

    public class BlockStreamEvent : StreamEvent
    {
        [JsonProperty("target_object")]
        public User User { get; set; }
    }

    public class UnblockStreamEvent : StreamEvent
    {
        [JsonProperty("target_object")]
        public User User { get; set; }
    }

    public class FavoriteStreamEvent : StreamEvent
    {
        [JsonProperty("target_object")]
        public User User { get; set; }
    }

    public class UnfavoriteStreamEvent : StreamEvent
    {
        [JsonProperty("target_object")]
        public User User { get; set; }
    }

    public class FollowStreamEvent : StreamEvent
    {
        [JsonProperty("target_object")]
        public User User { get; set; }
    }

    public class UnfollowStreamEvent : StreamEvent
    {
        [JsonProperty("target_object")]
        public User User { get; set; }
    }

    public class ListStreamEvent : StreamEvent
    {
        [JsonProperty("target_object")]
        public TwitterList List { get; set; }
    }

    public class StreamEvent : IStreamEvent
    {
        [JsonProperty("event")]
        public string EventName { get; set; }

        [JsonProperty("target")]
        public User TargetUser { get; set; }

        [JsonProperty("source")]
        public User SourceUser { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
