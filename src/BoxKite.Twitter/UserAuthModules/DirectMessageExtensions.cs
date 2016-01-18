// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public static class DirectMessageExtensions
    {
        /// <summary>
        /// Returns the most recent direct messages sent to the authenticating user.
        /// </summary>
        /// <param name="sinceId">Returns results with an ID greater than (that is, more recent than) the specified ID</param>
        /// <param name="maxId">Returns results with an ID less than (that is, older than) or equal to the specified ID</param>
        /// <param name="count">Specifies the number of direct messages to try and retrieve, up to a maximum of 200</param>
        /// <returns>(awaitable) IEnumerable of DirectMessages received by the session's authenticated user</returns>
        /// <remarks>ref: https://dev.twitter.com/docs/api/1.1/get/direct_messages </remarks>
        public async static Task<TwitterResponseCollection<DirectMessage>> GetDirectMessages(this IUserSession session, long sinceId = 0, long maxId = 0, int count = 20)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true, count: count, since_id: sinceId,
                max_id: maxId,full_text: true);
                
            return await session.GetAsync(TwitterApi.Resolve("/1.1/direct_messages.json"), parameters)
                          .ContinueWith(c => c.MapToMany<DirectMessage>());
        }

        /// <summary>
        /// Returns the most recent direct messages sent by the authenticating user.
        /// </summary>
        /// <param name="sinceId">Returns results with an ID greater than (that is, more recent than) the specified ID</param>
        /// <param name="maxId">Returns results with an ID less than (that is, older than) or equal to the specified ID</param>
        /// <param name="count">Specifies the number of direct messages to try and retrieve, up to a maximum of 200</param>
        /// <returns>(awaitable) IEnumerable of DirectMessages Sent by the session's authenticated user</returns>
        /// <remarks>ref: https://dev.twitter.com/docs/api/1.1/get/direct_messages/sent </remarks>
        public async static Task<TwitterResponseCollection<DirectMessage>> GetDirectMessagesSent(this IUserSession session, long sinceId = 0, long maxId = 0, int count = 20)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true, count: count, since_id: sinceId,
                max_id: maxId, full_text:true);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/direct_messages/sent.json"), parameters)
                          .ContinueWith(c => c.MapToMany<DirectMessage>());
        }

        /// <summary>
        /// Returns a direct message sent to the authenticating user.
        /// </summary>
        /// <param name="Id">ID of direct message to return</param>
        /// <returns>(awaitable) Get A DirectMessage sent/received the session's authenticated user</returns>
        /// <remarks>ref: https://dev.twitter.com/docs/api/1.1/get/direct_messages/show </remarks>
        public async static Task<DirectMessage> GetDirectMessageSingle(this IUserSession session, long Id)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(id: Id, full_text:true);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/direct_messages/show.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<DirectMessage>());
        }

        /// <summary>
        /// Sends a direct message sent to a user.
        /// </summary>
        /// <param name="text">Text to send to user</param>
        /// <param name="screenName">Screen name of the recipient</param>
        /// <returns></returns>
        /// <remarks>ref: https://dev.twitter.com/docs/api/1.1/post/direct_messages/new </remarks>
        public async static Task<DirectMessage> SendDirectMessage(this IUserSession session, string screenName, string text)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities:true, screen_name:screenName, text:text.TrimAndTruncate(140));
            if (parameters.EnsureAllArePresent(new [] {"screen_name", "text"}).IsFalse())
            {
                return session.MapParameterError<DirectMessage>(
                        "Either screen_name and text required");
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/direct_messages/new.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<DirectMessage>());
        }

        /// <summary>
        /// Deletes a sent direct message
        /// </summary>
        /// <param name="Id">ID of the direct message to delete</param>
        /// <returns>TwitterSuccess (true) if deletion worked</returns>
        /// <remarks>ref: https://dev.twitter.com/docs/api/1.1/post/direct_messages/destroy </remarks>
        public async static Task<TwitterSuccess> DeleteDirectMessage(this IUserSession session, long Id)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(id: Id);

            return await session.PostAsync(TwitterApi.Resolve("/1.1/direct_messages/destroy.json"), parameters)
                          .ContinueWith(c => c.MapToTwitterSuccess());
        }
    }
}