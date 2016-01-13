// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Helpers;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class Tweet : TwitterControlBase
    {
        [JsonProperty("text")]
        public string RawText { get; set; }

        [JsonIgnore]
        public string Text
        {
            get { return RawText.HTMLDecode(); }
        }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
        public DateTimeOffset Time { get; set; }

        [JsonProperty("id_str")]
        public long Id { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("retweet_count")]
         public int? RetweetCount { get; set; }

        [JsonProperty("place")]
        public Place Place { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("in_reply_to_status_id")]
        public long? InReplyToId { get; set; }

        [JsonProperty("in_reply_to_user_id")]
        public long? InReplyToUserId { get; set; }

        [JsonProperty("favorited")]
        public bool Favourited { get; set; }

        [JsonProperty("geo")]
        public GeoCoordinates Location { get; set; }

        [JsonProperty("entities")]
         public Entities Entities { get; set; }

        [JsonProperty("extended_entities")]
        public Entities ExtendedEntities { get; set; }

        [JsonProperty("retweeted_status")]
        public Tweet RetweetedStatus { get; set; }

       [JsonProperty("favorite_count")]
       public int FavoriteCount { get; set; }

        [JsonProperty("truncated")]
        public bool Truncated { get; set; }

        [JsonProperty("retweeted")]
        public bool? Retweeted { get; set; }
    }

    public class Entities
    {
        [JsonProperty("urls")]
        public IEnumerable<Url> Urls { get; set; }

        [JsonProperty("hashtags")]
        public IEnumerable<Hashtag> Hashtags { get; set; }

        [JsonProperty("user_mentions")]
        public IEnumerable<Mention> Mentions { get; set; }

        [JsonProperty("media")]
        public IEnumerable<Media> Media { get; set; }
    }

    public class Variant
    {
        public int Bitrate { get; set; }
        [JsonProperty("content_type")]
        public string ContentType { get; set; }
        public string Url { get; set; }
    }

    public class VideoInfo
    {
        [JsonProperty("aspect_ratio")]
        public List<int> AspectRatio { get; set; }
        [JsonProperty("duration_millis")]
        public int Duration { get; set; }

        public List<Variant> Variants { get; set; }
    }

    public class Url
    {
        [JsonProperty("indices")]
        public IEnumerable<int> indices { get; set; }

        [JsonProperty("url")]
        public string _Url { get; set; }

        [JsonProperty("display_url")]
        public string DisplayUrl { get; set; }

        [JsonProperty("expanded_url")]
        public string ExpandedUrl { get; set; }
    }

    public class Hashtag
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("indices")]
        public IEnumerable<int> indices { get; set; }
    }

    public class Mention
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("indices")]
        public IEnumerable<int> indices { get; set; }

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public class Media
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("sizes")]
        public Sizes Sizes { get; set; }

        [JsonProperty("indices")]
        public IEnumerable<int> indices { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("media_url")]
        public string MediaUrl { get; set; }

        [JsonProperty("display_url")]
        public string DisplayUrl { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("expanded_url")]
        public string ExpandedUrl { get; set; }

        [JsonProperty("media_url_https")]
        public string MediaUrlHttps { get; set; }

        [JsonProperty("video_info")]
        public VideoInfo VideoInfo { get; set; }
    }

    public class Sizes
    {
        [JsonProperty("thumb")]
        public ImageSizeType Thumb { get; set; }

        [JsonProperty("large")]
        public ImageSizeType Large { get; set; }

        [JsonProperty("medium")]
        public ImageSizeType Medium { get; set; }

        [JsonProperty("small")]
        public ImageSizeType Small { get; set; }
    }

    public class ImageSizeType
    {
        [JsonProperty("h")]
        public int Height { get; set; }
        [JsonProperty("resize")]
        public string Resize { get; set; }
        [JsonProperty("w")]
        public int Width { get; set; }
    }

}
