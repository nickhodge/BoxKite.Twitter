using System.Collections.Generic;
using BoxKite.Twitter.Models.Service;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class Ids : TwitterControlBase
    {
            public int previous_cursor { get; set; }

            [JsonProperty("ids")]
            public List<long> ids { get; set; }

            public string previous_cursor_str { get; set; }
            public int next_cursor { get; set; }
            public string next_cursor_str { get; set; } 
    }
}
