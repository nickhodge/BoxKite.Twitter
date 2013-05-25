// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests
{
    public class TestableSession : IUserSession
    {
        string contents;
        SortedDictionary<string, string> receviedParameters;
        string expectedGetUrl;
        string expectedPostUrl;
        public bool simulatingError { get; set; }
        public HttpStatusCode httpStatusCode { get; set; }

        public Task<HttpResponseMessage> GetAsync(string relativeUrl, SortedDictionary<string, string> parameters)
        {
            if (!string.IsNullOrWhiteSpace(expectedGetUrl))
            {
                Assert.AreEqual(expectedGetUrl, relativeUrl);
            }

            this.receviedParameters = parameters;

            if (simulatingError)
            {
                var response = new HttpResponseMessage(httpStatusCode) {Content = new StringContent(contents)}; //grab the supplied error code in setup
                return Task.FromResult(response);
            }
            else
            {
                var response = new HttpResponseMessage
                               {
                                   StatusCode = HttpStatusCode.OK,
                                   Content = new StringContent(contents)
                               };
                return Task.FromResult(response);
            }
        }

        public Task<HttpResponseMessage> PostAsync(string relativeUrl, SortedDictionary<string, string> parameters)
        {
            if (!string.IsNullOrWhiteSpace(expectedPostUrl))
            {
                Assert.AreEqual(expectedPostUrl, relativeUrl);
            }

            this.receviedParameters = parameters;

            if (simulatingError)
            {
                var response = new HttpResponseMessage(httpStatusCode) {Content = new StringContent(contents)};
                    //grab the supplied error code in setup
                return Task.FromResult(response);
            }
            else
            {
                var response = new HttpResponseMessage
                               {
                                   StatusCode = HttpStatusCode.OK,
                                   Content = new StringContent(contents)
                               };
                return Task.FromResult(response);
            }
        }


        public Task<HttpResponseMessage> PostFileAsync(string url, SortedDictionary<string, string> parameters, string fileName, byte[] fileContents, string fileContentsKey)
        {
            if (!string.IsNullOrWhiteSpace(expectedPostUrl))
            {
                Assert.AreEqual(expectedPostUrl, url);
            }

            this.receviedParameters = parameters;

            var response = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(contents)};
            return Task.FromResult(response);
        }

        public HttpRequestMessage CreateGet(string fullUrl, SortedDictionary<string, string> parameters)
        {
            this.receviedParameters = parameters;
            var req = new HttpRequestMessage { Content = new StringContent(contents) }; //grab the supplied error code in setup
            return req;
        }

        public HttpRequestMessage CreatePost(string fullUrl, SortedDictionary<string, string> parameters)
        {
            this.receviedParameters = parameters;
            var req = new HttpRequestMessage { Content = new StringContent(contents) }; //grab the supplied error code in setup
            return req;
        }

        public async Task<HttpResponseMessage> MakeResponse()
        {
            var resp = new HttpResponseMessage() { Content = new StringContent(contents) }; 
            return resp;
        }

        public void Returns(string contentsReturn)
        {
            this.contents = contentsReturn;
        }

        public bool ReceivedParameter(string key, string value)
        {
            if (!receviedParameters.ContainsKey(key))
                return false;

            var actualValue = receviedParameters[key];
            return actualValue == value;
        }

        public void ExpectGet(string url)
        {
            expectedGetUrl = url;
        }

        public void ExpectPost(string url)
        {
            expectedPostUrl = url;
        }
    }
}