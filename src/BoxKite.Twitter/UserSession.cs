// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public class UserSession : IUserSession
    {
        // used http://garyshortblog.wordpress.com/2011/02/11/a-twitter-oauth-example-in-c/

        readonly TwitterCredentials credentials;

        const string OauthSignatureMethod = "HMAC-SHA1";
        const string OauthVersion = "1.0";
        private IPlatformAdaptor _platformAdaptor;

#if (PORTABLE)
        public UserSession(TwitterCredentials credentials, IPlatformAdaptor platformAdaptor)
        {
            this.credentials = credentials;
            _platformAdaptor = platformAdaptor;
        }
#elif (WINDOWS)
        public UserSession(TwitterCredentials credentials)
        {
            this.credentials = credentials;
            _platformAdaptor = new DesktopPlatformAdaptor();
        }
#endif
        public Task<HttpResponseMessage> GetAsync(string url, SortedDictionary<string, string> parameters)
        {
            var querystring = parameters.Aggregate("", (current, entry) => current + (entry.Key + "=" + entry.Value + "&"));

            var oauth = BuildAuthenticatedResult(url, parameters, "GET");
            var fullUrl = url;

            var client = new HttpClient { MaxResponseContentBufferSize = 10 * 1024 * 1024 };
            client.DefaultRequestHeaders.Add("Authorization", oauth.Header);
            client.DefaultRequestHeaders.Add("User-Agent", "BoxKite.Twitter/1.0");

            if (!string.IsNullOrWhiteSpace(querystring))
                fullUrl += "?" + querystring.Substring(0, querystring.Length - 1);

            return client.GetAsync(fullUrl);
        }

        public Task<HttpResponseMessage> PostAsync(string url, SortedDictionary<string, string> parameters)
        {
            var oauth = BuildAuthenticatedResult(url, parameters, "POST");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", oauth.Header);
            client.DefaultRequestHeaders.Add("User-Agent", "BoxKite.Twitter/1.0");

            var content = parameters.Aggregate(string.Empty, (current, e) => current + string.Format("{0}={1}&", e.Key, Uri.EscapeDataString(e.Value)));
            var data = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");

            return client.PostAsync(url, data);
        }

        public Task<HttpResponseMessage> PostFileAsync(string url, SortedDictionary<string, string> parameters,
            string fileName, byte[] fileContents, string fileContentsKey)
        {
            var oauth = BuildAuthenticatedResult(url, parameters, "POST", multipartform: true);
            var client = new HttpClient();
            client.DefaultRequestHeaders.ExpectContinue = false;
            client.DefaultRequestHeaders.Add("Authorization", oauth.Header);
            client.DefaultRequestHeaders.Add("User-Agent", "BoxKite.Twitter/1.0");

            var data = new MultipartFormDataContent();
            if (parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    var statusData = new StringContent(parameter.Value);
                    statusData.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                                            {
                                                                Name = "\"" + parameter.Key + "\""
                                                            };
                    data.Add(statusData);
                }
            }

            var filedata = new ByteArrayContent(fileContents);
            filedata.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            filedata.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                                  {
                                                      FileName = "\"" + fileName + "\"",
                                                      Name = "\"" + fileContentsKey + "\"",
                                                  };
            data.Add(filedata);

            return client.PostAsync(url, data);
        }

        public HttpRequestMessage CreateGet(string url, SortedDictionary<string, string> parameters)
        {
            var querystring = parameters.Aggregate("", (current, entry) => current + (entry.Key + "=" + entry.Value + "&"));
            var oauth = BuildAuthenticatedResult(url, parameters, "GET");
            var fullUrl = url;

            if (!string.IsNullOrWhiteSpace(querystring))
                fullUrl += "?" + querystring.Substring(0, querystring.Length - 1);

            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.Add("Authorization", oauth.Header);
            request.Headers.Add("User-Agent", "BoxKite.Twitter/1.0");
            return request;
        }

        public HttpRequestMessage CreatePost(string url, SortedDictionary<string, string> parameters)
        {
            var oauth = BuildAuthenticatedResult(url, parameters, "POST");
            var fullUrl = url;

            var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
            request.Headers.Add("Authorization", oauth.Header);
            request.Headers.Add("User-Agent", "BoxKite.Twitter/1.0");
 
            var content = parameters.Aggregate(string.Empty, (current, e) => current + string.Format("{0}={1}&", e.Key, Uri.EscapeDataString(e.Value)));
            request.Content = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
            
            return request;
        }

        private OAuth BuildAuthenticatedResult(string fullUrl, IEnumerable<KeyValuePair<string, string>> parameters, string method, bool multipartform=false)
        {
            var url = fullUrl;

            var oauthToken = credentials.Token;
            var oauthConsumerKey = credentials.ConsumerKey;
            var rand = new Random();
            var oauthNonce = rand.Next(1000000000).ToString();

            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var oauthTimestamp = Convert.ToInt64(ts.TotalSeconds).ToString();

            //GS - When building the signature string the params
            //must be in alphabetical order. I can't be bothered
            //with that, get SortedDictionary to do it's thing
            var sd = new SortedDictionary<string, string>
                         {
                             {"oauth_consumer_key", oauthConsumerKey},
                             {"oauth_nonce", oauthNonce},
                             {"oauth_signature_method", OauthSignatureMethod},
                             {"oauth_timestamp", oauthTimestamp},
                             {"oauth_token", oauthToken},
                             {"oauth_version", OauthVersion}
                         };

            var querystring = "";

            var baseString = method.ToUpper() + "&" + Uri.EscapeDataString(url) + "&";

            if (!multipartform) // with Multi-part form, only the oauth_ headers are used to create the signature
            {
                if (method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var entry in parameters)
                    {
                        querystring += entry.Key + "=" + entry.Value + "&";
                    }
                }

                foreach (var entry in parameters)
                    sd.Add(entry.Key, entry.Value);
            }

            foreach (var entry in sd)
            {
                string value;
                if (entry.Key == "status" || entry.Key == "text" || entry.Key == "screen_name" || entry.Key == "user_id")
                {
                    value = Uri.EscapeDataString(entry.Value);
                }
                else
                {
                   value = entry.Value;
                }

                baseString += Uri.EscapeDataString(entry.Key + "=" + value + "&");
            }

            baseString = baseString.Substring(0, baseString.Length - 3);

            var signingKey = Uri.EscapeDataString(credentials.ConsumerSecret) + "&" + Uri.EscapeDataString(credentials.TokenSecret);

            var encoding = Encoding.UTF8;
            _platformAdaptor.AssignKey(encoding.GetBytes(signingKey));
            var data = Encoding.UTF8.GetBytes(baseString);
            var hash = _platformAdaptor.ComputeHash(data);
            var signatureString = Convert.ToBase64String(hash);
            return new OAuth
                       {
                           Nonce = oauthNonce,
                           SignatureMethod = OauthSignatureMethod,
                           Timestamp = oauthTimestamp,
                           ConsumerKey = oauthConsumerKey,
                           Token = oauthToken,
                           SignatureString = signatureString,
                           Version = OauthVersion,
                           Header = string.Format(
                                        "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", oauth_token=\"{4}\", oauth_signature=\"{5}\", oauth_version=\"{6}\"",
                                        Uri.EscapeDataString(oauthNonce),
                                        Uri.EscapeDataString(OauthSignatureMethod),
                                        Uri.EscapeDataString(oauthTimestamp),
                                        Uri.EscapeDataString(oauthConsumerKey),
                                        Uri.EscapeDataString(oauthToken),
                                        Uri.EscapeDataString(signatureString),
                                        Uri.EscapeDataString(OauthVersion))
                       };
        }

        private struct OAuth
        {
            public string Nonce;
            public string SignatureMethod;
            public string Timestamp;
            public string ConsumerKey;
            public string Token;
            public string SignatureString;
            public string Version;
            public string Header;
        }
    }
}