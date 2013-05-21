using System.Collections.Generic;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Models.Service;

namespace BoxKite.Twitter
{
    public static class DirectMessageExtensions
    {
        /// <summary>
        /// Returns the most recent direct messages sent to the authenticating user.
        /// </summary>
        /// <param name="since_id">Returns results with an ID greater than (that is, more recent than) the specified ID</param>
        /// <param name="max_id">Returns results with an ID less than (that is, older than) or equal to the specified ID</param>
        /// <param name="count">Specifies the number of direct messages to try and retrieve, up to a maximum of 200</param>
        /// <returns></returns>
        /// <remarks>ref: https://dev.twitter.com/docs/api/1.1/get/direct_messages </remarks>
        public async static Task<TwitterResponseCollection<DirectMessage>> GetDirectMessages(this IUserSession session, long since_id = 0, long max_id = 0, int count = 20)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"include_entities", true.ToString()},
                                     {"count", count.ToString()},
                                 };

            if (since_id > 0)
            {
                parameters.Add("since_id", since_id.ToString());
            }

            if (max_id > 0)
            {
                parameters.Add("max_id", max_id.ToString());
            }

            return await session.GetAsync(Api.Resolve("/1.1/direct_messages.json"), parameters)
                          .ContinueWith(c => c.MapToMany<DirectMessage>());
        }

        /// <summary>
        /// Returns the most recent direct messages sent by the authenticating user.
        /// </summary>
        /// <param name="since_id">Returns results with an ID greater than (that is, more recent than) the specified ID</param>
        /// <param name="max_id">Returns results with an ID less than (that is, older than) or equal to the specified ID</param>
        /// <param name="count">Specifies the number of direct messages to try and retrieve, up to a maximum of 200</param>
        /// <returns></returns>
        /// <remarks>ref: https://dev.twitter.com/docs/api/1.1/get/direct_messages/sent </remarks>
        public async static Task<TwitterResponseCollection<DirectMessage>> GetDirectMessagesSent(this IUserSession session, long since_id = 0, long max_id = 0, int count = 20)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"include_entities", true.ToString()},
                                     {"count", count.ToString()},
                                 };

            if (since_id > 0)
            {
                parameters.Add("since_id", since_id.ToString());
            }

            if (max_id > 0)
            {
                parameters.Add("max_id", max_id.ToString());
            }

            return await session.GetAsync(Api.Resolve("/1.1/direct_messages/sent.json"), parameters)
                          .ContinueWith(c => c.MapToMany<DirectMessage>());
        }

        /// <summary>
        /// Returns a direct message sent to the authenticating user.
        /// </summary>
        /// <param name="id">ID of direct message to return</param>
        /// <returns></returns>
        /// <remarks>ref: https://dev.twitter.com/docs/api/1.1/get/direct_messages/show </remarks>
        public async static Task<DirectMessage> GetDirectMessageSingle(this IUserSession session, long id)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"id", id.ToString()},
                                 };

            return await session.GetAsync(Api.Resolve("/1.1/direct_messages/show.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<DirectMessage>());
        }

        /// <summary>
        /// Sends a direct message sent to a user.
        /// </summary>
        /// <param name="text">Text to send to user</param>
        /// <param name="screen_name">Screen name of the recipient</param>
        /// <returns></returns>
        /// <remarks>ref: https://dev.twitter.com/docs/api/1.1/post/direct_messages/new </remarks>
        public async static Task<DirectMessage> SendDirectMessage(this IUserSession session, string text, string screen_name)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"include_entities", true.ToString()},
                                     {"screen_name", screen_name},
                                     {"text", text}
                                 };

            return await session.PostAsync(Api.Resolve("/1.1/direct_messages/new.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<DirectMessage>());
        }

        /// <summary>
        /// Deletes a sent direct message
        /// </summary>
        /// <param name="id">ID of the direct message to delete</param>
        /// <returns></returns>
        /// <remarks>ref: https://dev.twitter.com/docs/api/1.1/post/direct_messages/destroy </remarks>
        public async static Task<TwitterSuccess> DeleteDirectMessage(this IUserSession session, long id)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"include_entities", true.ToString()},
                                     {"id", id.ToString()}
                                 };

            return await session.PostAsync(Api.Resolve("/1.1/direct_messages/destroy.json"), parameters)
                          .ContinueWith(c => c.MapToTwitterSuccess());
        }
    }
}