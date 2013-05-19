using System;
using BoxKite.Twitter.Helpers;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Service
{
    public class AccountProfile : TwitterControlBase
    {
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

        [JsonProperty("favourites_count")]
        public int FavouritesCount { get; set; }

        [JsonProperty("follow_request_sent")]
        public bool FollowRequestSent { get; set; }

        [JsonProperty("followers_count")]
        public int FollowersCount { get; set; }

        [JsonProperty("following")]
        public bool Following { get; set; }

        [JsonProperty("friends_count")]
        public int FriendsCount { get; set; }

        [JsonProperty("geo_enabled")]
        public bool GeoEnabled { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("is_translator")]
        public bool IsTranslator { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }

        [JsonProperty("listed_count")]
        public int ListedCount { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("notifications")]
        public bool Notifications { get; set; }

        [JsonProperty("profile_background_color")]
        public string ProfileBackgroundColour { get; set; }

        [JsonProperty("profile_background_image_url")]
        public string ProfilebackgroundImageUrl { get; set; }

        [JsonProperty("profile_background_image_url_https")]
        public string ProfilebackgroundImageUrlHTTPS { get; set; }

        [JsonProperty("profile_background_tile")]
        public bool ProfileBackgroundTile { get; set; }

        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [JsonProperty("profile_image_url_https")]
        public string ProfileImageUrlHTTPS { get; set; }

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

        [JsonProperty("protected")]
        public bool _Protected { get; set; } // protected is a reserved C# word

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        [JsonProperty("show_all_inline_media")]
        public bool ShowAllInlineMedia { get; set; }

        [JsonProperty("statuses_count")]
        public int StatusesCount { get; set; }

        [JsonProperty("time_zone")]
        public string TimeZone { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("utc_offset")]
        public int UTCOffset { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }
    }

}
