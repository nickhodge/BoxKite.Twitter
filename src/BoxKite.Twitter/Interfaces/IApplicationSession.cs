// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public interface IApplicationSession
    {
        string clientID { get; set; }
        string clientSecret { get; set; }
        string bearerToken { get; set; }
        int WaitTimeoutSeconds { get; set; }

        Task<HttpResponseMessage> GetAsync(string relativeUrl, SortedDictionary<string, string> parameters);
 
     }
}
