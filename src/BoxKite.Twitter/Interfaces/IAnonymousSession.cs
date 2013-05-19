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