// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class Place : TwitterControlBase
    {
        private string _id;
        [JsonProperty("id")]
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _url;
        [JsonProperty("url")]
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        private string _placetype;
        [JsonProperty("place_type")]
        public string PlaceType
        {
            get { return _placetype; }
            set { SetProperty(ref _placetype, value); }
        }

        private string _name;
        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _fullname;
        [JsonProperty("full_name")]
        public string FullName
        {
            get { return _fullname; }
            set { SetProperty(ref _fullname, value); }
        }

        private string _countrycode;
        [JsonProperty("country_code")]
        public string CountryCode
        {
            get { return _countrycode; }
            set { SetProperty(ref _countrycode, value); }
        }

        private string _country;
        [JsonProperty("country")]
        public string Country
        {
            get { return _country; }
            set { SetProperty(ref _country, value); }
        }

        private BoundingBox _boundingbox;
        [JsonProperty("bounding_box")]
        public BoundingBox BoundingBox
        {
            get { return _boundingbox; }
            set { SetProperty(ref _boundingbox, value); }
        }

        [JsonProperty("attributes")]
        public object Attributes { get; set; }
    }
}