// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public static class ListExtensions
    {
        /// <summary>
        /// Returns all lists the authenticating or specified user subscribes to, including their own. The user is specified using the user_id or screen_name parameters. If no user is given, the authenticating user is used.
        /// </summary>
        /// <param name="userId">The ID of the user for whom to return results for. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="screenName">The screen name of the user for whom to return results for.</param>
        /// <param name="reverse">Set this to true if you would like owned lists to be returned first.</param>
        /// <returns>(awaitable) IEnumerable Lists the authenticated user or screen_name subscribes to</returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/list </remarks>
        public static async Task<TwitterResponseCollection<TwitterList>> GetLists(this ITwitterSession session, long userId = 0, string screenName = "", bool reverse = false)
        {
            var parameters = new TwitterParametersCollection {{"reverse", reverse.ToString()}};
            parameters.Create(screen_name: screenName, user_id: userId);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<TwitterResponseCollection<TwitterList>>(
                        "Either screen_name or user_id required");
            }

            return await session.GetAsync(TwitterApi.Resolve("/1.1/lists/list.json"), parameters)
                          .ContinueWith(c => c.MapToMany<TwitterList>());
        }

        /// <summary>
        /// Returns a timeline of tweets authored by members of the specified list. Retweets are included by default.
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="sinceId">Returns results with an ID greater than (that is, more recent than) the specified ID.</param>
        /// <param name="count">Specifies the number of results to retrieve per "page."</param>
        /// <param name="maxId">Returns results with an ID less than (that is, older than) or equal to the specified ID.</param>
        /// <param name="includeRetweets">the list timeline will contain native retweets (if they exist) in addition to the standard stream of tweets.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/statuses </remarks>
        public static async Task<TwitterResponseCollection<Tweet>> GetListTimeline(this ITwitterSession session, long listId, string slug, long ownerId = 0, string ownerScreenName = "", long sinceId = 0, int count = 20, long maxId = 0, bool includeRetweets = true)
        {
            var parameters = new TwitterParametersCollection
                                 {
                                     {"include_rts", includeRetweets.ToString()},
                                 };
            parameters.Create(list_id:listId, slug:slug, owner_id:ownerId, owner_screen_name:ownerScreenName, since_id:sinceId, max_id:maxId);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/lists/statuses.json"), parameters)
                          .ContinueWith(c => c.MapToMany<Tweet>());
        }

        /// <summary>
        /// Removes the specified member from the list. The authenticated user must be the list's owner to remove members from the list.
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="userId">The ID of the user to remove from the list. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="screenName">The screen name of the user for whom to remove from the list. Helpful for disambiguating when a valid screen name is also a user ID.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/members/destroy </remarks>
        public static async Task<TwitterSuccess> DeleteUserFromList(this IUserSession session, long listId = 0, string slug = "",
            long userId = 0, string screenName = "", string ownerScreenName = "", long ownerId = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName, user_id: userId, screen_name: screenName);

            return await session.PostAsync(TwitterApi.Resolve("/1.1/lists/members/destroy"), parameters)
                          .ContinueWith(c => c.MapToTwitterSuccess());
        }

        /// <summary>
        /// Returns the lists the specified user has been added to. If user_id or screen_name are not provided the memberships for the authenticating user are returned.
        /// </summary>
        /// <param name="userId">The ID of the user for whom to return results for.</param>
        /// <param name="screenName">The screen name of the user for whom to return results for.</param>
        /// <param name="cursor">Breaks the results into pages. Provide a value of -1 to begin paging.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/memberships </remarks>
        public static async Task<UserInListCursored> GetListMembershipForUser(this ITwitterSession session, long userId = 0,
            string screenName = "", long cursor = -1)
        {
            var parameters = new TwitterParametersCollection
                                 {
                                     {"filter_to_owned_lists", false.ToString()},
                                 };
            parameters.Create(cursor: cursor, user_id: userId, screen_name: screenName);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/lists/memberships.json"), parameters)
                          .ContinueWith(c => c.MapToSingle <UserInListCursored>()); 
        }

        /// <summary>
        /// Returns the authenticating user's lists the specified user has been added to.
        /// </summary>
        /// <param name="userId">The ID of the user for whom to return results for.</param>
        /// <param name="screenName">The screen name of the user for whom to return results for.</param>
        /// <param name="cursor">Breaks the results into pages. Provide a value of -1 to begin paging.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/memberships </remarks>
        public static async Task<UserInListCursored> GetMyListsUserIsMemberOf(this ITwitterSession session, long userId = 0,
            string screenName = "", long cursor = -1)
        {
            var parameters = new TwitterParametersCollection
                                 {
                                     {"filter_to_owned_lists", false.ToString()},
                                 };
            parameters.Create(cursor: cursor, user_id: userId, screen_name: screenName);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/lists/memberships.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<UserInListCursored>());
        }

        /// <summary>
        /// Returns the subscribers of the specified list. Private list subscribers will only be shown if the authenticated user owns the specified list.
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="cursor">Breaks the results into pages.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/subscribers </remarks>
        public static async Task<UserListDetailedCursored> GetListSubscribers(this ITwitterSession session, long listId,
            string slug="", long ownerId = 0,
            string ownerScreenName = "", long cursor = -1)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(skip_status:false, include_entities:true, cursor:cursor, list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/lists/subscribers.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<UserListDetailedCursored>()); 
        }

        /// <summary>
        /// Subscribes the authenticated user to the specified list.
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/subscribers/create </remarks>
        public static async Task<TwitterList> SubscribeToUsersList(this IUserSession session, long listId,
            string slug, long ownerId = 0, string ownerScreenName = "")
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName);

            return await session.PostAsync(TwitterApi.Resolve("/1.1/lists/subscribers/create.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<TwitterList>());
        }

        /// <summary>
        /// Check if the specified user is a subscriber of the specified list. Returns the user if they are subscriber.
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="userId">The ID of the user for whom to return results for. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="screenName">The screen name of the user for whom to return results for. Helpful for disambiguating when a valid screen name is also a user ID.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/subscribers/show </remarks>
        public static async Task<User> IsSubscribedToList(this ITwitterSession session, long listId, string slug,
    long userId = 0, string screenName = "", string ownerScreenName = "", long ownerId = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName, user_id: userId, screen_name: screenName);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/lists/subscribers/show.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<User>());
        }

        /// <summary>
        /// Unsubscribes the authenticated user from the specified list.
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/subscribers/destroy </remarks>
        public static async Task<TwitterSuccess> DeleteFromUsersList(this IUserSession session, long listId, string slug,
            string ownerScreenName = "", long ownerId = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName);

            return await session.PostAsync(TwitterApi.Resolve("/1.1/lists/subscribers/destroy.json"), parameters)
                          .ContinueWith(c => c.MapToTwitterSuccess());
        }
        
        /// <summary>
        /// Adds multiple members to a list (up to 100)
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="screenNames">list of screen names, up to 100 are allowed in a single request.</param>
        /// <param name="userIds">list of user IDs, up to 100 are allowed in a single request.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/members/create_all </remarks>
        public static async Task<TwitterSuccess> AddUsersToList(this IUserSession session, long listId, string slug,
            IEnumerable<string> screenNames, IEnumerable<long> userIds,
            string ownerScreenName = "", long ownerId = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName);
            parameters.CreateCollection(screen_names:screenNames,user_ids:userIds);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<TwitterSuccess>(
                        "Either screen_names or user_ids required");
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/lists/members/create_all.json"), parameters)
                          .ContinueWith(c => c.MapToTwitterSuccess());
        }

        /// <summary>
        /// Check if the specified user is a member of the specified list
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="userId">The ID of the user for whom to return results for. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="screenName">The screen name of the user for whom to return results for. Helpful for disambiguating when a valid screen name is also a user ID.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/members/show </remarks>
        public static async Task<User> IsUserOnList(this ITwitterSession session, long listId, string slug="",
            long userId = 0, string screenName = "", string ownerScreenName = "", long ownerId = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(user_id:userId,screen_name:screenName, list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName, skip_status:true,include_entities:true);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/lists/members/show.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<User>());
        }

        /// <summary>
        /// Returns the members of the specified list. Private list members will only be shown if the authenticated user owns the specified list.
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="cursor">Breaks the results into pages.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/members </remarks>
        public static async Task<UserListDetailedCursored> GetMembersOnList(this IUserSession session, long listId, string slug,
            string ownerScreenName = "", long ownerId = 0, long cursor = -1)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName, skip_status: true, include_entities: true, cursor:cursor);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/lists/members.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<UserListDetailedCursored>());
        }

        /// <summary>
        /// Add a member to a list. The authenticated user must own the list to be able to add members to it. 
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="screenNameToAdd">The screen name of the user for whom to return results for. Helpful for disambiguating when a valid screen name is also a user ID.</param>
        /// <param name="userIdToAdd">The ID of the user for whom to return results for. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/members/create </remarks>
        public static async Task<TwitterSuccess> AddUserToMyList(this IUserSession session, long listId,
    string screenNameToAdd="", long userIdToAdd=0, string slug ="", string ownerScreenName = "", long ownerId = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName, user_id: userIdToAdd, screen_name: screenNameToAdd);

            return await session.PostAsync(TwitterApi.Resolve("/1.1/lists/members/create.json"), parameters)
                          .ContinueWith(c => c.MapToTwitterSuccess());
        }

        /// <summary>
        /// Deletes the specified list. The authenticated user must own the list to be able to destroy it.
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
         /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/destroy </remarks>
        public static async Task<TwitterList> DeleteList(this IUserSession session, long listId,
             string slug, long ownerId = 0, string ownerScreenName = "")
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName);

            return await session.PostAsync(TwitterApi.Resolve("/1.1/lists/destroy.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<TwitterList>());
        }

        /// <summary>
        /// Updates the specified list. The authenticated user must own the list to be able to update it.
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="name">The name for the list.</param>
        /// <param name="mode">Whether your list is public or private. Values can be public or private. If no mode is specified the list will be public.</param>
        /// <param name="description">The description to give the list.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/update </remarks>
        public static async Task<TwitterSuccess> ChangeList(this IUserSession session, long listId,
            string slug, string name = "", string mode = "", string description = "", long ownerId = 0,
            string ownerScreenName = "")
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(name:name, list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName);

            if (!string.IsNullOrWhiteSpace(mode))
            {
                parameters.Add("mode", mode);
            }
            if (!string.IsNullOrWhiteSpace(description))
            {
                parameters.Add("description", description);
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/lists/update.json"), parameters)
                          .ContinueWith(c => c.MapToTwitterSuccess());
        }

        /// <summary>
        /// Creates a new list for the authenticated user. Note that you can't create more than 20 lists per account.
        /// </summary>
        /// <param name="name">The name for the list.</param>
        /// <param name="mode">Whether your list is public or private. Values can be public or private. If no mode is specified the list will be public.</param>
        /// <param name="description">The description to give the list.</param>
         /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/update </remarks>
        public static async Task<TwitterList> CreateList(this IUserSession session, string name, string mode, string description = "", long ownerId = 0,
            string ownerScreenName = "")
        {
            var parameters = new TwitterParametersCollection
                                 {
                                     {"name", name},
                                     {"mode", mode},
                                 };

            if (!string.IsNullOrWhiteSpace(description))
            {
                parameters.Add("description", description);
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/lists/create.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<TwitterList>());
        }

        /// <summary>
        /// Returns the specified list. Private lists will only be shown if the authenticated user owns the specified list.
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/show </remarks>
        public static async Task<TwitterList> GetList(this ITwitterSession session, long listId, string slug,
            string ownerScreenName = "", long ownerId = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/lists/show.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<TwitterList>());
        }

        /// <summary>
        /// Obtain a collection of the lists the specified user is subscribed to, 20 lists per page by default. Does not include the user's own lists.
        /// </summary>
        /// <param name="screenName">The screen name of the user for whom to return results for. Helpful for disambiguating when a valid screen name is also a user ID.</param>
        /// <param name="userId">The ID of the user for whom to return results for. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="count">The amount of results to return per page. Defaults to 20. No more than 1000 results will ever be returned in a single page.</param>
        /// <param name="cursor">Breaks the results into pages. Provide a value of -1 to begin paging.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/subscriptions </remarks>
        public static async Task<TwitterListCursored> GetMySubscriptions(this ITwitterSession session, 
            string screenName = "", long userId = 0, int count=20, long cursor= -1)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(user_id:userId,screen_name:screenName,count:count,cursor:cursor);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/lists/subscriptions.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<TwitterListCursored>());
        }

        /// <summary>
        /// Removes multiple members from a list, by specifying a comma-separated list of member ids or screen names. The authenticated user must own the list to be able to remove members from it.
        /// </summary>
        /// <param name="listId">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="screenNames">list of screen names, up to 100 are allowed in a single request.</param>
        /// <param name="userIds">list of user IDs, up to 100 are allowed in a single request.</param>
        /// <param name="ownerScreenName">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="ownerId">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/members/destroy_all </remarks>
        public static async Task<TwitterSuccess> DeleteUsersFromList(this IUserSession session, long listId=0, string slug="",
            IEnumerable<string> screenNames=null, IEnumerable<long> userIds=null,
            string ownerScreenName = "", long ownerId = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(list_id: listId, slug: slug, owner_id: ownerId, owner_screen_name: ownerScreenName);
            parameters.CreateCollection(screen_names:screenNames, user_ids:userIds);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<TwitterSuccess>(
                        "Either screen_names or user_ids required");
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/lists/members/destroy_all.json"), parameters)
                          .ContinueWith(c => c.MapToTwitterSuccess());
        }

        /// <summary>
        /// Returns the lists owned by the specified Twitter user. Private lists will only be shown if the authenticated user is also the owner of the lists.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/ownerships </remarks>
        public static async Task<TwitterListCursored> GetListOwned(this ITwitterSession session,
            string screenName = "", long userId = 0, int count = 20, long cursor = -1)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(user_id: userId, screen_name: screenName, count: count, cursor: cursor);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/lists/ownerships.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<TwitterListCursored>());
        }

    }
}
