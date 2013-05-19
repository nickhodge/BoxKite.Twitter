using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Service
{
    public class UserListDetailedCursored :TwitterControlBase 
    {
        [JsonProperty(PropertyName = "users")]
        public IEnumerable<User> users { get; set; }
        public long previous_cursor { get; set; }
        public long next_cursor { get; set; }
    }
}
