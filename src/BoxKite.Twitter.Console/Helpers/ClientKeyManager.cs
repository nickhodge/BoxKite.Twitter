// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BoxKite.Twitter.Console
{
    public static class ClientKeyManager
    {
        public static async Task<Tuple<string, string>> GetTwitterClientKeys()
        {
            var vc = new WebClient();
            var rawdata =
                await vc.DownloadStringTaskAsync("http://service.lawrencehargrave.com/1/twitterapikeys/release");
            var j = JObject.Parse(rawdata);
            return new Tuple<string, string>(j["consumerkey"].ToString(), j["consumersecret"].ToString());
        }
    }
}
