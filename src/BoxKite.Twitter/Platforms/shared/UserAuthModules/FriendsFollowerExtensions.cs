// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public static class FriendsFollowersExtensions
    {
        /// <summary>
        /// Returns a cursored collection of user IDs for every user the specified user is following (otherwise known as their "friends")
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <param name="userId">screen_name or user_id must be provided</param>
        /// <param name="screenName">screen_name or user_id must be provided</param>
        /// <param name="count">how many to return default 500</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/friends/ids </remarks>
        public async static Task<FriendsFollowersIDsCursored> GetFriendsIDs(this ITwitterSession session, string screenName = "", int userId = 0, int count = 500, long cursor = -1)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(count: count, cursor:cursor, screen_name:screenName, user_id:userId);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<FriendsFollowersIDsCursored>(
                        "Either screen_name or user_id required");
            }

            return await session.GetAsync(TwitterApi.Resolve("/1.1/friends/ids.json"), parameters)
                             .ContinueWith(t => t.MapToSingle<FriendsFollowersIDsCursored>());
        }

        /// <summary>
        /// Returns a cursored collection of user IDs following a particular user(otherwise known as their "followers")
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <param name="userId">screen_name or user_id must be provided</param>
        /// <param name="screenName">screen_name or user_id must be provided</param>
        /// <param name="count">how many to return default 500</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/followers/ids </remarks>
        public static async Task<FriendsFollowersIDsCursored> GetFollowersIDs(this ITwitterSession session, string screenName = "", int userId = 0, int count = 500, long cursor = -1)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(count: count, cursor: cursor, screen_name: screenName, user_id: userId);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<FriendsFollowersIDsCursored>(
                        "Either screen_name or user_id required");
            }

            return await session.GetAsync(TwitterApi.Resolve("/1.1/followers/ids.json"), parameters)
                             .ContinueWith(t => t.MapToSingle<FriendsFollowersIDsCursored>());
        }


        /// <summary>
        /// Returns the relationships of the authenticating user to the comma-separated list of up to 100 screen_names or user_ids provided. Values for connections can be: following, following_requested, followed_by, none.
        /// </summary>
        /// <param name="screenNames">list of screen_names to check</param>
        /// <param name="userIds">list of user_ids to check against</param>
        /// <returns></returns>
        /// <remarks> ref : https://dev.twitter.com/docs/api/1.1/get/friendships/lookup </remarks>
        public async static Task<TwitterResponseCollection<FriendshipLookupResponse>> GetFriendships(this IUserSession session, IEnumerable<string> screenNames = null, IEnumerable<long> userIds = null)
        {
            var parameters = new TwitterParametersCollection();
            parameters.CreateCollection(screen_names: screenNames, user_ids:userIds);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<TwitterResponseCollection<FriendshipLookupResponse>>(
                        "Either screen_names or user_ids required");
            }

            var url = TwitterApi.Resolve("/1.1/friendships/lookup.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(f => f.MapToMany<FriendshipLookupResponse>());
        }

        /// <summary>
        /// Returns a collection of numeric IDs for every user who has a pending request to follow the authenticating user.
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/friendships/incoming </remarks>
        public async static Task<FriendsFollowersIDsCursored> GetFriendshipRequestsIncoming(this IUserSession session, long cursor = -1)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(cursor:cursor);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/friendships/incoming.json"), parameters)
                             .ContinueWith(t => t.MapToSingle<FriendsFollowersIDsCursored>());
        }

        /// <summary>
        /// Returns a collection of numeric IDs for every protected user for whom the authenticating user has a pending follow request.
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/friendships/outgoing </remarks>
        public async static Task<FriendsFollowersIDsCursored> GetFriendshipRequestsOutgoing(this IUserSession session, long cursor = -1)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(cursor: cursor);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/friendships/outgoing.json"), parameters)
                             .ContinueWith(t => t.MapToSingle<FriendsFollowersIDsCursored>());
        }

        /// <summary>
        /// Allows the authenticating users to follow the user specified.
        /// </summary>
        /// <param name="screenName">The screen name of the user for whom to befriend.</param>
        /// <param name="user_id">The ID of the user for whom to befriend.</param>
        /// <param name="follow">Enable notifications for the target user.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/friendships/create </remarks>
        public async static Task<User> CreateFriendship(this IUserSession session, string screenName = "",
            int user_id = 0, bool follow=true)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(screen_name:screenName,user_id:user_id,follow:follow);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<User>(
                        "Either screen_name or user_id required");
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/friendships/create.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<User>());
        }

        /// <summary>
        /// Allows the authenticating users to follow the user specified.
        /// </summary>
        /// <param name="screenName">The screen name of the user for whom to un-befriend.</param>
        /// <param name="user_id">The ID of the user for whom to un-befriend.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/friendships/destroy </remarks>
        public async static Task<User> DeleteFriendship(this IUserSession session, string screenName = "",
            int user_id = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(screen_name: screenName, user_id: user_id);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<User>(
                        "Either screen_name or user_id required");
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/post/friendships/destroy.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<User>());
        }

        /// <summary>
        /// Allows one to enable or disable retweets and device notifications from the specified user.
        /// </summary>
        /// <param name="screenName">The screen name of the user</param>
        /// <param name="user_id">The ID of the user</param>
        /// <param name="device">Enable/disable device notifications from the target user.</param>
        /// <param name="retweets">Enable/disable retweets from the target user.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/friendships/update </remarks>
        public async static Task<UserStatus> ChangeFriendship(this IUserSession session, string screenName = "",
            int user_id = 0, bool device=false, bool retweets=false)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(screen_name: screenName, user_id: user_id,device:device,retweets:retweets);
            
            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<UserStatus>(
                        "Either screen_name or user_id required");
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/friendships/update.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<UserStatus>());
        }

        /// <summary>
        /// Returns detailed information about the relationship between two arbitrary users.
        /// </summary>
        /// <param name="sourceScreenName">The user_id of the subject user.</param>
        /// <param name="sourceId">The screen_name of the subject user.</param>
        /// <param name="targetId">The user_id of the target user.</param>
        /// <param name="targetScreenName">The screen_name of the target user.</param>
        /// <returns></returns>
        /// <remarks> ref: https://api.twitter.com/1.1/friendships/show.json </remarks>
        public async static Task<UserStatus> GetFriendship(this IUserSession session, string sourceScreenName="",string targetScreenName="", int sourceId=0,int targetId=0)
        {
            var parameters = new TwitterParametersCollection();

            if (!string.IsNullOrWhiteSpace(sourceScreenName))
            {
                parameters.Add("source_screen_name", sourceScreenName);
            }

            if (sourceId != 0)
            {
                parameters.Add("source_id", sourceId.ToString());
            }

            if (!string.IsNullOrWhiteSpace(targetScreenName))
            {
                parameters.Add("target_screen_name", targetScreenName);
            }

            if (targetId != 0)
            {
                parameters.Add("target_id", targetId.ToString());
            }

            if (parameters.EnsureAllArePresent(new [] { "source_screen_name", "source_id", "target_screen_name", "target_id" }).IsFalse())
            {
                return session.MapParameterError<UserStatus>(
                        "source_screen_name, source_id, target_screen_name and target_id are all required");
            }


            return await session.PostAsync(TwitterApi.Resolve("/1.1/friendships/show.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<UserStatus>());
        }

        /// <summary>
        /// Returns detailed information about the relationship between two arbitrary users.
        /// </summary>
        /// <param name="sourceScreenName">The user_id of the subject user.</param>
        /// <param name="sourceId">The screen_name of the subject user.</param>
        /// <param name="targetId">The user_id of the target user.</param>
        /// <param name="targetScreenName">The screen_name of the target user.</param>
        /// <returns></returns>
        /// <remarks> ref: https://api.twitter.com/1.1/friendships/show.json </remarks>
        public async static Task<UserStatus> GetFriendship(this ITwitterSession session, string sourceScreenName = "", string targetScreenName = "", int sourceId = 0, int targetId = 0)
        {
            var parameters = new TwitterParametersCollection();

            if (!string.IsNullOrWhiteSpace(sourceScreenName))
            {
                parameters.Add("source_screen_name", sourceScreenName);
            }

            if (sourceId != 0)
            {
                parameters.Add("source_id", sourceId.ToString());
            }

            if (!string.IsNullOrWhiteSpace(targetScreenName))
            {
                parameters.Add("target_screen_name", targetScreenName);
            }

            if (targetId != 0)
            {
                parameters.Add("target_id", targetId.ToString());
            }

            return await session.GetAsync(TwitterApi.Resolve("/1.1/friendships/show.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<UserStatus>());
        }

        /// <summary>
        /// Returns a cursored collection of user objects for every user the specified user is following (otherwise known as their "friends").
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <param name="userId">screen_name or user_id must be provided</param>
        /// <param name="screenName">screen_name or user_id must be provided</param>
        /// <param name="count">how many to return default 500</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/friends/list </remarks>
        public async static Task<UserListDetailedCursored> GetFriendsList(this ITwitterSession session, string screenName = "", int userId = 0, int count = 20, long cursor = -1)
        {
            var parameters = new TwitterParametersCollection
                                    {
                                        {"include_user_entities", true.ToString()},
                                    };
            parameters.Create(count: count, cursor: cursor, screen_name: screenName, user_id: userId, skip_status:true);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<UserListDetailedCursored>(
                        "Either screen_name or user_id required");
            }

            return await session.GetAsync(TwitterApi.Resolve("/1.1/friends/list.json"), parameters)
                             .ContinueWith(t => t.MapToSingle<UserListDetailedCursored>());
        }

        /// <summary>
        /// Returns a cursored collection of user objects for users following the specified user.
        /// Presently in most recent following first
        /// </summary>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
        /// <param name="userId">screen_name or user_id must be provided</param>
        /// <param name="screenName">screen_name or user_id must be provided</param>
        /// <param name="count">how many to return default 500</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/followers/list </remarks>
        public async static Task<UserListDetailedCursored> GetFollowersList(this ITwitterSession session, string screenName = "", int userId = 0, int count = 20, long cursor = -1)
        {
            var parameters = new TwitterParametersCollection
                                    {
                                        {"include_user_entities", true.ToString()},
                                    };
            parameters.Create(count: count, cursor: cursor, screen_name: screenName, user_id: userId, skip_status:true);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<UserListDetailedCursored>(
                        "Either screen_name or user_id required");
            }

            return await session.GetAsync(TwitterApi.Resolve("/1.1/followers/list.json"), parameters)
                             .ContinueWith(t => t.MapToSingle<UserListDetailedCursored>());
        }

    }
}
