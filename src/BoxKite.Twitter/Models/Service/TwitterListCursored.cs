using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Service
{
    public class TwitterListCursored : TwitterControlBase
    {
        [JsonProperty(PropertyName = "lists")]
        public IEnumerable<TwitterList> twitterlists { get; set; }
        public long previous_cursor { get; set; }
        public long next_cursor { get; set; }
    }
}
