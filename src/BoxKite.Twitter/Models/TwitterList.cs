using System;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models.Service;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class TwitterList : TwitterControlBase
    {
        private string _slug;
        [JsonProperty("slug")]
        public string Slug
        {
            get { return _slug; }
            set { SetProperty(ref _slug, value); }
        }

        private string _name;
        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private DateTimeOffset _createdtime;
        [JsonProperty("created_at")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
        public DateTimeOffset CreatedTime
        {
            get { return _createdtime; }
            set { SetProperty(ref _createdtime, value); }
        }

        private string _uri;
        [JsonProperty("uri")]
        public string Uri
        {
            get { return _uri; }
            set { SetProperty(ref _uri, value); }
        }

        private int _subscribercount;
        [JsonProperty("subscriber_count")]
        public int SubscriberCount
        {
            get { return _subscribercount; }
            set { SetProperty(ref _subscribercount, value); }
        }

        private int _membercount;
        [JsonProperty("member_count")]
        public int MemberCount
        {
            get { return _membercount; }
            set { SetProperty(ref _membercount, value); }
        }

        private string _mode;
        [JsonProperty("mode")]
        public string Mode
        {
            get { return _mode; }
            set { SetProperty(ref _mode, value); }
        }

        private int _id;
        [JsonProperty("id")]
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _fullname;
        [JsonProperty("full_name")]
        public string FullName
        {
            get { return _fullname; }
            set { SetProperty(ref _fullname, value); }
        }

        private string _description;
        [JsonProperty("description")]
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private User _user;
        [JsonProperty("user")]
        public User User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        private bool _following;
        [JsonProperty("following")]
        public bool Following
        {
            get { return _following; }
            set { SetProperty(ref _following, value); }
        }
    }
}
