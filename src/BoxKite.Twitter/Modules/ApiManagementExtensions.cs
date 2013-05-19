using System.Collections.Generic;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models.Service;
using System.Threading.Tasks;

namespace BoxKite.Twitter
{
    public static class ApiManagementExtensions
    {
        /// <summary>
        /// Returns the current rate limits for methods belonging to the specified resource families.
        /// </summary>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/application/rate_limit_status </remarks>
        public static async Task<ApiRateLimit> GetCurrentAPIStatus(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>();
            var url = Api.Resolve("/1.1/application/rate_limit_status.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToApiRateLimits());
        }


    }
}
