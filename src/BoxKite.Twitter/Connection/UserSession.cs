// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using BoxKite.Twitter.Authentication;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public class UserSession : ApplicationSession, IUserSession
    {
        // used http://garyshortblog.wordpress.com/2011/02/11/a-twitter-oauth-example-in-c/

        private TwitterCredentials _credentials;
        public TwitterCredentials TwitterCredentials {
            get
            {
                return _credentials;
            }
            set
            {
                _credentials = value;
            }
        }

        private const string OAuthSignatureMethod = "HMAC-SHA1";
        private const string OAuthVersion = "1.0";

        public IPlatformAdaptor PlatformAdaptor { get; set; }
        public IUserStream UserStream { get; set; }
        public ISearchStream SearchStream { get; set; }

        public UserSession(string clientID, string clientSecret, IPlatformAdaptor platformAdaptor, int waitTimeoutSeconds = 30) 
            : base(clientID, clientSecret, waitTimeoutSeconds)
        {
            this.clientID = clientID;
            this.clientSecret = clientSecret;
            TwitterCredentials = TwitterCredentials.Null;
            PlatformAdaptor = platformAdaptor;
            WaitTimeoutSeconds = waitTimeoutSeconds;
        }

        public UserSession(string clientID, string clientSecret, string bearerToken, IPlatformAdaptor platformAdaptor, int waitTimeoutSeconds = 30)
            : base(clientID, clientSecret, waitTimeoutSeconds)
        {
            this.clientID = clientID;
            this.clientSecret = clientSecret;
            this.bearerToken = bearerToken;
            TwitterCredentials = TwitterCredentials.Null;
            PlatformAdaptor = platformAdaptor;
            WaitTimeoutSeconds = waitTimeoutSeconds;
        }

        public UserSession(TwitterCredentials credentials, IPlatformAdaptor platformAdaptor, int waitTimeoutSeconds = 30)
            : base(credentials.ConsumerKey, credentials.ConsumerSecret, waitTimeoutSeconds)
        {
            TwitterCredentials = credentials;
            clientID = credentials.ConsumerKey;
            clientSecret = credentials.ConsumerSecret;
            bearerToken = credentials.BearerToken;
            PlatformAdaptor = platformAdaptor;
            WaitTimeoutSeconds = waitTimeoutSeconds;
            IsActive = true;
        }

        public IUserStream UserStreamBuilder()
        {
            return UserStream ?? this.GetUserStream(TwitterConnectionEvents);
        }

        /// <summary>
        /// Use OAuth1.0a auth to do more intensive reads
        /// </summary>
        /// <param name="url">URL to call</param>
        /// <param name="parameters">Params to send</param>
        /// <returns></returns>
        public new async Task<HttpResponseMessage> GetAsync(string url, SortedDictionary<string, string> parameters) 
        {
            if (TwitterCredentials == TwitterCredentials.Null || TwitterCredentials.Valid == false)
                throw new ArgumentException("TwitterCredentials must be specified and validated");

            var querystring = parameters.Aggregate("", (current, entry) => current + (entry.Key + "=" + entry.Value + "&"));

            var oauth1aAuthheader = BuildAuthenticatedResult(url, parameters, "GET");
            var fullUrl = url;

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Authorization", oauth1aAuthheader.Header);
            client.DefaultRequestHeaders.Add("User-Agent", TwitterApi.UserAgent());

            if (!string.IsNullOrWhiteSpace(querystring))
                fullUrl += "?" + querystring.Substring(0, querystring.Length - 1);

            var download = client.GetAsync(fullUrl).ToObservable().Timeout(TimeSpan.FromSeconds(WaitTimeoutSeconds));
            var clientdownload = await download;

            return clientdownload;
        }

        /// <summary>
        /// Use OAuth1.0a auth to do more intensive POST
        /// </summary>
        /// <param name="url">URL to call</param>
        /// <param name="parameters">Params to send</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string url, SortedDictionary<string, string> parameters)
        {
            if (TwitterCredentials == TwitterCredentials.Null || TwitterCredentials.Valid == false)
                throw new ArgumentException("TwitterCredentials must be specified and validated");

            var oauth1aAuthheader = BuildAuthenticatedResult(url, parameters, "POST");
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Add("Authorization", oauth1aAuthheader.Header);
            client.DefaultRequestHeaders.Add("User-Agent", TwitterApi.UserAgent());

            var content = parameters.Aggregate(string.Empty, (current, e) => current + string.Format("{0}={1}&", e.Key, Uri.EscapeDataString(e.Value)));
            var data = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");

            var download = client.PostAsync(url, data).ToObservable().Timeout(TimeSpan.FromSeconds(WaitTimeoutSeconds));
            var clientdownload = await download;

            return clientdownload;
        }

        public async Task<HttpResponseMessage> PostFileAsync(string url, SortedDictionary<string, string> parameters, string fileName, string fileContentsKey, byte[] fileContents = null, Stream srImageStream = null)
        {
            if (TwitterCredentials == TwitterCredentials.Null || TwitterCredentials.Valid == false)
                throw new ArgumentException("TwitterCredentials must be specified and validated");

            if (fileContents == null && srImageStream == null)
            {
                throw new ArgumentException("FileContents must have something actually in them.");
            }
            var oauth1aAuthheader = BuildAuthenticatedResult(url, parameters, "POST", multipartform: true);
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.ExpectContinue = false;
            client.DefaultRequestHeaders.Add("Authorization", oauth1aAuthheader.Header);
            client.DefaultRequestHeaders.Add("User-Agent", TwitterApi.UserAgent());

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

            var filedata = FileDataContent(fileContents, srImageStream);
            filedata.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            filedata.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                FileName = "\"" + fileName + "\"",
                Name = "\"" + fileContentsKey + "\"",
            };
            data.Add(filedata);

            var download = client.PostAsync(url, data).ToObservable().Timeout(TimeSpan.FromSeconds(WaitTimeoutSeconds));
            var clientdownload = await download;

            return clientdownload;
        }

        private static ByteArrayContent FileDataContent(byte[] fileData=null, Stream srReader=null)
        {
            if (fileData!=null)
                return new ByteArrayContent(fileData);
            if (srReader == null) return null;
            var fd = srReader.ReadFully();
            return new ByteArrayContent(fd);
        }

        public string GenerateNoonce()
        {
            var rand = new Random();
            return rand.Next(1000000000).ToString();
        }

        public string GenerateTimestamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
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
            request.Headers.Add("User-Agent", TwitterApi.UserAgent());
            return request;
        }

        public HttpRequestMessage CreatePost(string url, SortedDictionary<string, string> parameters)
        {
            var oauth = BuildAuthenticatedResult(url, parameters, "POST");
            var fullUrl = url;

            var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
            request.Headers.Add("Authorization", oauth.Header);
            request.Headers.Add("User-Agent", TwitterApi.UserAgent());

            var content = parameters.Aggregate(string.Empty, (current, e) => current + string.Format("{0}={1}&", e.Key, Uri.EscapeDataString(e.Value)));
            request.Content = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
            
            return request;
        }

        private OAuth BuildAuthenticatedResult(string fullUrl, IEnumerable<KeyValuePair<string, string>> parameters, string method, bool multipartform=false)
        {
            var url = fullUrl;

            var oauthToken = _credentials.Token;
            var oauthConsumerKey = _credentials.ConsumerKey;
            var oauthNonce = GenerateNoonce();

            var oauthTimestamp = GenerateTimestamp();

            //GS - When building the signature string the params
            //must be in alphabetical order. I can't be bothered
            //with that, get SortedDictionary to do it's thing
            var sd = new SortedDictionary<string, string>
                         {
                             {"oauth_consumer_key", oauthConsumerKey},
                             {"oauth_nonce", oauthNonce},
                             {"oauth_signature_method", OAuthSignatureMethod},
                             {"oauth_timestamp", oauthTimestamp},
                             {"oauth_token", oauthToken},
                             {"oauth_version", OAuthVersion}
                         };

            var querystring = "";

            var baseString = method.ToUpper() + "&" + Uri.EscapeDataString(url) + "&";

            if (!multipartform) // with Multi-part form, only the oauth_ headers are used to create the signature
            {
                if (method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                {
                    querystring = parameters.Aggregate(querystring, (current, entry) => current + (entry.Key + "=" + entry.Value + "&"));
                }

                foreach (var entry in parameters)
                    sd.Add(entry.Key, entry.Value);
            }

            foreach (var entry in sd)
            {
                string value;
                if (entry.Key == "status" || entry.Key == "text" || entry.Key == "screen_name" || entry.Key == "user_id" || entry.Key == "track" || entry.Key == "follow" || entry.Key == "locations")
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

            var signingKey = Uri.EscapeDataString(_credentials.ConsumerSecret) + "&" + Uri.EscapeDataString(_credentials.TokenSecret);

            var encoding = Encoding.UTF8;
            PlatformAdaptor.AssignKey(encoding.GetBytes(signingKey));
            var data = Encoding.UTF8.GetBytes(baseString);
            var hash = PlatformAdaptor.ComputeHash(data);
            var signatureString = Convert.ToBase64String(hash);
            return new OAuth
                       {
                           Nonce = oauthNonce,
                           SignatureMethod = OAuthSignatureMethod,
                           Timestamp = oauthTimestamp,
                           ConsumerKey = oauthConsumerKey,
                           Token = oauthToken,
                           SignatureString = signatureString,
                           Version = OAuthVersion,
                           Header = string.Format(
                                        "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", oauth_token=\"{4}\", oauth_signature=\"{5}\", oauth_version=\"{6}\"",
                                        Uri.EscapeDataString(oauthNonce),
                                        Uri.EscapeDataString(OAuthSignatureMethod),
                                        Uri.EscapeDataString(oauthTimestamp),
                                        Uri.EscapeDataString(oauthConsumerKey),
                                        Uri.EscapeDataString(oauthToken),
                                        Uri.EscapeDataString(signatureString),
                                        Uri.EscapeDataString(OAuthVersion))
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