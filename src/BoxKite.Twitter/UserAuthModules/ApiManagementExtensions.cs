// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using System.Collections.Generic;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
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
        public static async Task<ApiRateStatusResponse> GetCurrentAPIStatus(this ITwitterSession session)
        {
            var parameters = new SortedDictionary<string, string>();
            var url = TwitterApi.Resolve("/1.1/application/rate_limit_status.json");
            var c = session.GetAsync(url, parameters);
            return await c.MapToApiRateLimits();
        }
    }
}
