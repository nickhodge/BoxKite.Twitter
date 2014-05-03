// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
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
        private string _rawtext;
        [JsonProperty("text")]
        public string RawText
        {
            get { return _rawtext; }
            set { SetProperty(ref _rawtext, value); }
        }

        [JsonIgnore]
        public string Text
        {
            get { return _rawtext.HTMLDecode(); }
        }

        private DateTimeOffset _time;
        [JsonProperty("created_at")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
        public DateTimeOffset Time
        {
            get { return _time; }
            set { SetProperty(ref _time, value); }
        }

        [JsonIgnore]
        public string ToFriendlyDateTime
        {
            get { return _time.ToFriendlyText(); }
        }

        private long _id;
        [JsonProperty("id_str")]
        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private User _user;
        [JsonProperty("user")]
        public User User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        private int? _retweetcount;
        [JsonProperty("retweet_count")]
        public int? RetweetCount
        {
            get { return _retweetcount; }
            set { SetProperty(ref _retweetcount, value); }
        }

        private Place _place;
        [JsonProperty("place")]
        public Place Place
        {
            get { return _place; }
            set { SetProperty(ref _place, value); }
        }

        private string _source;
        [JsonProperty("source")]
        public string Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        private long? _inreplytoid;
        [JsonProperty("in_reply_to_status_id")]
        public long? InReplyToId
        {
            get { return _inreplytoid; }
            set { SetProperty(ref _inreplytoid, value); }
        }

        private long? _inreplytouserid;
        [JsonProperty("in_reply_to_user_id")]
        public long? InReplyToUserId
        {
            get { return _inreplytouserid; }
            set { SetProperty(ref _inreplytouserid, value); }
        }

        private bool _favourited;
        [JsonProperty("favorited")]
        public bool Favourited
        {
            get { return _favourited; }
            set { SetProperty(ref _favourited, value); }
        }

        private Coordinates _location;
        [JsonProperty("geo")]
        public Coordinates Location
        {
            get { return _location; }
            set { SetProperty(ref _location, value); }
        }

        private Entities _entities;
        [JsonProperty("entities")]
        public Entities Entities
        {
            get { return _entities; }
            set { SetProperty(ref _entities, value); }
        }


        private Tweet _retweetedStatus;
        [JsonProperty("retweeted_status")]
        public Tweet RetweetedStatus
        {
            get { return _retweetedStatus; }
            set { SetProperty(ref _retweetedStatus, value); }
        }
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

    public class Url
    {
        [JsonProperty("indices")]
        public int[] indices { get; set; }

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
        public int[] indices { get; set; }
    }

    public class Mention
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("indices")]
        public int[] indices { get; set; }

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
        public int[] indices { get; set; }

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
    }

    public class Sizes
    {
        [JsonProperty("thumb")]
        public Thumb Thumb { get; set; }

        [JsonProperty("large")]
        public Large Large { get; set; }

        [JsonProperty("medium")]
        public Medium Medium { get; set; }

        [JsonProperty("small")]
        public Small Small { get; set; }
    }

    public class Thumb
    {
        public int h { get; set; }
        public string resize { get; set; }
        public int w { get; set; }
    }

    public class Large
    {
        public int h { get; set; }
        public string resize { get; set; }
        public int w { get; set; }
    }

    public class Medium
    {
        public int h { get; set; }
        public string resize { get; set; }
        public int w { get; set; }
    }

    public class Small
    {
        public int h { get; set; }
        public string resize { get; set; }
        public int w { get; set; }
    }

    /*public class RetweetedStatus : BindableBase
    {
        private bool _truncated;
        [JsonProperty("truncated")]
        public bool Truncated
        {
            get { return _truncated; }
            set { SetProperty(ref _truncated, value); }
        }

        private bool _retweeted;
        [JsonProperty("retweeted")]
        public bool ReTweeted
        {
            get { return _retweeted; }
            set { SetProperty(ref _retweeted, value); }
        }

        private Coordinates _location;
        [JsonProperty("geo")]
        public Coordinates Location
        {
            get { return _location; }
            set { SetProperty(ref _location, value); }
        }

         private string _inreplytoscreenname;
        [JsonProperty("in_reply_to_screen_name")]
        public string InReplyToScreenName
        {
            get { return _inreplytoscreenname; }
            set { SetProperty(ref _inreplytoscreenname, value); }
        }

        private long? _inreplytoid;
        [JsonProperty("in_reply_to_status_id")]
        public long? InReplyToId
        {
            get { return _inreplytoid; }
            set { SetProperty(ref _inreplytoid, value); }
        }

        [JsonIgnore]
        public object contributors { get; set; }

        private string _source;
        [JsonProperty("source")]
        public string Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        private DateTimeOffset _time;
        [JsonProperty("created_at")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
        public DateTimeOffset Time
        {
            get { return _time; }
            set { SetProperty(ref _time, value); }
        }

        [JsonIgnore]
        public string ToFriendlyDateTime
        {
            get { return _time.ToFriendlyText(); }
        }

        private bool _favourited;
        [JsonProperty("favorited")]
        public bool Favourited
        {
            get { return _favourited; }
            set { SetProperty(ref _favourited, value); }
        }

        private Entities _entities;
        [JsonProperty("entities")]
        public Entities Entities
        {
            get { return _entities; }
            set { SetProperty(ref _entities, value); }
        }

        private User _user;
        [JsonProperty("user")]
        public User User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        private Place _place;
        [JsonProperty("place")]
        public Place Place
        {
            get { return _place; }
            set { SetProperty(ref _place, value); }
        }

        private long _id;
        [JsonProperty("id")]
        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private int? _retweetcount;
        [JsonProperty("retweet_count")]
        public int? RetweetCount
        {
            get { return _retweetcount; }
            set { SetProperty(ref _retweetcount, value); }
        }

        private int? _inreplytouserid;
        [JsonProperty("in_reply_to_user_id")]
        public int? InReplyToUserId
        {
            get { return _inreplytouserid; }
            set { SetProperty(ref _inreplytouserid, value); }
        }

        private string _rawtext;
        [JsonProperty("text")]
        public string RawText
        {
            get { return _rawtext; }
            set { SetProperty(ref _rawtext, value); }
        }

        [JsonIgnore]
        public string Text
        {
            get { return _rawtext.HTMLDecode(); }
        }
    } */

}
