// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

// https://dev.twitter.com/docs/platform-objects/users

using System;
using BoxKite.Twitter.Helpers;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class User : TwitterControlBase
    {
        private string _name;
        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _avatar;
        [JsonProperty("profile_background_image_url")]
        public string Avatar
        {
            get { return _avatar; }
            set { SetProperty(ref _avatar, value); }
        }

        private int _followers;
        [JsonProperty("followers_count")]
        public int Followers
        {
            get { return _followers; }
            set { SetProperty(ref _followers, value); }
        }

        private int _friends;
        [JsonProperty("friends_count")]
        public int Friends
        {
            get { return _friends; }
            set { SetProperty(ref _friends, value); }
        }

        private bool _isprotected;
        [JsonProperty("protected")]
        public bool IsProtected
        {
            get { return _isprotected; }
            set { SetProperty(ref _isprotected, value); }
        }

        private bool _isfollowedbyme;
        [JsonProperty("following")]
        [JsonConverter(typeof(NullToBoolConverter))]
        public bool IsFollowedByMe
        {
            get { return _isfollowedbyme; }
            set
            {
                SetProperty(ref _isfollowedbyme, value);
            }
        }

        private string _screenname;
        [JsonProperty("screen_name")]
        public string ScreenName
        {
            get { return _screenname; }
            set { SetProperty(ref _screenname, value); }
        }

        private int _userid;
        [JsonProperty("id")]
        public int UserId
        {
            get { return _userid; }
            set { SetProperty(ref _userid, value); }
        }

        [JsonProperty("contributors_enabled")]
        public bool ContributorsEnabled { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("default_profile")]
        public bool DefaultProfile { get; set; }

        [JsonProperty("default_profile_image")]
        public bool DefaultProfileImage { get; set; }

        private string _description;
        [JsonProperty("description")]
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private int _favouritescount;
        [JsonProperty("favourites_count")] // yes, UK English spelling for a JSON field
        public int FavouritesCount
        {
            get { return _favouritescount; }
            set { SetProperty(ref _favouritescount, value); }
        }

        private bool _followrequestsent;
        [JsonProperty("follow_request_sent")]
        [JsonConverter(typeof(NullToBoolConverter))]
        public bool FollowRequestSent
        {
            get { return _followrequestsent; }
            set { SetProperty(ref _followrequestsent, value); }
        }

        [JsonProperty("geo_enabled")]
        public bool GeoEnabled { get; set; }

        [JsonProperty("is_translator")]
        public bool IsTranslator { get; set; }

        [JsonProperty("source")]
        public bool Source { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }

        private int _listedcount;
        [JsonProperty("listed_count")]
        public int ListedCount
        {
            get { return _listedcount; }
            set { SetProperty(ref _listedcount, value); }
        }

        private string _location;
        [JsonProperty("location")]
        public string Location
        {
            get { return _location; }
            set { SetProperty(ref _location, value); }
        }

        [JsonProperty("notifications")]
        [JsonConverter(typeof(NullToBoolConverter))]
        public bool Notifications { get; set; }

        [JsonProperty("profile_background_color")]
        public string ProfileBackgroundColour { get; set; }

        [JsonProperty("profile_background_image_url_https")]
        public string ProfilebackgroundImageUrlHTTPS { get; set; }

        [JsonProperty("profile_background_tile")]
        public bool ProfileBackgroundTile { get; set; }

        private string _profileimageurl;
        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl
        {
            get { return _profileimageurl; }
            set { SetProperty(ref _profileimageurl, value); }
        }

        private string _profileimageurlhttps;
        [JsonProperty("profile_image_url_https")]
        public string ProfileImageUrlHTTPS
        {
            get { return _profileimageurlhttps; }
            set { SetProperty(ref _profileimageurlhttps, value); }
        }

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

        private int _statuscount;
        [JsonProperty("statuses_count")]
        public int StatusesCount
        {
            get { return _statuscount; }
            set { SetProperty(ref _statuscount, value); }
        }

        [JsonProperty("time_zone")]
        public string TimeZone { get; set; }

        private string _url;
        [JsonProperty("url")]
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        [JsonProperty("utc_offset")]
        public int? UTCOffset { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }
    }

}