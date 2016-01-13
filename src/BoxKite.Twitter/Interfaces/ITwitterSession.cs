// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BoxKite.Twitter
{
    public interface ITwitterSession
    {
        Task<HttpResponseMessage> GetAsync(string relativeUrl, SortedDictionary<string, string> parameters);

    }
}
