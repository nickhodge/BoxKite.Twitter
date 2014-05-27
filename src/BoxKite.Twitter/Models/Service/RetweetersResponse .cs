// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class RetweetersResponseIDsCursored : TwitterControlBase
    {
        [JsonProperty("ids")]
        public IEnumerable<long> UserIDs { get; set; }
        public long previous_cursor { get; set; }
        public long next_cursor { get; set; }
    }

}