using System;
using BoxKite.Twitter.Extensions;

namespace BoxKite.Twitter.Models.Service
{
    public class ApiRateLimit : TwitterControlBase
    {
        public Help help { get; set; }
        public Statuses statuses { get; set; }
        public Users users { get; set; }
        public Search search { get; set; }
    }

    public class ApiRateStatus
    {
        public int remaining { get; set; }
        public int limit { get; set; }
        public double reset { get; set; }
        public DateTime ResetTime
        {
            get { return reset.FromUnixEpochSeconds(); }
        }
    }

    public class Help
    {
        public ApiRateStatus helpprivacy { get; set; }
        public ApiRateStatus helpconfiguration { get; set; }
        public ApiRateStatus helptos { get; set; }
        public ApiRateStatus helplanguages { get; set; }
    }

    public class Statuses
    {
        public ApiRateStatus statusesoembed { get; set; }
        public ApiRateStatus statusesuser_timeline { get; set; }
        public ApiRateStatus statusesmentions_timeline { get; set; }
        public ApiRateStatus statuseshome_timeline { get; set; }
        public ApiRateStatus statusesshowid { get; set; }
        public ApiRateStatus statusesretweetsid { get; set; }
    }

    public class Users
    {
        public ApiRateStatus usersshow { get; set; }
        public ApiRateStatus userssearch { get; set; }
        public ApiRateStatus userssuggestions { get; set; }
        public ApiRateStatus userscontributors { get; set; }
        public ApiRateStatus userssuggestionsslugmembers { get; set; }
        public ApiRateStatus userssuggestionsslug { get; set; }
        public ApiRateStatus userscontributees { get; set; }
        public ApiRateStatus userslookup { get; set; }
    }

    public class Search
    {
        public ApiRateStatus searchtweets { get; set; }
    }

}