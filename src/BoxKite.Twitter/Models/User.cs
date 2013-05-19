using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models.Service;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class User : TwitterControlBase
    {
        private string name;
        [JsonProperty("name")]
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string avatar;
        [JsonProperty("profile_background_image_url")]
        public string Avatar
        {
            get { return avatar; }
            set { SetProperty(ref avatar, value); }
        }

        private int followers;
        [JsonProperty("followers_count")]
        public int Followers
        {
            get { return followers; }
            set { SetProperty(ref followers, value); }
        }

        private int friends;
        [JsonProperty("friends_count")]
        public int Friends
        {
            get { return friends; }
            set { SetProperty(ref friends, value); }
        }

        private bool isProtected;
        [JsonProperty("protected")]
        public bool IsProtected
        {
            get { return isProtected; }
            set { SetProperty(ref isProtected, value); }
        }

        private bool isFollowedByMe;
        [JsonProperty("following")]
        [JsonConverter(typeof(NullToBoolConverter))]
        public bool IsFollowedByMe
        {
            get { return isFollowedByMe; }
            set
            {
                SetProperty(ref isFollowedByMe, value);
            }
        }

        private string screenName;
        [JsonProperty("screen_name")]
        public string ScreenName
        {
            get { return screenName; }
            set { SetProperty(ref screenName, value); }
        }

        private int userId;
        [JsonProperty("id")]
        public int UserId
        {
            get { return userId; }
            set { SetProperty(ref userId, value); }
        }

    }

}