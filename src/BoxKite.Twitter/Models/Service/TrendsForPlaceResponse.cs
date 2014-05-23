// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using System;
using System.Collections.Generic;

namespace BoxKite.Twitter.Models
{
    public class TrendsForPlaceResponse : TwitterControlBase
    {
        public DateTime as_of { get; set; }
        public DateTime created_at { get; set; }
        public IEnumerable<Location> locations { get; set; }
        public IEnumerable<Trend> trends { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
        public int woeid { get; set; }
    }

    public class Trend
    {
        public string events { get; set; }
        public string name { get; set; }
        public bool? promoted_content { get; set; }
        public string query { get; set; }
        public string url { get; set; }
    }
}
