// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL


using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BoxKite.Twitter
{
    public interface IAnonymousSession
    {
        Task<HttpResponseMessage> GetAsync(string relativeUrl, SortedDictionary<string, string> parameters);
        Task<HttpResponseMessage> GetAsyncFull(string fullUrl);
    }
}