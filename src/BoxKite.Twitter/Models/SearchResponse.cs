// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class SearchResponse : TwitterControlBase
    {
        [JsonProperty("statuses")]
        public IEnumerable<Tweet> Tweets { get; set; }

        public long max_id { get; set; }
        public long since_id { get; set; }
        public string refresh_url { get; set; }
        public string next_url { get; set; }
        public int count { get; set; }
        public double completed_in { get; set; }
        public SearchMetadata search_metadata { get; set; }
    }


    public class SearchMetadata
    {
        public long max_id { get; set; }
        public long since_id { get; set; }
        public string refresh_url { get; set; }
        public string next_results { get; set; }
        public int count { get; set; }
        public float completed_in { get; set; }
        public string query { get; set; }
    }

}