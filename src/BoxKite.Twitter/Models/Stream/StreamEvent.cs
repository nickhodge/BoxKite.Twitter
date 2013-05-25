// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster

using System;
using BoxKite.Twitter.Helpers;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class TweetStreamEvent : StreamEvent
    {
        [JsonProperty("target_object")]
        public Tweet tweet { get; set; }
    }

    public class ListStreamEvent : StreamEvent
    {
        [JsonProperty("target_object")]
        public TwitterList list { get; set; }
    }

    public class StreamEvent
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
