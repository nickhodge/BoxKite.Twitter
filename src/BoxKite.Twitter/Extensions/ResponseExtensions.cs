using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Models.Service;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Extensions
{
    internal static class ResponseExtensions
    {
        internal static TwitterResponseCollection<TwitterList> MapToTwitterLists(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new TwitterResponseCollection<TwitterList> { TwitterControlMessage = MapHTTPResponses(task)};
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterResponseCollection<TwitterList> { TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var twitterlists = JsonConvert.DeserializeObject<TwitterResponseCollection<TwitterList>>(content.Result);
            return twitterlists;
        }

        internal static TwitterList MapToTwitterList(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new TwitterList { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterList { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var twitterlist = JsonConvert.DeserializeObject<TwitterList>(content.Result);
            return twitterlist;
        }

        internal static UserStatus MapToUserStatus(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new UserStatus { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new UserStatus { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var ids = JsonConvert.DeserializeObject<dynamic>(content.Result);
            var userst = new UserStatus
                      {
                          Target = new UserStatusTarget
                                   {
                                       Id = ids.relationship.target.id,
                                       FollowedBy = ids.relationship.target.followed_by,
                                       ScreenName = ids.relationship.target.screen_name,
                                       Following = ids.relationship.target.following
                                   },
                          Source = new UserStatusSource
                                   {
                                       Id = ids.relationship.source.id,
                                       FollowedBy = ids.relationship.source.followed_by,
                                       ScreenName = ids.relationship.source.screen_name,
                                       Following = ids.relationship.source.following,
                                       CanDM = ids.relationship.source.can_dm,
                                       Blocking = ids.relationship.source.blocking,
                                       AllReplies = ids.relationship.source.all_replies,
                                       WantRetweets = ids.relationship.source.want_retweets,
                                       MarkedSpam = ids.relationship.source.marked_spam,
                                       NotificationsEnabled = ids.relationship.source.notifications_enabled
                                   }
                      };
            return userst;
        }

        internal static FriendsFollowersIDsCursored MapToFriendsFollowersIDsCursored(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new FriendsFollowersIDsCursored { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new FriendsFollowersIDsCursored { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var ffsids = JsonConvert.DeserializeObject<FriendsFollowersIDsCursored>(content.Result);
            return ffsids;
        }

        internal static UserInListCursored MapToUserInListCursored(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new UserInListCursored { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new UserInListCursored { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var userinlist = JsonConvert.DeserializeObject<UserInListCursored>(content.Result);
            return userinlist;
        }

        internal static TwitterListCursored MapToTwitterListCursored(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new TwitterListCursored { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterListCursored { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var twl = JsonConvert.DeserializeObject<TwitterListCursored>(content.Result);
            return twl;
        }

        internal static ApiRateStatusCall MapToApiRateLimits(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new ApiRateStatusCall { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new ApiRateStatusCall { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();

            var apiresp = JsonConvert.DeserializeObject<dynamic>(content.Result);
            var apiratelimit = new ApiRateStatusCall {ApiRateStatuses = new Dictionary<string, ApiRateStatus>()};

            //TODO: clean this up

            foreach (var i in apiresp.resources.lists)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.application)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus{apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset});
            }

            foreach (var i in apiresp.resources.friendships)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.blocks)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.geo)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.users)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.followers)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.statuses)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.help)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.friends)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.direct_messages)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.account)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.favorites)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.saved_searches)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.search)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.trends)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            return apiratelimit;
        }

        internal static AccountSettings MapToAccountSettings(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new AccountSettings { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new AccountSettings { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var accts = JsonConvert.DeserializeObject<AccountSettings>(content.Result);
            return accts;
        }

        internal static Place MapToPlace(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new Place { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return  new Place { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var placeinfo = JsonConvert.DeserializeObject<Place>(content.Result);
            return placeinfo;
        }

        internal static TwitterResponseCollection<TrendsForPlaceResponse> MapToTrendsForPlaceResponse(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new TwitterResponseCollection<TrendsForPlaceResponse> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterResponseCollection<TrendsForPlaceResponse> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var placeinfo = JsonConvert.DeserializeObject<TwitterResponseCollection<TrendsForPlaceResponse>>(content.Result);
            return placeinfo;
        }

        internal static TwitterResponseCollection<TrendsAvailableLocationsResponse> MapToTrendsAvailableLocationsResponse(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new TwitterResponseCollection<TrendsAvailableLocationsResponse> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
             }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterResponseCollection<TrendsAvailableLocationsResponse> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var placeinfo = JsonConvert.DeserializeObject<TwitterResponseCollection<TrendsAvailableLocationsResponse>>(content.Result);
            return placeinfo;
        }
        
        internal static ReverseGeoCodePlaces MapToPlaces(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new ReverseGeoCodePlaces { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new ReverseGeoCodePlaces { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var placeinfo = JsonConvert.DeserializeObject<ReverseGeoCodePlaces>(content.Result);
            return placeinfo;
        }

        internal static AddPlaceResponse MapToAddPlaceResponse(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new AddPlaceResponse { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new AddPlaceResponse { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var placeinfo = JsonConvert.DeserializeObject<AddPlaceResponse>(content.Result);
            return placeinfo;
        }

        internal static SuggestedUsers MapToUserSuggestions(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new SuggestedUsers { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new SuggestedUsers { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var suggusers = JsonConvert.DeserializeObject<SuggestedUsers>(content.Result);
            return suggusers;
        }

        internal static TwitterResponseCollection<SuggestedUsers> MapToSuggestions(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new TwitterResponseCollection<SuggestedUsers> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterResponseCollection<SuggestedUsers> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var suggusers = JsonConvert.DeserializeObject<TwitterResponseCollection<SuggestedUsers>>(content.Result);
            return suggusers;
        }

        internal static AccountProfile MapToAccountProfile(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new AccountProfile { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new AccountProfile { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var acctp = JsonConvert.DeserializeObject<AccountProfile>(content.Result);
            return acctp;
        }

        internal static UserListDetailedCursored MapToUserListDetailedCursored(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new UserListDetailedCursored { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new UserListDetailedCursored { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var userdlist = JsonConvert.DeserializeObject<UserListDetailedCursored>(content.Result);
            return userdlist;
        }

        internal static TwitterResponseCollection<FriendshipLookupResponse> MapToListOfFriendships(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new TwitterResponseCollection<FriendshipLookupResponse> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterResponseCollection<FriendshipLookupResponse> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var ffs = JsonConvert.DeserializeObject<TwitterResponseCollection<FriendshipLookupResponse>>(content.Result);
            return ffs;
        }

        internal static TwitterResponseCollection<long> MapToIds(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new TwitterResponseCollection<long> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterResponseCollection<long> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var ids = JsonConvert.DeserializeObject<TwitterResponseCollection<long>>(content.Result);
            return ids;
        }

        internal static User MapToSingleUser(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new User { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new User { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            } 
            
            var content = result.Content.ReadAsStringAsync();
            var singleuser = JsonConvert.DeserializeObject<User>(content.Result);
            return singleuser;
        }

        internal static Tweet MapToSingleTweet(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new Tweet { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new Tweet { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            } 

            var content = result.Content.ReadAsStringAsync();
            var singletweet = JsonConvert.DeserializeObject<Tweet>(content.Result);
            return singletweet;
        }

        internal static TwitterResponseCollection<Tweet> MapToListOfTweets(this Task<HttpResponseMessage> c)
        {
            if (c.IsFaulted || c.IsCanceled)
            {
                return new TwitterResponseCollection<Tweet> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(c) };
            }

            var result = c.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterResponseCollection<Tweet> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(c) };
            }

            var content = result.Content.ReadAsStringAsync();
            var manytweets = JsonConvert.DeserializeObject<TwitterResponseCollection<Tweet>>(content.Result);
            return manytweets;
        }

        internal static DirectMessage MapToSingleDirectMessage(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new DirectMessage { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new DirectMessage { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var singledm = JsonConvert.DeserializeObject<DirectMessage>(content.Result);
            return singledm;
        }

        internal static TwitterResponseCollection<DirectMessage> MapToListOfDirectMessages(this Task<HttpResponseMessage> c)
        {
            if (c.IsFaulted || c.IsCanceled)
            {
                return new TwitterResponseCollection<DirectMessage> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(c) };
             }

            var result = c.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterResponseCollection<DirectMessage> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(c) };
            }

            var content = result.Content.ReadAsStringAsync();
            var manydms = JsonConvert.DeserializeObject<TwitterResponseCollection<DirectMessage>>(content.Result);
            return manydms;
        }

        internal static TwitterResponseCollection<User> MapToListOfUsers(this Task<HttpResponseMessage> c)
        {
            if (c.IsFaulted || c.IsCanceled)
            {
                return new TwitterResponseCollection<User> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(c) };
            }

            var result = c.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterResponseCollection<User> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(c) };
            }

            var content = result.Content.ReadAsStringAsync();
            var manyusers = JsonConvert.DeserializeObject<TwitterResponseCollection<User>>(content.Result);
            return manyusers;
        }

        internal static SearchResponse MapToSearchResponse(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new SearchResponse { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new SearchResponse { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }  

            var content = result.Content.ReadAsStringAsync();
            var searchresponse = JsonConvert.DeserializeObject<SearchResponse>(content.Result);
            return searchresponse;
        }

        internal static SavedSearch MapToSavedSearch(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new SavedSearch { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new SavedSearch { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var searchresponse = JsonConvert.DeserializeObject<SavedSearch>(content.Result);
            return searchresponse;
        }

        internal static TwitterResponseCollection<SavedSearch> MapToListSavedSearches(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new TwitterResponseCollection<SavedSearch> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterResponseCollection<SavedSearch> { twitterFaulted = true, TwitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            var searchresponse = JsonConvert.DeserializeObject<TwitterResponseCollection<SavedSearch>>(content.Result);
            return searchresponse;
        }

        internal static TwitterSuccess MapToTwitterSuccess(this Task<HttpResponseMessage> c)
        {
            var resp = new TwitterSuccess { Status = false, TwitterControlMessage = MapHTTPResponses(c) };
            if (c.IsFaulted || c.IsCanceled)
                return resp;

            var result = c.Result;
            resp.Status = result.IsSuccessStatusCode;
            return resp;
        }

        // Map error results specially
        internal static TwitterControlMessage MapHTTPResponses(this Task<HttpResponseMessage> m)
        {
            var twitterControlMessage = new TwitterControlMessage();
            try
            {
                 var bodyContent = m.Result.Content.ReadAsStringAsync();
                /* {StatusCode: 429, ReasonPhrase: 'Too Many Requests', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:*/
                twitterControlMessage.http_status_code = IntValueForHTTPHeaderKey("StatusCode", m.Result.ToString());
                twitterControlMessage.twitter_rate_limit_limit = IntValueForHTTPHeaderKey("x-rate-limit-limit", m.Result.ToString());
                twitterControlMessage.twitter_rate_limit_remaining = IntValueForHTTPHeaderKey("x-rate-limit-remaining", m.Result.ToString());
                twitterControlMessage.twitter_rate_limit_reset = DateTimeForHTTPHeaderKey("x-rate-limit-reset", m.Result.ToString());

                if (twitterControlMessage.http_status_code == 429)
                    twitterControlMessage.twitter_error_message = "Rate Limit Exceeded";
                else
                    twitterControlMessage.http_reason = StringValueForHTTPHeaderKey("ReasonPhrase", m.Result.ToString());

            
                var errordetail = JsonConvert.DeserializeObject<TwitterError>(bodyContent.Result);
                twitterControlMessage.twitter_error_message = errordetail.errors[0].message;
                twitterControlMessage.twitter_error_code = errordetail.errors[0].code;
            }
            catch (Exception e)
            {
                twitterControlMessage.twitter_error_message = m.Result.ToString() + e.ToString();
            }

            return twitterControlMessage;
        }

        internal static int IntValueForHTTPHeaderKey(string key, string header)
        {
            var val = 0;
            var statuskey = new Regex(key + @":\s?(\d+)");
            var mc = statuskey.Matches(header);
            if (mc.Count > 0)
            {
                val = Int32.Parse(mc[0].Groups[1].Value);
            }
            return val;
        }

        internal static string StringValueForHTTPHeaderKey(string key, string header)
        {
            var val = "";
            var statuskey = new Regex(key + @":\s?'(.+)'");
            var mc = statuskey.Matches(header);
            if (mc.Count > 0)
            {
                val = mc[0].Groups[1].Value;
            }
            return val;
        }

        internal static DateTime DateTimeForHTTPHeaderKey(string key, string header)
        {
            double val = 0;
            // http://stackoverflow.com/questions/16621738/d-less-efficient-than-0-9
            var statuskey = new Regex(key + @":\s?([0123456789]{+})");
            var mc = statuskey.Matches(header);
            if (mc.Count > 0)
            {
                val = double.Parse(mc[0].Groups[1].Value);
            }
            
            return val.FromUnixEpochSeconds();
        }
    }
}