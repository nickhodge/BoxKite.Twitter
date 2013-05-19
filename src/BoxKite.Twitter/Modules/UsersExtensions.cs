using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Models.Service;
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
            var parameters = new SortedDictionary<string, string>();
            var url = Api.Resolve("/1.1/account/settings.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToAccountSettings());
        }

        /// <summary>
        /// Returns a variety of information about the user specified by the required user_id or screen_name parameter. The author's most recent Tweet will be returned inline when possible.
        /// </summary>
        /// <param name="screen_name">The screen name of the user for whom to return results for. Either a id or screen_name is required for this method.</param>
        /// <param name="user_id">The ID of the user for whom to return results for. Either an id or screen_name is required for this method.</param>
        /// <returns></returns>
        public static async Task<User> GetProfile(this IUserSession session, string screen_name="", int user_id=0)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"include_entities", "true"},
                                 };

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            var url = Api.Resolve("/1.1/users/show.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToSingleUser());
        }

        /// <summary>
        /// Returns an HTTP 200 OK response code and a representation of the requesting user if authentication was successful; returns a 401 status code and an error message if not. Use this method to test if supplied user credentials are valid.
        /// </summary>
        /// <returns></returns>
        public static async Task<User> GetVerifyCredentials(this IUserSession session) 
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"include_entities", "true"},
                                 };
            var url = Api.Resolve("/1.1/account/verify_credentials.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToSingleUser());
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
            var parameters = new SortedDictionary<string, string>
                             {
                                 {"include_entities","false"},
                                 {"sleep_time_enabled", sleep_time_enabled.ToString()},
                             };

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

            var url = Api.Resolve("/1.1/account/settings.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToAccountSettings());
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
            string purl =
                "", string location = "", string description = "")
        {
            var parameters = new SortedDictionary<string, string>
                             {
                                 {"include_entities","false"},
                                 {"skip_status", true.ToString()},
                             };

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

            var url = Api.Resolve("/1.1/account/update_profile.json");
            return await session.PostAsync(url, parameters)
                .ContinueWith(c => c.MapToAccountProfile());
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
            var parameters = new SortedDictionary<string, string>
                             {
                                 {"tile", tile.ToString()},
                                 {"use", useImage.ToString()},
                             };

            var url = Api.Resolve("/1.1/account/update_profile_background_image.json");
            return await session.PostFileAsync(url, parameters, fileName, imageContent, "image")
                           .ContinueWith(c => c.MapToAccountProfile());
        }

        
        /// <summary>
        /// Updates the authenticating user's profile image.
        /// </summary>
        /// <param name="fileName">file name for upload</param>
        /// <param name="imageContent">byte array of image content</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/update_profile_image </remarks> </remarks>
        public static async Task<AccountProfile> ChangeAccountProfileImage(this IUserSession session, string fileName, byte[] imageContent)
        {
            var parameters = new SortedDictionary<string, string>();

            var url = Api.Resolve("/1.1/account/update_profile_image.json");
            return await session.PostFileAsync(url, parameters, fileName, imageContent, "image")
                           .ContinueWith(c => c.MapToAccountProfile());
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
            var parameters = new SortedDictionary<string, string>
                             {
                                 {"include_entities","false"},
                             };

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

            var url = Api.Resolve("/1.1/account/update_profile_colors.json");
            return await session.PostAsync(url, parameters)
                           .ContinueWith(c => c.MapToAccountProfile());
        }

        /// <summary>
        /// Sets which device Twitter delivers updates to for the authenticating user. Sending none as the device parameter will disable SMS updates
        /// </summary>
        /// <param name="device">none or sms</param>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/update_delivery_device </remarks>
        public static async Task<bool> ChangeUpdateDeliverySettings(this IUserSession session, string device = "none")
        {
            var parameters = new SortedDictionary<string, string>
                            {
                                {"device", device},
                            };

            var url = Api.Resolve("/1.1/account/update_delivery_device.json");
            return await session.PostAsync(url, parameters)
                         .ContinueWith(c => c.MapToBoolean()); 
        }

        /// <summary>
        /// Removes the uploaded profile banner for the authenticating user. Returns HTTP 200 upon success.
        /// </summary>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/account/remove_profile_banner </remarks>
        public static async Task<bool> DeleteProfileBanner(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>();

            var url = Api.Resolve("/1.1/account/remove_profile_banner.json");
            return await session.PostAsync(url, parameters)
                         .ContinueWith(c => c.MapToBoolean());
        }

        /// <summary>
        /// Returns a collection of user objects that the authenticating user is blocking.
        /// </summary>
        /// <param name="cursor">Causes the list of blocked users to be broken into pages of no more than 5000 IDs at a time.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/blocks/list </remarks>
        public static async Task<UserListDetailedCursored> GetBlockList(this IUserSession session, long cursor=-1)
        {
            var parameters = new SortedDictionary<string, string>{
                                {"cursor", cursor.ToString()},
                            };

            var url = Api.Resolve("/1.1/blocks/list.json");
            return await session.GetAsync(url, parameters)
                         .ContinueWith(c => c.MapToUserListDetailedCursored());
        }

        /// <summary>
        /// Blocks the specified user from following the authenticating user. In addition the blocked user will not show in the authenticating users mentions or timeline (unless retweeted by another user). If a follow or friend relationship exists it is destroyed.
        /// </summary>
        /// <param name="screen_name">The screen name of the potentially blocked user.</param>
        /// <param name="user_id">The ID of the potentially blocked user.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/blocks/create </remarks>
        public static async Task<User> CreateBlock(this IUserSession session, string screen_name = "", int user_id = 0)
        {
            var parameters = new SortedDictionary<string, string>{
                                {"include_entities", false.ToString()},
                                {"skip_status", true.ToString()},
                            };

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            if (user_id != 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            var url = Api.Resolve("/1.1/blocks/create.json");
            return await session.PostAsync(url, parameters)
                         .ContinueWith(c => c.MapToSingleUser());
        }

        /// <summary>
        /// Un-blocks the user specified in the ID parameter for the authenticating user. Returns the un-blocked user in the requested format when successful. If relationships existed before the block was instated, they will not be restored.
        /// </summary>
        /// <param name="screen_name">The screen name of the potentially blocked user.</param>
        /// <param name="user_id">The ID of the potentially blocked user.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/blocks/destroy </remarks>
        public static async Task<User> DeleteBlock(this IUserSession session, string screen_name = "", int user_id = 0)
        {
            var parameters = new SortedDictionary<string, string>{
                                {"include_entities", false.ToString()},
                                {"skip_status", true.ToString()},
                            };

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            if (user_id != 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            var url = Api.Resolve("/1.1/blocks/destroy.json");
            return await session.PostAsync(url, parameters)
                         .ContinueWith(c => c.MapToSingleUser());
        }

        /// <summary>
        /// Returns fully-hydrated user objects for up to 100 users per request, as specified by comma-separated values passed to the user_id and/or screen_name parameters.
        /// </summary>
        /// <param name="screen_names">up to 100 are allowed in a single request.</param>
        /// <param name="user_ids">up to 100 are allowed in a single request. </param>
        /// <returns>Observable List of full user details</returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/users/lookup </remarks>
        public async static Task<IEnumerable<User>> GetFullUserDetails(this IUserSession session, IEnumerable<string> screen_names,
            IEnumerable<int> user_ids)
        {
            var parameters = new SortedDictionary<string, string>
                             {
                                 {"include_entities", false.ToString()},
                             };

            var postscreennames = new StringBuilder();
            var postuserids = new StringBuilder();
            if (screen_names.Any())
            {
                foreach (var screenname in screen_names)
                {
                    postscreennames.Append(screenname + ",");
                }
                parameters.Add("screen_name", postscreennames.ToString().TrimLastChar());
            } else if (user_ids.Any())
            {
                foreach (var ids in user_ids)
                {
                    postscreennames.Append(ids + ",");
                }
                parameters.Add("user_id", postuserids.ToString().TrimLastChar());
            }

            var url = Api.Resolve("/1.1/users/lookup.json");
            return await session.PostAsync(url, parameters).ContinueWith(c => c.MapToListOfUsers());
        }

        /// <summary>
        /// Provides a simple, relevance-based search interface to public user accounts on Twitter. Only the first 1,000 matching results are available.
        /// </summary>
        /// <param name="searchQuery">The search query to run against people search.</param>
        /// <param name="count">The number of potential user results to retrieve per page. This value has a maximum of 20.</param>
        /// <param name="page">Specifies the page of results to retrieve.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/users/search </remarks>
        public async static Task<IEnumerable<User>> SearchForUsers(this IUserSession session, string searchQuery, int count=20, int page=1)
        {
            var parameters = new SortedDictionary<string, string>
                             {
                                 {"include_entities", false.ToString()},
                                 {"q", searchQuery},
                                 {"page", page.ToString()},
                                 {"count", count.ToString()}
                             };

            var url = Api.Resolve("/1.1/users/search.json");
            return await session.GetAsync(url, parameters).ContinueWith(c => c.MapToListOfUsers());
        }
    }
}
