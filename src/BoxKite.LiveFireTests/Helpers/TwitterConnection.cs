// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Net;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using Newtonsoft.Json.Linq;

namespace BoxKite.Twitter.Console
{
    public static class TwitterConnection
    {
        public static TwitterCredentials GetTwitterCredentials()
        {
            var x = GetTwitterClientKeys().Result;

            var twitterauth = new TwitterAuthenticator(x.Item1, x.Item2);
            var authstartok = twitterauth.StartAuthentication();
            if (authstartok.Result)
            {
                System.Console.Write("pin: ");
                var pin = System.Console.ReadLine();
                return twitterauth.ConfirmPin(pin).Result;
            }
            else
            {
                return TwitterCredentials.Null;
            }
        }

        private static async Task<Tuple<string, string>> GetTwitterClientKeys()
        {
            var vc = new WebClient();
            var rawdata =
                await vc.DownloadStringTaskAsync("http://service.lawrencehargrave.com/1/twitterapikeys/release");
            var j = JObject.Parse(rawdata);
            return new Tuple<string, string>(j["consumerkey"].ToString(), j["consumersecret"].ToString());
        }
    }
}
