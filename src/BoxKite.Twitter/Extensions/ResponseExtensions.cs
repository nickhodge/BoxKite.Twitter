using System;
using System.Collections.Generic;
using System.Reflection;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models.Service;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Extensions
{
    internal static class ResponseExtensions
    {
        // special case decodes
        internal static ApiRateStatusCall MapToApiRateLimits(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new ApiRateStatusCall { twitterFaulted = true, twitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new ApiRateStatusCall { twitterFaulted = true, twitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();

            var apiresp = JsonConvert.DeserializeObject<dynamic>(content.Result);
            var apiratelimit = new ApiRateStatusCall { ApiRateStatuses = new Dictionary<string, ApiRateStatus>() };

            //TODO: clean this up

            foreach (var i in apiresp.resources.lists)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
            }

            foreach (var i in apiresp.resources.application)
            {
                apiratelimit.ApiRateStatuses.Add(i.Name, new ApiRateStatus { apipath = i.Name, remaining = i.Value.remaining, limit = i.Value.limit, reset = i.Value.reset });
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

        internal static TwitterSuccess MapToTwitterSuccess(this Task<HttpResponseMessage> task)
        {
            var resp = new TwitterSuccess { Status = false, twitterControlMessage = MapHTTPResponses(task) };
            if (task.IsFaulted || task.IsCanceled)
                return resp;

            var result = task.Result;
            resp.Status = result.IsSuccessStatusCode;
            return resp;
        }

        internal static UserStatus MapToUserStatus(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new UserStatus { twitterFaulted = true, twitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new UserStatus { twitterFaulted = true, twitterControlMessage = MapHTTPResponses(task) };
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

        // special generic case
        internal static TwitterResponseCollection<T> MapToMany<T>(this Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return new TwitterResponseCollection<T> { twitterFaulted = true, twitterControlMessage = MapHTTPResponses(task) };
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                return new TwitterResponseCollection<T> { twitterFaulted = true, twitterControlMessage = MapHTTPResponses(task) };
            }

            var content = result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TwitterResponseCollection<T>>(content.Result);
        }

        internal static T MapToSingle<T>(this Task<HttpResponseMessage> task) where T : new()
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                var singleError = new T();
                var boolType = new Type[] { typeof(bool) };
                var statusProperty = singleError.GetType().GetRuntimeMethod("set_twitterFaulted", boolType);
                statusProperty.Invoke(singleError, new object[] { true });

                var tcmType = new Type[] { typeof(TwitterControlMessage) };
                var tcmProperty = singleError.GetType().GetRuntimeMethod("set_twitterControlMessage", tcmType);
                tcmProperty.Invoke(singleError, new object[] { MapHTTPResponses(task) });

                return singleError;
            }

            var result = task.Result;
            if (!result.IsSuccessStatusCode)
            {
                var singleError = new T();
                var boolType = new Type[] {typeof (bool)};
                var statusProperty = singleError.GetType().GetRuntimeMethod("set_twitterFaulted", boolType);
                statusProperty.Invoke(singleError, new object[] {true});

                var tcmType = new Type[] { typeof(TwitterControlMessage) };
                var tcmProperty = singleError.GetType().GetRuntimeMethod("set_twitterControlMessage", tcmType);
                tcmProperty.Invoke(singleError, new object[] { MapHTTPResponses(task) });

                return singleError;
            }

            var content = result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content.Result);
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