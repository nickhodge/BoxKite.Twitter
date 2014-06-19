// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public static class TweetExtensions
    {
        /// <summary>
        /// Sends a Tweet
        /// </summary>
        /// <param name="text">Text of tweet to send</param>
        /// <param name="latitude">Latitude of sender</param>
        /// <param name="longitude">Longotide of sender</param>
        /// <param name="placeId">A place in the world identified by a Twitter place ID. Place IDs can be retrieved from geo/reverse_geocode.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/update </remarks>
        public async static Task<Tweet> SendTweet(this IUserSession session, string text, double latitude = 0.0, double longitude = 0.0, string placeId="")
        {
            var parameters = new TwitterParametersCollection
                                 {
                                     { "status", text.TrimAndTruncate(1000)},
                                     { "trim_user", true.ToString() },
                                 };
            parameters.Create(include_entities:true,place_id:placeId);
            
            if (latitude != 0.0 && longitude != 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/statuses/update.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// Deletes tweet of a given id
        /// </summary>
        /// <param name="tweetId">ID of the tweet to delete</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/destroy/%3Aid </remarks>
        public async static Task<TwitterSuccess> DeleteTweet(this IUserSession session, string tweetId)
        {
            var parameters = new TwitterParametersCollection();
            var url = TwitterApi.Resolve("/1.1/statuses/destroy/{0}.json", tweetId); 
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToTwitterSuccess());
        }

        /// <summary>
        /// Sends a Tweet in reply to another tweet
        /// </summary>
        /// <param name="tweet">Tweet replying to</param>
        /// <param name="text">Text of the reply</param>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="placeId">A place in the world identified by a Twitter place ID. Place IDs can be retrieved from geo/reverse_geocode.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/update </remarks>
        public async static Task<Tweet> ReplyToTweet(this IUserSession session, Tweet tweet, string text, double latitude=0.0, double longitude = 0.0, string placeId="")
        {
             var parameters = new TwitterParametersCollection
                                {
                                     {"status", text },
                                     {"in_reply_to_status_id", tweet.Id.ToString()}
                                 };
            parameters.Create(place_id:placeId);

            if (latitude != 0.0 && longitude != 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/statuses/update.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// Retweets a tweet
        /// </summary>
        /// <param name="tweet">The tweet to retweet</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/retweet/%3Aid </remarks>
        public async static Task<Tweet> Retweet(this IUserSession session, Tweet tweet)
        {
            var parameters = new TwitterParametersCollection();
            var path = TwitterApi.Resolve("/1.1/statuses/retweet/{0}.json", tweet.Id);

            return await session.PostAsync(path, parameters)
                .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// Gets a particular Tweet
        /// </summary>
        /// <param name="tweetId">The tweet ID to return</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/statuses/show/%3Aid  </remarks>
        public async static Task<Tweet> GetTweet(this ITwitterSession session, long tweetId)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"trim_user",false.ToString()},
                                 {"include_my_retweet",true.ToString()},
                             };
            parameters.Create(id: tweetId, include_entities:true);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/statuses/show.json"), parameters)
                .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// Gets the Retweets of a particular tweet
        /// </summary>
        /// <param name="tweet"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/statuses/retweets/%3Aid  </remarks>
        public async static Task<TwitterResponseCollection<Tweet>> GetRetweets(this ITwitterSession session, Tweet tweet, int count = 20)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"count", count.ToString()},
                             };
            var path = TwitterApi.Resolve("/1.1/statuses/retweets/{0}.json", tweet.Id);
            return await session.GetAsync(path, parameters)
                .ContinueWith(c => c.MapToMany<Tweet>());
        }


        /// <summary>
        /// Gets the Retweets of a particular tweet
        /// </summary>
        /// <param name="tweetId"></param>
        /// <param name="count"></param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/statuses/retweets/%3Aid  </remarks>
        public async static Task<TwitterResponseCollection<Tweet>> GetRetweets(this ITwitterSession session, long tweetId, int count = 20)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"count", count.ToString()},
                             };
            var path = TwitterApi.Resolve("/1.1/statuses/retweets/{0}.json", tweetId);
            return await session.GetAsync(path, parameters)
                .ContinueWith(c => c.MapToMany<Tweet>());
        }

        /// <summary>
        /// Gets the Users who retweeted a particular tweet
        /// </summary>
        /// <param name="tweetId"></param>
        /// <param name="cursor">default is first page (-1) otherwise provide starting point</param>
         /// <param name="screen_name">screen_name or user_id must be provided</param>
        /// <param name="count">how many to return default 500</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/statuses/retweeters/ids </remarks>
        public async static Task<RetweetersResponseIDsCursored> GetRetweeters(this IUserSession session, long tweetId, int count = 20, long cursor = -1)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(id: tweetId, count: count, cursor: cursor);

            var path = TwitterApi.Resolve("/1.1/statuses/retweeters/id.json", parameters);
            return await session.GetAsync(path, parameters)
                .ContinueWith(c => c.MapToSingle<RetweetersResponseIDsCursored>());
        }

        /// <summary>
        /// Sends a Tweet, with status text, with attached image
        /// </summary>
        /// <param name="text">Text of tweet to send</param>
        /// <param name="fileName">Name of the file, including extension</param>
        /// <param name="imageData">the image data as a byte array</param>
        /// <param name="placeId">A place in the world identified by a Twitter place ID. Place IDs can be retrieved from geo/reverse_geocode.</param>
        /// <returns>Tweet sent</returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/update_with_media </remarks>
        public async static Task<Tweet> SendTweetWithImage(this IUserSession session, string text, string fileName, byte[] imageData, double latitude = 0.0, double longitude = 0.0, string placeId="")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"status", text},
                             };
            parameters.Create(place_id:placeId);

            if (latitude != 0.0 && longitude != 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());
            }

            return await session.PostFileAsync(TwitterApi.Upload("/1.1/statuses/update_with_media.json"), parameters, fileName, "media[]", imageData)
                          .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// Sends a Tweet, with status text, with attached image
        /// </summary>
        /// <param name="text">Text of tweet to send</param>
        /// <param name="fileName">Name of the file, including extension</param>
        /// <param name="imageData">Stream of the image</param>
        /// <param name="placeId">A place in the world identified by a Twitter place ID. Place IDs can be retrieved from geo/reverse_geocode.</param>
        /// <returns>Tweet sent</returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/update_with_media </remarks>
        public async static Task<Tweet> SendTweetWithImage(this IUserSession session, string text, string fileName, Stream imageDataStream, double latitude = 0.0, double longitude = 0.0, string placeId = "")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"status", text},
                             };
            parameters.Create(place_id: placeId);

            if (latitude != 0.0 && longitude != 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());
            }

            return await session.PostFileAsync(TwitterApi.Upload("/1.1/statuses/update_with_media.json"), parameters, fileName, "media[]", srImage:imageDataStream)
                          .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// Sends a Tweet in reply to another tweet
        /// </summary>
        /// <param name="tweet">Text to send</param>
        /// <param name="text">Text of tweet to send</param>
        /// <param name="fileName">Name of the file, including extension</param>
        /// <param name="imageData">the image data as a byte array</param>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="placeId">A place in the world identified by a Twitter place ID. Place IDs can be retrieved from geo/reverse_geocode.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/update_with_media </remarks>
        public async static Task<Tweet> ReplyToTweetWithImage(this IUserSession session, Tweet tweet, string text, string fileName, byte[] imageData, double latitude = 0.0, double longitude = 0.0, string placeId = "")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"status", text},
                                 {"in_reply_to_status_id", tweet.Id.ToString()}
                             };

            parameters.Create(place_id: placeId);
            
            if (latitude != 0.0 && longitude != 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());
            }

            return await session.PostFileAsync(TwitterApi.Upload("/1.1/statuses/update_with_media.json"), parameters, fileName, "media[]", imageData)
                          .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// Sends a Tweet in reply to another tweet
        /// </summary>
        /// <param name="tweet">Text to send</param>
        /// <param name="text">Text of tweet to send</param>
        /// <param name="fileName">Name of the file, including extension</param>
        /// <param name="imageDataStream">Stream containing the image</param>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="placeId">A place in the world identified by a Twitter place ID. Place IDs can be retrieved from geo/reverse_geocode.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/update_with_media </remarks>
        public async static Task<Tweet> ReplyToTweetWithImage(this IUserSession session, Tweet tweet, string text, string fileName, Stream imageDataStream, double latitude = 0.0, double longitude = 0.0, string placeId = "")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"status", text},
                                 {"in_reply_to_status_id", tweet.Id.ToString()}
                             };

            parameters.Create(place_id: placeId);

            if (latitude != 0.0 && longitude != 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());
            }

            return await session.PostFileAsync(TwitterApi.Upload("/1.1/statuses/update_with_media.json"), parameters, fileName, "media[]", srImage: imageDataStream)
                          .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// Returns fully-hydrated tweets for up to 100 tweets per request, as specified by comma-separated values passed to the user_id and/or screen_name parameters.
        /// </summary>
        /// <param name="id">up to 100 are allowed in a single request.</param>
        /// <returns>Observable List of full tweets</returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/statuses/lookup </remarks>
        public static async Task<TwitterResponseCollection<Tweet>> GetTweetsFull(this IUserSession session, IEnumerable<long> tweetIds = null)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true);
            parameters.CreateCollection(tweetids: tweetIds);

            return await session.PostAsync(TwitterApi.Resolve("/1.1/statuses/lookup.json"), parameters)
                .ContinueWith(c => c.MapToMany<Tweet>());
        }
    }
}
