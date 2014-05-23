// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL


using System.Collections.Generic;
using System.IO;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using System.Threading.Tasks;

namespace BoxKite.Twitter
{
    public static class UsersExtensions
    {
        /// <summary>
        /// Returns settings (including current trend, geo and sleep time information) for the authenticating user.
        /// </summary>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/account/settings </remarks>
        public static async Task<AccountSettings> GetAccountSettings(this IUserSession session)
        {
            var parameters = new TwitterParametersCollection();
            return await session.GetAsync(TwitterApi.Resolve("/1.1/account/settings.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<AccountSettings>());
        }

        /// <summary>
        /// Returns a variety of information about the user specified by the required user_id or screen_name parameter. The author's most recent Tweet will be returned inline when possible.
        /// </summary>
        /// <param name="screen_name">The screen name of the user for whom to return results for. Either a id or screen_name is required for this method.</param>
        /// <param name="user_id">The ID of the user for whom to return results for. Either an id or screen_name is required for this method.</param>
        /// <returns></returns>
        public static async Task<User> GetUserProfile(this IUserSession session, string screen_name="", long user_id=0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(screen_name:screen_name,include_entities:true,user_id:user_id);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<User>(
                        "Either screen_name or user_id required"); ;
            }

            return await session.GetAsync(TwitterApi.Resolve("/1.1/users/show.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<User>());
        }

        /// <summary>
        /// Returns an HTTP 200 OK response code and a representation of the requesting user if authentication was successful; returns a 401 status code and an error message if not. Use this method to test if supplied user credentials are valid.
        /// </summary>
        /// <returns></returns>
        public static async Task<User> GetVerifyCredentials(this IUserSession session) 
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/account/verify_credentials.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<User>());
        }

        /// <summary>
        /// Updates the authenticating user's settings.
        /// </summary>
        /// <param name="trend_location_woeid">The Yahoo! Where On Earth ID to use as the user's default trend location.</param>
        /// <param name="sleep_time_enabled">enable sleep time for the user. Sleep time is the time when push or SMS notifications should not be sent to the user.</param>
        /// <param name="start_sleep_time">The hour that sleep time should begin if it is enabled. (00)</param>
        /// <param name="end_sleep_time">The hour that sleep time should end if it is enabled. (23) </param>
        /// <param name="time_zone">The timezone dates and times should be displayed in for the user. http://api.rubyonrails.org/classes/ActiveSupport/TimeZone.html </param>
        /// <param name="lang">The language which Twitter should render in for this user https://dev.twitter.com/docs/api/1/get/help/languages </param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/settings </remarks>
        public static async Task<AccountSettings> ChangeAccountSettings(this IUserSession session, string trend_location_woeid = "1",
            bool sleep_time_enabled = false, string start_sleep_time = "", string end_sleep_time = "",
            string time_zone = "", string lang = "")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"sleep_time_enabled", sleep_time_enabled.ToString()},
                             };
            parameters.Create(include_entities: true);
 
            if (!string.IsNullOrWhiteSpace(trend_location_woeid))
            {
                parameters.Add("trend_location_woeid", trend_location_woeid);
            }

            if (!string.IsNullOrWhiteSpace(start_sleep_time))
            {
                parameters.Add("start_sleep_time", start_sleep_time);
            }

            if (!string.IsNullOrWhiteSpace(end_sleep_time))
            {
                parameters.Add("end_sleep_time", end_sleep_time);
            }

            if (!string.IsNullOrWhiteSpace(time_zone))
            {
                parameters.Add("time_zone", time_zone);
            }

            if (!string.IsNullOrWhiteSpace(lang))
            {
                parameters.Add("lang", lang);
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/account/settings.json"), parameters)
                          .ContinueWith(c => c.MapToSingle<AccountSettings>());
        }

        /// <summary>
        /// Sets values that users are able to set under the "Account" tab of their settings page. Only the parameters specified will be updated.
        /// </summary>
        /// <param name="name">Full name associated with the profile. Maximum of 20 characters.</param>
        /// <param name="purl">URL associated with the profile. Will be prepended with "http://" if not present. Maximum of 100 characters.</param>
        /// <param name="location">The city or country describing where the user of the account is located.</param>
        /// <param name="description">A description of the user owning the account. Maximum of 160 characters.</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/update_profile </remarks>
        public static async Task<AccountProfile> ChangeAccountProfile(this IUserSession session, string name = "",
            string purl = "", string location = "", string description = "")
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true,skip_status:true);

            // first 20 chars
            if (!string.IsNullOrWhiteSpace(name))
            {
                parameters.Add("name", name.TrimAndTruncate(20));
            }

            // first 100 chars
            if (!string.IsNullOrWhiteSpace(purl))
            {
                parameters.Add("purl", purl.TrimAndTruncate(100));
            }

            // first 30 chars
            if (!string.IsNullOrWhiteSpace(location))
            {
                parameters.Add("location", location.TrimAndTruncate(30));
            }

            // first 160 chars
            if (!string.IsNullOrWhiteSpace(description))
            {
                parameters.Add("description", description.TrimAndTruncate(160));
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/account/update_profile.json"), parameters)
                .ContinueWith(c => c.MapToSingle<AccountProfile>());
        }

        /// <summary>
        /// Updates the authenticating user's background image as displayed on twitter.com.
        /// </summary>
        /// <param name="imageContent">byte array of image content</param>
        /// <param name="tile">True or false to tile</param>
        /// <param name="useImage">Turn on/off image on background</param>
        /// <returns>Account Profile (note: image update may take up to 5s)</returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/update_profile_background_image </remarks>
        public static async Task<AccountProfile> ChangeAccountBackgroundImage(this IUserSession session, string fileName, byte[] imageContent, bool tile = true, bool useImage = true)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"tile", tile.ToString()},
                                 {"use", useImage.ToString()},
                             };

            return await session.PostFileAsync(TwitterApi.Resolve("/1.1/account/update_profile_background_image.json"), parameters, fileName, "image", imageContent)
                           .ContinueWith(c => c.MapToSingle<AccountProfile>());
        }


        /// <summary>
        /// Updates the authenticating user's background image as displayed on twitter.com.
        /// </summary>
        /// <param name="imageDataStream">Stream of image content</param>
        /// <param name="tile">True or false to tile</param>
        /// <param name="useImage">Turn on/off image on background</param>
        /// <returns>Account Profile (note: image update may take up to 5s)</returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/update_profile_background_image </remarks>
        public static async Task<AccountProfile> ChangeAccountBackgroundImage(this IUserSession session, string fileName, Stream imageDataStream, bool tile = true, bool useImage = true)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"tile", tile.ToString()},
                                 {"use", useImage.ToString()},
                             };

            return await session.PostFileAsync(TwitterApi.Resolve("/1.1/account/update_profile_background_image.json"), parameters, fileName, "image", srImage:imageDataStream)
                           .ContinueWith(c => c.MapToSingle<AccountProfile>());
        }

        /// <summary>
        /// Updates the authenticating user's profile image.
        /// </summary>
        /// <param name="fileName">file name for upload</param>
        /// <param name="imageContentStream">Stream pointing to the image data</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/update_profile_image </remarks>
        public static async Task<AccountProfile> ChangeAccountProfileImage(this IUserSession session, string fileName, Stream imageContentStream)
        {
            var parameters = new TwitterParametersCollection();

            return await session.PostFileAsync(TwitterApi.Resolve("/1.1/account/update_profile_image.json"), parameters, fileName, "image", srImage:imageContentStream)
                           .ContinueWith(c => c.MapToSingle<AccountProfile>());
        }

       
        /// <summary>
        /// Updates the authenticating user's profile image.
        /// </summary>
        /// <param name="fileName">file name for upload</param>
        /// <param name="imageContent">byte array of image content</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/update_profile_image </remarks>
        public static async Task<AccountProfile> ChangeAccountProfileImage(this IUserSession session, string fileName, byte[] imageContent)
        {
            var parameters = new TwitterParametersCollection();

            return await session.PostFileAsync(TwitterApi.Resolve("/1.1/account/update_profile_image.json"), parameters, fileName, "image", imageContent)
                           .ContinueWith(c => c.MapToSingle<AccountProfile>());
        }

        /// <summary>
        /// Sets one or more hex values that control the color scheme of the authenticating user's profile page on twitter.com. Each parameter's value must be a valid hexidecimal value, and may be either three or six characters (ex: #fff or #ffffff).
        /// </summary>
        /// <param name="profile_background_color">Example Values: 3D3D3D</param>
        /// <param name="profile_link_color">Example Values: 3D3D3D</param>
        /// <param name="profile_sidebar_border_color">Example Values: 3D3D3D</param>
        /// <param name="profile_sidebar_fill_color">Example Values: 3D3D3D</param>
        /// <param name="profile_text_color">Example Values: 3D3D3D</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/update_profile_colors </remarks>
        public static async Task<AccountProfile> ChangeAccountColours(this IUserSession session,
            string profile_background_color = "", string profile_link_color = "", string profile_sidebar_border_color = "",
            string profile_sidebar_fill_color = "", string profile_text_color = "")
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true, skip_status: true);

            if (!string.IsNullOrWhiteSpace(profile_background_color))
            {
                parameters.Add("profile_background_color", profile_background_color);
            }

            if (!string.IsNullOrWhiteSpace(profile_link_color))
            {
                parameters.Add("profile_link_color", profile_link_color);
            }

            if (!string.IsNullOrWhiteSpace(profile_sidebar_border_color))
            {
                parameters.Add("profile_sidebar_border_color", profile_sidebar_border_color);
            }

            if (!string.IsNullOrWhiteSpace(profile_sidebar_fill_color))
            {
                parameters.Add("profile_sidebar_fill_color", profile_sidebar_fill_color);
            }

            if (!string.IsNullOrWhiteSpace(profile_text_color))
            {
                parameters.Add("profile_text_color", profile_text_color);
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/account/update_profile_colors.json"), parameters)
                           .ContinueWith(c => c.MapToSingle<AccountProfile>());
        }

        /// <summary>
        /// Sets which device Twitter delivers updates to for the authenticating user. Sending none as the device parameter will disable SMS updates
        /// </summary>
        /// <param name="device">none or sms</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/update_delivery_device </remarks>
        public static async Task<TwitterSuccess> ChangeUpdateDeliverySettings(this IUserSession session, string device = "none")
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"device", device}
                             };

            return await session.PostAsync(TwitterApi.Resolve("/1.1/account/update_delivery_device.json"), parameters)
                         .ContinueWith(c => c.MapToTwitterSuccess()); 
        }

        /// <summary>
        /// Removes the uploaded profile banner for the authenticating user. Returns HTTP 200 upon success.
        /// </summary>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/remove_profile_banner </remarks>
        public static async Task<TwitterSuccess> DeleteProfileBanner(this IUserSession session)
        {
            var parameters = new TwitterParametersCollection();

            return await session.PostAsync(TwitterApi.Resolve("/1.1/account/remove_profile_banner.json"), parameters)
                         .ContinueWith(c => c.MapToTwitterSuccess());
        }

        /// <summary>
        /// Removes the uploaded profile banner for the authenticating user. Returns HTTP 20x upon success.
        /// </summary>
        /// <param name="fileName">file name for upload</param>
        /// <param name="imageContent">byte array of image content</param>
        /// <param name="banner_width">The width of the preferred section of the image being uploaded in pixels. Use with height, offset_left, and offset_top to select the desired region of the image to use.</param>
        /// <param name="banner_height">The height of the preferred section of the image being uploaded in pixels. Use with width, offset_left, and offset_top to select the desired region of the image to use.</param>
        /// <param name="offset_left">The number of pixels by which to offset the uploaded image from the left. Use with height, width, and offset_top to select the desired region of the image to use.</param>
        /// <param name="offset_top">The number of pixels by which to offset the uploaded image from the top. Use with height, width, and offset_left to select the desired region of the image to use.</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/update_profile_banner </remarks>
        public static async Task<TwitterSuccess> ChangeProfileBanner(this IUserSession session, string fileName, byte[] imageContent, int banner_width = 0, int banner_height = 0, int banner_left_offset = 0, int banner_top_offset = 0)
        {
            var parameters = new TwitterParametersCollection();

            if (banner_width != 0)
            {
                parameters.Add("width", banner_width.ToString());
            }

            if (banner_height != 0)
            {
                parameters.Add("height", banner_width.ToString());
            }

            if (banner_left_offset != 0)
            {
                parameters.Add("offset_left", banner_width.ToString());
            }

            if (banner_top_offset != 0)
            {
                parameters.Add("offset_top", banner_width.ToString());
            }

            return await session.PostFileAsync(TwitterApi.Resolve("/1.1/account/update_profile_banner.json"), parameters, fileName, "banner", imageContent)
                           .ContinueWith(c => c.MapToTwitterSuccess());
        }

        /// <summary>
        /// Removes the uploaded profile banner for the authenticating user. Returns HTTP 20x upon success.
        /// </summary>
        /// <param name="fileName">file name for upload</param>
        /// <param name="imageDataStream">Stream of image content</param>
        /// <param name="banner_width">The width of the preferred section of the image being uploaded in pixels. Use with height, offset_left, and offset_top to select the desired region of the image to use.</param>
        /// <param name="banner_height">The height of the preferred section of the image being uploaded in pixels. Use with width, offset_left, and offset_top to select the desired region of the image to use.</param>
        /// <param name="offset_left">The number of pixels by which to offset the uploaded image from the left. Use with height, width, and offset_top to select the desired region of the image to use.</param>
        /// <param name="offset_top">The number of pixels by which to offset the uploaded image from the top. Use with height, width, and offset_left to select the desired region of the image to use.</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/update_profile_banner </remarks>
        public static async Task<TwitterSuccess> ChangeProfileBanner(this IUserSession session, string fileName, Stream imageContentStream, int banner_width = 0, int banner_height = 0, int banner_left_offset = 0, int banner_top_offset = 0)
        {
            var parameters = new TwitterParametersCollection();

            if (banner_width != 0)
            {
                parameters.Add("width", banner_width.ToString());
            }

            if (banner_height != 0)
            {
                parameters.Add("height", banner_width.ToString());
            }

            if (banner_left_offset != 0)
            {
                parameters.Add("offset_left", banner_width.ToString());
            }

            if (banner_top_offset != 0)
            {
                parameters.Add("offset_top", banner_width.ToString());
            }

            return await session.PostFileAsync(TwitterApi.Resolve("/1.1/account/update_profile_banner.json"), parameters, fileName, "banner", srImage: imageContentStream)
                .ContinueWith(c => c.MapToTwitterSuccess());

       }

        /// <summary>
        /// Returns a map of the available size variations of the specified user's profile banner. 
        /// </summary>
        /// <param name="screen_name">The screen name of the user for whom to return results for.</param>
        /// <param name="user_id">The ID of the user for whom to return results for.</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/users/profile_banner </remarks>
        public static async Task<ProfileBanners> GetProfileBanners(this IUserSession session, string screen_name = "", long user_id = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities:true,skip_status:true,screen_name:screen_name,user_id:user_id);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<ProfileBanners>(
                        "Either screen_name or user_id required"); ;
            } 
            return await session.GetAsync(TwitterApi.Resolve("/1.1/users/profile_banner.json"), parameters)
                         .ContinueWith(c => c.MapToSingle<ProfileBanners>());
        }

        /// <summary>
        /// Returns a collection of user objects that the authenticating user is blocking.
        /// </summary>
        /// <param name="cursor">Causes the list of blocked users to be broken into pages of no more than 5000 IDs at a time.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/blocks/list </remarks>
        public static async Task<UserListDetailedCursored> GetBlockList(this IUserSession session, long cursor=-1)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(cursor:cursor);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/blocks/list.json"), parameters)
                         .ContinueWith(c => c.MapToSingle<UserListDetailedCursored>());
        }

        /// <summary>
        /// Blocks the specified user from following the authenticating user. In addition the blocked user will not show in the authenticating users mentions or timeline (unless retweeted by another user). If a follow or friend relationship exists it is destroyed.
        /// </summary>
        /// <param name="screen_name">The screen name of the potentially blocked user.</param>
        /// <param name="user_id">The ID of the potentially blocked user.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/blocks/create </remarks>
        public static async Task<User> CreateUserBlock(this IUserSession session, string screen_name = "", long user_id = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities:true,skip_status:true,screen_name:screen_name,user_id:user_id);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<User>(
                        "Either screen_name or user_id required"); ;
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/blocks/create.json"), parameters)
                         .ContinueWith(c => c.MapToSingle<User>());
        }

        /// <summary>
        /// Un-blocks the user specified in the ID parameter for the authenticating user. Returns the un-blocked user in the requested format when successful. If relationships existed before the block was instated, they will not be restored.
        /// </summary>
        /// <param name="screen_name">The screen name of the potentially blocked user.</param>
        /// <param name="user_id">The ID of the potentially blocked user.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/blocks/destroy </remarks>
        public static async Task<User> DeleteUserBlock(this IUserSession session, string screen_name = "", long user_id = 0)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true, skip_status: true, screen_name: screen_name, user_id: user_id);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<User>(
                        "Either screen_name or user_id required"); ;
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/blocks/destroy.json"), parameters)
                         .ContinueWith(c => c.MapToSingle<User>());
        }

        /// <summary>
        /// Returns fully-hydrated user objects for up to 100 users per request, as specified by comma-separated values passed to the user_id and/or screen_name parameters.
        /// </summary>
        /// <param name="screen_names">up to 100 are allowed in a single request.</param>
        /// <param name="user_ids">up to 100 are allowed in a single request. </param>
        /// <returns>Observable List of full user details</returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/users/lookup </remarks>
        public static async Task<TwitterResponseCollection<User>> GetUsersDetailsFull(this IUserSession session, List<string> screen_names = null,
            List<long> user_ids = null)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(include_entities: true);
            parameters.CreateCollection(screen_names:screen_names,user_ids:user_ids);

            if (parameters.EnsureEitherOr("screen_name", "user_id").IsFalse())
            {
                return session.MapParameterError<TwitterResponseCollection<User>>(
                        "Either screen_names or user_ids required"); ;
            }

            return await session.PostAsync(TwitterApi.Resolve("/1.1/users/lookup.json"), parameters).ContinueWith(c => c.MapToMany<User>());
        }

        /// <summary>
        /// Provides a simple, relevance-based search interface to public user accounts on Twitter. Only the first 1,000 matching results are available.
        /// </summary>
        /// <param name="searchQuery">The search query to run against people search.</param>
        /// <param name="count">The number of potential user results to retrieve per page. This value has a maximum of 20.</param>
        /// <param name="page">Specifies the page of results to retrieve.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/users/search </remarks>
        public async static Task<TwitterResponseCollection<User>> SearchForUsers(this IUserSession session, string searchQuery, int count = 20, int page = 1)
        {
            var parameters = new TwitterParametersCollection
                             {
                                 {"q", searchQuery},
                                 {"page", page.ToString()},
                             };
            parameters.Create(count:count,include_entities:true);

            return await session.GetAsync(TwitterApi.Resolve("/1.1/users/search.json"), parameters).ContinueWith(c => c.MapToMany<User>());
        }
    }
}
