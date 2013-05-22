using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Stream
{
    public class StreamScrubGeo
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("up_to_status_id")]
        public long UpToStatusId { get; set; }
    }

}
