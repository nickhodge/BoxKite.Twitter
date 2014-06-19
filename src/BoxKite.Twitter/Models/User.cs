// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

// https://dev.twitter.com/docs/platform-objects/users

using System;
using BoxKite.Twitter.Helpers;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class User : TwitterControlBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

         [JsonProperty("profile_image_url")]
        public string Avatar { get; set; }

         [JsonProperty("followers_count")]
         public int Followers { get; set; }

        [JsonProperty("friends_count")]
         public int Friends { get; set; }

         [JsonProperty("protected")]
        public bool IsProtected { get; set; }

        [JsonProperty("following")]
        [JsonConverter(typeof(NullToBoolConverter))]
         public bool IsFollowedByMe { get; set; }

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        [JsonProperty("id_str")]
        public long UserId { get; set; }

        [JsonProperty("contributors_enabled")]
        public bool ContributorsEnabled { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("default_profile")]
        public bool DefaultProfile { get; set; }

        [JsonProperty("default_profile_image")]
        public bool DefaultProfileImage { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

         [JsonProperty("favourites_count")] // yes, UK English spelling for a JSON field
        public int FavouritesCount { get; set; }

        [JsonProperty("follow_request_sent")]
        [JsonConverter(typeof(NullToBoolConverter))]
         public bool FollowRequestSent { get; set; }

        [JsonProperty("geo_enabled")]
        public bool GeoEnabled { get; set; }

        [JsonProperty("is_translator")]
        public bool IsTranslator { get; set; }

        [JsonProperty("source")]
        public bool Source { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }

        [JsonProperty("listed_count")]
        public int ListedCount { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("notifications")]
        [JsonConverter(typeof(NullToBoolConverter))]
        public bool Notifications { get; set; }

        [JsonProperty("profile_background_color")]
        public string ProfileBackgroundColour { get; set; }

        [JsonProperty("profile_background_image_url_https")]
        public string ProfileBackgroundImageUrlHttps { get; set; }

        [JsonIgnore]
        public string ProfileBackgroundImageUrl { get { return ProfileBackgroundImageUrlHttps; } }

        [JsonProperty("profile_banner_url")]
        public string ProfileBannerImageUrlHttps { get; set; }

        [JsonIgnore]
        public string ProfileBannerImageUrl { get { return ProfileBannerImageUrlHttps; } }

        [JsonProperty("profile_background_tile")]
        public bool ProfileBackgroundTile { get; set; }

         [JsonProperty("profile_image_url_https")]
        public string ProfileImageUrlHttps { get; set; }

        [JsonProperty("profile_link_color")]
        public string ProfileLinkColour { get; set; }

        [JsonProperty("profile_sidebar_border_color")]
        public string ProfileSidebarBorderColour { get; set; }

        [JsonProperty("profile_sidebar_fill_color")]
        public string ProfileSidebarFillColour { get; set; }

        [JsonProperty("profile_text_color")]
        public string ProfileTextColour { get; set; }

        [JsonProperty("profile_use_background_image")]
        public bool ProfileUseBackgroundImage { get; set; }

        [JsonProperty("show_all_inline_media")]
        public bool ShowAllInlineMedia { get; set; }

        [JsonProperty("statuses_count")]
        public int StatusesCount { get; set; }

        [JsonProperty("time_zone")]
        public string TimeZone { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("utc_offset")]
        public int? UTCOffset { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }
    }

}