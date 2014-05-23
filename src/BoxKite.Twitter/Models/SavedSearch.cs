// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using BoxKite.Twitter.Helpers;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class SavedSearch : TwitterControlBase
    {
        private long _id;
        [JsonProperty("id")]
        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _name;
        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _position;
        [JsonProperty("position")]
        public string Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        private string _query;
        [JsonProperty("query")]
        public string Query
        {
            get { return _query; }
            set { SetProperty(ref _query, value); }
        }

        private DateTimeOffset _datecreated;
        [JsonProperty("created_at")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
        public DateTimeOffset DateCreated
        {
            get { return _datecreated; }
            set { SetProperty(ref _datecreated, value); }
        }

    }
}
