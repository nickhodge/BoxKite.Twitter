using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Models.Service;

namespace BoxKite.Twitter
{
    public static class FriendsFollowersExtensions
    {
        /// <summary>
        /// Returns a cursored collection of user IDs for every user the specified user is following (otherwise known as their "friends")
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <param name="user_id">screen_name or user_id must be provided</param>
        /// <param name="screen_name">screen_name or user_id must be provided</param>
        /// <param name="count">how many to return default 500</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/friends/ids </remarks>
        public async static Task<FriendsFollowersIDsCursored> GetFriendsIDs(this IUserSession session, long cursor = -1, int user_id = 0, string screen_name = "", int count = 500)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                    {"count",count.ToString()},
                                    { "cursor", cursor.ToString() },
                                 };

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            return await session.GetAsync(Api.Resolve("/1.1/friends/ids.json"), parameters)
                             .ContinueWith(t => t.MapToFriendsFollowersIDsCursored());
        }

        /// <summary>
        /// Returns a cursored collection of user IDs following a particular user(otherwise known as their "followers")
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <param name="user_id">screen_name or user_id must be provided</param>
        /// <param name="screen_name">screen_name or user_id must be provided</param>
        /// <param name="count">how many to return default 500</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/followers/ids </remarks>
        public static async Task<FriendsFollowersIDsCursored> GetFollowersIDs(this IUserSession session, long cursor = -1, int user_id = 0, string screen_name = "", int count = 500)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                    {"count",count.ToString()},
                                    { "cursor", cursor.ToString() },
                                 };

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            return await session.GetAsync(Api.Resolve("/1.1/followers/ids.json"), parameters)
                             .ContinueWith(t => t.MapToFriendsFollowersIDsCursored());
        }


        /// <summary>
        /// Returns the relationships of the authenticating user to the comma-separated list of up to 100 screen_names or user_ids provided. Values for connections can be: following, following_requested, followed_by, none.
        /// </summary>
        /// <param name="screen_names">list of screen_names to check</param>
        /// <param name="user_ids">list of user_ids to check against</param>
        /// <returns></returns>
        /// <remarks> ref : https://dev.twitter.com/docs/api/1.1/get/friendships/lookup </remarks>
        public async static Task<IEnumerable<FriendshipLookupResponse>> GetFriendships(this IUserSession session, string[] screen_names, string[] user_ids)
        {
            var parameters = new SortedDictionary<string, string>();
            var screenNameList = new StringBuilder();
            if (screen_names.HasAny())
            {
                foreach (var screenName in screen_names)
                {
                    screenNameList.Append(screenName + ",");
                }
                parameters.Add("screen_name", screenNameList.ToString().TrimLastChar());
            }

            var userIDList = new StringBuilder();
            if (user_ids.HasAny())
            {
                foreach (var userID in user_ids)
                {
                    userIDList.Append(userID + ",");
                }
                parameters.Add("user_id", userIDList.ToString().TrimLastChar());
            }

            var url = Api.Resolve("/1.1/friendships/lookup.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(f => f.MapToListOfFriendships());
        }

        /// <summary>
        /// Returns a collection of numeric IDs for every user who has a pending request to follow the authenticating user.
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/friendships/incoming </remarks>
        public async static Task<FriendsFollowersIDsCursored> GetFriendshipRequestsIncoming(this IUserSession session, long cursor = -1)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                    { "cursor", cursor.ToString() },
                                 };

            return await session.GetAsync(Api.Resolve("/1.1/friendships/incoming.json"), parameters)
                             .ContinueWith(t => t.MapToFriendsFollowersIDsCursored());
        }

        /// <summary>
        /// Returns a collection of numeric IDs for every protected user for whom the authenticating user has a pending follow request.
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/friendships/outgoing </remarks>
        public async static Task<FriendsFollowersIDsCursored> GetFriendshipRequestsOutgoing(this IUserSession session, long cursor = -1)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                    { "cursor", cursor.ToString() },
                                 };

            return await session.GetAsync(Api.Resolve("/1.1/friendships/outgoing.json"), parameters)
                             .ContinueWith(t => t.MapToFriendsFollowersIDsCursored());
        }

        /// <summary>
        /// Allows the authenticating users to follow the user specified.
        /// </summary>
        /// <param name="screen_name">The screen name of the user for whom to befriend.</param>
        /// <param name="user_id">The ID of the user for whom to befriend.</param>
        /// <param name="follow">Enable notifications for the target user.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/friendships/create </remarks>
        public async static Task<User> CreateFriendship(this IUserSession session, string screen_name = "",
            int user_id = 0, bool follow=true)
        {
            var createFriendship = Api.Resolve("/1.1/friendships/create.json");
            var parameters = new SortedDictionary<string, string>
                             {
                                 {"follow", follow.ToString()},
                             };

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            return await session.PostAsync(createFriendship, parameters)
                          .ContinueWith(c => c.MapToSingleUser());
        }

        /// <summary>
        /// Allows the authenticating users to follow the user specified.
        /// </summary>
        /// <param name="screen_name">The screen name of the user for whom to un-befriend.</param>
        /// <param name="user_id">The ID of the user for whom to un-befriend.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/friendships/destroy </remarks>
        public async static Task<User> DeleteFriendship(this IUserSession session, string screen_name = "",
            int user_id = 0)
        {
            var destroyFriendship = Api.Resolve("/1.1/post/friendships/destroy.json");
            var parameters = new SortedDictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            return await session.PostAsync(destroyFriendship, parameters)
                          .ContinueWith(c => c.MapToSingleUser());
        }

        /// <summary>
        /// Allows one to enable or disable retweets and device notifications from the specified user.
        /// </summary>
        /// <param name="screen_name">The screen name of the user</param>
        /// <param name="user_id">The ID of the user</param>
        /// <param name="device">Enable/disable device notifications from the target user.</param>
        /// <param name="retweets">Enable/disable retweets from the target user.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/friendships/update </remarks>
        public async static Task<UserStatus> ChangeFriendship(this IUserSession session, string screen_name = "",
            int user_id = 0, bool device=false, bool retweets=false)
        {
            var updateFriendship = Api.Resolve("/1.1/friendships/update.json");
            var parameters = new SortedDictionary<string, string>
                             {
                                 {"device",device.ToString()},
                                 {"retweets",retweets.ToString()}
                             };

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            return await session.PostAsync(updateFriendship, parameters)
                          .ContinueWith(c => c.MapToUserStatus());
        }

        /// <summary>
        /// Returns detailed information about the relationship between two arbitrary users.
        /// </summary>
        /// <param name="source_screen_name">The user_id of the subject user.</param>
        /// <param name="source_id">The screen_name of the subject user.</param>
        /// <param name="target_id">The user_id of the target user.</param>
        /// <param name="target_screen_name">The screen_name of the target user.</param>
        /// <returns></returns>
        /// <remarks> ref: https://api.twitter.com/1.1/friendships/show.json </remarks>
        public async static Task<UserStatus> GetFriendship(this IUserSession session, string source_screen_name="",string target_screen_name="", string source_id="",string target_id="")
        {
            var getFriendship = Api.Resolve("/1.1/friendships/show.json");
            var parameters = new SortedDictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(source_screen_name))
            {
                parameters.Add("source_screen_name", source_screen_name);
            }

            if (!string.IsNullOrWhiteSpace(source_id))
            {
                parameters.Add("source_id", source_id);
            }

            if (!string.IsNullOrWhiteSpace(target_screen_name))
            {
                parameters.Add("target_screen_name", target_screen_name);
            }

            if (!string.IsNullOrWhiteSpace(target_id))
            {
                parameters.Add("target_id", target_id);
            }
            return await session.PostAsync(getFriendship, parameters)
                          .ContinueWith(c => c.MapToUserStatus());
        }

        /// <summary>
        /// Returns a cursored collection of user objects for every user the specified user is following (otherwise known as their "friends").
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <param name="user_id">screen_name or user_id must be provided</param>
        /// <param name="screen_name">screen_name or user_id must be provided</param>
        /// <param name="count">how many to return default 500</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/friends/list </remarks>
        public async static Task<UserListDetailedCursored> GetFriendsList(this IUserSession session, long cursor = -1, int user_id = 0, string screen_name = "", int count = 20)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                    {"count",count.ToString()},
                                    { "cursor", cursor.ToString() },
                                    //TODO: add these as params
                                    {"skip_status", true.ToString()},
                                    {"include_user_entities", true.ToString()},
                                 };

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            return await session.GetAsync(Api.Resolve("/1.1/friends/list.json"), parameters)
                             .ContinueWith(t => t.MapToUserListDetailedCursored());
        }

        /// <summary>
        /// Returns a cursored collection of user objects for users following the specified user.
        /// Presently in most recent following first
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <param name="user_id">screen_name or user_id must be provided</param>
        /// <param name="screen_name">screen_name or user_id must be provided</param>
        /// <param name="count">how many to return default 500</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/followers/list </remarks>
        public async static Task<UserListDetailedCursored> GetFollowersList(this IUserSession session, long cursor = -1, int user_id = 0, string screen_name = "", int count = 20)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                    {"count",count.ToString()},
                                    { "cursor", cursor.ToString() },
                                    //TODO: add these as params
                                    {"skip_status",true.ToString()},
                                    {"include_user_entities",false.ToString()},
                                 };

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            return await session.GetAsync(Api.Resolve("/1.1/followers/list.json"), parameters)
                             .ContinueWith(t => t.MapToUserListDetailedCursored());
        }

    }
}
