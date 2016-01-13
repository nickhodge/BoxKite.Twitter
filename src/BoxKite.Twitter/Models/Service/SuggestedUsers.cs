// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{

    public class SuggestedUsers : TwitterControlBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("users")]
        public IEnumerable<User> Users { get; set; }
    }

}
