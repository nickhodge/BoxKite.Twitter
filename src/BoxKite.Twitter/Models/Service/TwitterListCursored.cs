// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class TwitterListCursored : TwitterControlBase
    {
        [JsonProperty(PropertyName = "lists")]
        public IEnumerable<TwitterList> twitterlists { get; set; }
        public long previous_cursor { get; set; }
        public long next_cursor { get; set; }
    }
}
