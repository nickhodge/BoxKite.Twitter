// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Stream
{
    public class StreamDisconnect
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("stream_name")]
        public string StreamName { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        //Source: https://dev.twitter.com/docs/streaming-apis/messages#Status_deletion_notices_delete
        public string LongerReason()
        {
            switch (Code)
            {
                case 1:
                    return "Shutdown: The feed was shutdown (possibly a machine restart)";
                case 2:
                    return "Duplicate stream: The same endpoint was connected too many times.";
                case 3:
                    return "Control request: Control streams was used to close a stream (applies to sitestreams).";
                case 4:
                    return "Stall: The client was reading too slowly and was disconnected by the server.";
                case 5:
                    return "Normal: The client appeared to have initiated a disconnect.";
                case 6:
                    return "Token revoked: An oauth token was revoked for a user (applies to site and userstreams).";
                case 7:
                    return "Admin logout: The same credentials were used to connect a new stream and the oldest was disconnected.";
                case 9:
                    return "Max message limit: The stream connected with a negative count parameter and was disconnected after all backfill was delivered.";
                case 10:
                    return "Stream exception: An internal issue disconnected the stream.";
                case 11:
                    return "Broker stall: An internal issue disconnected the stream.";
                case 12:
                    return "Shed load: The host the stream was connected to became overloaded and streams were disconnected to balance load. Reconnect as usual.";
                default:
                    return Reason;
            }
        }
    }
}
