/// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using BoxKite.Twitter.Extensions;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public static class SuggestedUsersExtension
    {
        /// <summary>
        /// Access to Twitter's suggested user list. This returns the list of suggested user categories.
        /// </summary>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/users/suggestions </remarks>
        public static async Task<TwitterResponseCollection<SuggestedUsers>> GetSuggestedLists(this IUserSession session)
        {
            var parameters = new TwitterParametersCollection();
            return await session.GetAsync(Api.Resolve("/1.1/users/suggestions.json"), parameters)
                          .ContinueWith(c => c.MapToMany<SuggestedUsers>());
        }

        /// <summary>
        /// Access the users in a given category of the Twitter suggested user list.
        /// </summary>
        /// <param name="slug">The short name of list or a category returned by GetSuggestedList</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/users/suggestions/%3Aslug </remarks>
        public static async Task<SuggestedUsers> GetSuggestedUsers(this IUserSession session, string slug)
        {
            var parameters = new TwitterParametersCollection();
            var url = Api.Resolve("/1.1/users/suggestions/{0}.json", slug);
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToSingle<SuggestedUsers>());
        }


    }
}
