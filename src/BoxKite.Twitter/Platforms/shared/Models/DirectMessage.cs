// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Helpers;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class DirectMessage : TwitterControlBase
    {
        [JsonProperty("created_at")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
        public DateTimeOffset Time { get; set; }

        [JsonProperty("entities")]
        public Entities Entities { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("recipient")]
        public User Recipient { get; set; }

        [JsonProperty("recipient_id")]
        public long RecipientId { get; set; }

        [JsonProperty("recipient_screen_name")]
        public string RecipientScreenName { get; set; }

         [JsonProperty("sender")]
        public User Sender { get; set; }

        [JsonProperty("sender_id")]
         public long SenderId { get; set; }

        [JsonProperty("sender_screen_name")]
        public string SenderScreenName { get; set; }

        [JsonProperty("text")]
        public string RawText { get; set; }

        [JsonIgnore]
        public string Text
        {
            get { return RawText.HTMLDecode(); }
        }
    }
}