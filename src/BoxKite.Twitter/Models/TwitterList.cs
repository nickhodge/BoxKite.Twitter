// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using BoxKite.Twitter.Helpers;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class TwitterList : TwitterControlBase
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

         [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
         public DateTimeOffset CreatedTime { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("subscriber_count")]
        public int SubscriberCount { get; set; }

         [JsonProperty("member_count")]
        public int MemberCount { get; set; }

        [JsonProperty("mode")]
         public string Mode { get; set; }

        // changed from int to long as per https://dev.twitter.com/blog/list-ids-become-64-bit-integers-early-2014
         [JsonProperty("id_str")]
        public long Id { get; set; }

         [JsonProperty("full_name")]
         public string FullName { get; set; }

        [JsonProperty("description")]
         public string Description { get; set; }

         [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("following")]
         public bool Following { get; set; }
    }
}
