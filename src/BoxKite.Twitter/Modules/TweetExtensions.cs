using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Models.Service;

namespace BoxKite.Twitter
{
    // TODO: display_coordinates (optional)
    // TODO: manage https://support.twitter.com/articles/14020-twitter-for-sms-basic-features

    public static class TweetExtensions
    {
        /// <summary>
        /// Sends a Tweet
        /// </summary>
        /// <param name="text">Text of tweet to send</param>
        /// <param name="latitude">Latitude of sender</param>
        /// <param name="longitude">Longotide of sender</param>
        /// <param name="place_id">A place in the world identified by a Twitter place ID. Place IDs can be retrieved from geo/reverse_geocode.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/update </remarks>
        public async static Task<Tweet> SendTweet(this IUserSession session, string text, double latitude = 0.0, double longitude = 0.0, string place_id="")
        {
            var parameters = new TwitterParametersCollection
                                 {
                                     { "status", text.TrimAndTruncate(1000)},
                                     { "trim_user", true.ToString() },
                                 };
            parameters.Create(include_entities:true,place_id:place_id);
            
            if (latitude != 0.0 && longitude != 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());
            }

            return await session.PostAsync(Api.Resolve("/1.1/statuses/update.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// Deletes tweet of a given id
        /// </summary>
        /// <param name="id">ID of the tweet to delete</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/destroy/%3Aid </remarks>
        public async static Task<TwitterSuccess> DeleteTweet(this IUserSession session, string id)
        {
            var parameters = new TwitterParametersCollection();
            var url = Api.Resolve("/1.1/statuses/destroy/{0}.json", id); 
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
        /// <param name="place_id">A place in the world identified by a Twitter place ID. Place IDs can be retrieved from geo/reverse_geocode.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/update </remarks>
        public async static Task<Tweet> ReplyToTweet(this IUserSession session, Tweet tweet, string text, double latitude=0.0, double longitude = 0.0, string place_id="")
        {
             var parameters = new TwitterParametersCollection
                                {
                                     {"status", text },
                                     {"in_reply_to_status_id", tweet.Id.ToString()}
                                 };
            parameters.Create(place_id:place_id);

            if (latitude != 0.0 && longitude != 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());
            }

            return await session.PostAsync(Api.Resolve("/1.1/statuses/update.json"), parameters)
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
            var path = Api.Resolve("/1.1/statuses/retweet/{0}.json", tweet.Id);

            return await session.PostAsync(path, parameters)
                .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// Gets a particular Tweet
        /// </summary>
        /// <param name="tweetID">The tweet ID to return</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/statuses/show/%3Aid  </remarks>
        public async static Task<Tweet> GetTweet(this IUserSession session, long tweetID)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"trim_user",false.ToString()},
                                 {"include_my_retweet",true.ToString()},
                             };
            parameters.Create(id: tweetID, include_entities:true);

            return await session.GetAsync(Api.Resolve("/1.1/statuses/show.json"), parameters)
                .ContinueWith(c => c.MapToSingle<Tweet>());
        }

        /// <summary>
        /// Gets the Retweets of a particular tweet
        /// </summary>
        /// <param name="tweet"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async static Task<TwitterResponseCollection<Tweet>> GetRetweets(this IUserSession session, Tweet tweet, int count = 20)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"count", count.ToString()},
                             };
            var path = Api.Resolve("/1.1/statuses/retweets/{0}.json", tweet.Id);
            return await session.GetAsync(path, parameters)
                .ContinueWith(c => c.MapToMany<Tweet>());
        }

        /// <summary>
        /// Sends a Tweet, with status text, with attached image
        /// </summary>
        /// <param name="text">Text of tweet to send</param>
        /// <param name="fileName">Name of the file, including extension</param>
        /// <param name="imageData">the image data as a byte array</param>
        /// <param name="place_id">A place in the world identified by a Twitter place ID. Place IDs can be retrieved from geo/reverse_geocode.</param>
        /// <returns>Tweet sent</returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/update_with_media </remarks>
        public async static Task<Tweet> SendTweetWithImage(this IUserSession session, string text, string fileName, byte[] imageData, double latitude = 0.0, double longitude = 0.0, string place_id="")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"status", text},
                             };
            parameters.Create(place_id:place_id);

            if (latitude != 0.0 && longitude != 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());
            }

            return await session.PostFileAsync(Api.Upload("/1/statuses/update_with_media.json"), parameters, fileName, imageData, "media[]")
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
        /// <param name="place_id">A place in the world identified by a Twitter place ID. Place IDs can be retrieved from geo/reverse_geocode.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/statuses/update_with_media </remarks>
        public async static Task<Tweet> ReplyToTweetWithImage(this IUserSession session, Tweet tweet, string text, string fileName, byte[] imageData, double latitude = 0.0, double longitude = 0.0, string place_id = "")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"status", text},
                                 {"in_reply_to_status_id", tweet.Id.ToString()}
                             };

            parameters.Create(place_id: place_id);
            
            if (latitude != 0.0 && longitude != 0.0)
            {
                parameters.Add("lat", latitude.ToString());
                parameters.Add("long", longitude.ToString());
            }

            return await session.PostFileAsync(Api.Upload("/1/statuses/update_with_media.json"), parameters, fileName, imageData, "media[]")
                          .ContinueWith(c => c.MapToSingle<Tweet>());
        }
    }
}
