using System.Collections.Generic;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models.Service;
using System.Threading.Tasks;

namespace BoxKite.Twitter
{
    public static class SuggestedUsersExtension
    {
        /// <summary>
        /// Access to Twitter's suggested user list. This returns the list of suggested user categories.
        /// </summary>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/users/suggestions </remarks>
        public static async Task<IEnumerable<SuggestedUsers>> GetSuggestedLists(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>();
            var url = Api.Resolve("/1.1/users/suggestions.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToSuggestions());
        }

        /// <summary>
        /// Access the users in a given category of the Twitter suggested user list.
        /// </summary>
        /// <param name="slug">The short name of list or a category returned by GetSuggestedList</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/users/suggestions/%3Aslug </remarks>
        public static async Task<SuggestedUsers> GetSuggestedUsers(this IUserSession session, string slug)
        {
            var parameters = new SortedDictionary<string, string>();
            var url = Api.Resolve("/1.1/users/suggestions/{0}.json", slug);
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToUserSuggestions());
        }


    }
}
