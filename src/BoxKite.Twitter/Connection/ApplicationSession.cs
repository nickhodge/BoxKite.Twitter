// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using BoxKite.Twitter.Authentication;
using BoxKite.Twitter.Extensions;

namespace BoxKite.Twitter
{
    public class ApplicationSession : IApplicationSession
    {
        public int waitTimeoutSeconds { get; set; }
        public string clientID { get; set; }
        public string clientSecret { get; set; }
        public string bearerToken { get; set; }
        public bool IsActive { get; set; }

        public ApplicationSession(string clientID, string clientSecret, int _waitTimeoutSeconds = 30)
        {
            this.clientID = clientID;
            this.clientSecret = clientSecret;
            waitTimeoutSeconds = _waitTimeoutSeconds;
        }

        public ApplicationSession(string clientID, string clientSecret, string bearerToken, int _waitTimeoutSeconds = 30)
        {
            this.clientID = clientID;
            this.clientSecret = clientSecret;
            this.bearerToken = bearerToken;
            waitTimeoutSeconds = _waitTimeoutSeconds;
        }

        /// <summary>
        /// Use OAuth2 Bearer To do read-only GET query
        /// </summary>
        /// <param name="url">URL to call</param>
        /// <param name="parameters">Params to send</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string url, SortedDictionary<string, string> parameters)
        {
            // ensure we have a bearerToken before progressing
            if (clientID != null && clientSecret != null && bearerToken == null)
            {
                await this.StartApplicationOnlyAuth();
            }
            if (bearerToken == null) return null;

            var querystring = parameters.Aggregate("", (current, entry) => current + (entry.Key + "=" + entry.Value + "&"));

            var oauth2 = String.Format("Bearer {0}", bearerToken);
            var fullUrl = url;

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Authorization", oauth2);
            client.DefaultRequestHeaders.Add("User-Agent", TwitterApi.UserAgent());

            if (!string.IsNullOrWhiteSpace(querystring))
                fullUrl += "?" + querystring.TrimLastChar();

            var download = client.GetAsync(fullUrl).ToObservable().Timeout(TimeSpan.FromSeconds(waitTimeoutSeconds));
            return await download;
       }

        /// <summary>
        /// Use OAuth2 Bearer Token for POST
        /// </summary>
        /// <param name="url">URL to call</param>
        /// <param name="parameters">Params to send</param>
        /// <param name="forInitialAuth">Is for an initial auth to get bearer token</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string url, SortedDictionary<string, string> parameters, bool forInitialAuth = false)
        {
            if (forInitialAuth)
            {
                if (clientID == null && clientSecret == null)
                {
                    return null;
                }
            }
            else
            {
                if (clientID != null && clientSecret != null && bearerToken == null)
                {
                    await this.StartApplicationOnlyAuth();
                }
                if (bearerToken == null) return null;
            }

            var querystring = parameters.Aggregate("", (current, entry) => current + (entry.Key + "=" + entry.Value + "&"));
            if (!string.IsNullOrWhiteSpace(querystring))
                querystring = querystring.TrimLastChar();

            var oauth2bearertoken = "";

            oauth2bearertoken = forInitialAuth
                ? String.Format("Basic {0}",
                    String.Format("{0}:{1}", clientID.UrlEncode(), clientSecret.UrlEncode()).ToBase64String())
                : String.Format("Bearer {0}", bearerToken);

            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            client.DefaultRequestHeaders.Add("Authorization", oauth2bearertoken);
            client.DefaultRequestHeaders.Add("User-Agent", TwitterApi.UserAgent());

            var data = new StringContent(querystring, Encoding.UTF8, "application/x-www-form-urlencoded");

            var download = client.PostAsync(url, data).ToObservable().Timeout(TimeSpan.FromSeconds(waitTimeoutSeconds));
            return await download;
        }
    }
}