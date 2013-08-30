// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoxKite.Twitter.Extensions;

namespace BoxKite.Twitter.Models
{
    public class TwitterParametersCollection : SortedDictionary<string,string>
    {

    }

    internal static class ApiParameters
    {
        internal static void CreateCollection(this TwitterParametersCollection parameters,
            IEnumerable<string> screen_names = null, IEnumerable<long> user_ids = null)
        {
            var screenNameList = new StringBuilder();
            if (screen_names != null)
            {
                if (screen_names.Count() > 0)
                {
                    foreach (var screenName in screen_names)
                    {
                        screenNameList.Append(screenName + ",");
                    }
                    parameters.Add("screen_name", screenNameList.ToString().TrimLastChar());
                }
            }

            var userIDList = new StringBuilder();
            if (user_ids != null)
            {
                if (user_ids.Any())
                {
                    foreach (var userID in user_ids)
                    {
                        userIDList.Append(userID + ",");
                    }
                    parameters.Add("user_id", userIDList.ToString().TrimLastChar());
                }
            }
        }

        internal static void Create(this TwitterParametersCollection parameters,
            bool? include_entities = null, long? since_id = null, long? max_id = null, int? count = null,
            long? user_id = null, string screen_name = null, long? id = null, long? cursor = null,
            string text = null, bool? follow = null, bool? device = null, bool? retweets = null,
            bool? skip_status = null, string slug = null, int? list_id = null,string owner_screen_name = null,
            long? owner_id = null, string name = null, bool? include_rts = null, string place_id = null, bool? stall_warnings = null, bool? delimited = null
            )
        {
            if (stall_warnings != null)
            {
                parameters.Add("stall_warnings", stall_warnings.ToString());
            }

            if (delimited != null)
            {
                parameters.Add("delimited", delimited.ToString());
            }

            if (cursor != null)
            {
                parameters.Add("cursor", cursor.ToString());
            }

            if (id != null)
            {
                if (id != 0)
                {
                    parameters.Add("id", id.ToString());
                }
            }

            if (list_id != null)
            {
                if (list_id != 0)
                {
                    parameters.Add("list_id", list_id.ToString());
                }
            }

            if (since_id != null)
            {
                if (since_id != 0)
                {
                    parameters.Add("since_id", since_id.ToString());
                }
            }

            if (max_id != null)
            {
                if (max_id != 0)
                {
                    parameters.Add("max_id", max_id.ToString());
                }
            }

            if (count != null)
            {
                if (count != 0)
                {
                    parameters.Add("count", count.ToString());
                }
            }

            if (user_id != null)
            {
                if (user_id != 0)
                {
                    parameters.Add("user_id", user_id.ToString());
                }
            }

            if (owner_id != null)
            {
                if (owner_id != 0)
                {
                    parameters.Add("owner_id", owner_id.ToString());
                }
            }

            if (include_rts != null)
            {
                parameters.Add("include_rts", include_rts.ToString());
            }

            if (include_entities != null)
            {
                parameters.Add("include_entities", include_entities.ToString());
            }

            if (follow != null)
            {
                parameters.Add("follow", follow.ToString());
            }

            if (skip_status != null)
            {
                parameters.Add("skip_status", skip_status.ToString());
            }

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            if (!string.IsNullOrWhiteSpace(place_id))
            {
                parameters.Add("place_id", place_id);
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                parameters.Add("name", name);
            }

            if (!string.IsNullOrWhiteSpace(slug))
            {
                parameters.Add("slug", slug);
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                parameters.Add("text", text);
            }
        }

        internal static void CreateCommaDelimitedList(this TwitterParametersCollection parameters, string elementName,
            IEnumerable elements)
        {
            var elementstr = new StringBuilder();
            if (elements == null) return;
            var elementcount = 0;
            foreach (var tr in elements)
            {
                elementstr.Append(tr + ",");
                elementcount++;
            }
            if (elementcount > 0)
                parameters.Add(elementName, elementstr.ToString().TrimLastChar());
        }

        internal static bool EnsureOneOf(this TwitterParametersCollection parameters, IEnumerable<string> requiredParams)
        {
            return requiredParams.Any(parameters.ContainsKey);
        }

        internal static bool EnsureIsPresent(this TwitterParametersCollection parameters, string paramA)
        {
            return parameters.ContainsKey(paramA);
        }

        internal static bool EnsureAllArePresent(this TwitterParametersCollection parameters, IEnumerable<string> requiredParams)
        {
            return requiredParams.All(parameters.ContainsKey);
        }

        internal static bool EnsureAnd(this TwitterParametersCollection parameters, string paramA, string paramB)
        {
            return parameters.ContainsKey(paramA) && parameters.ContainsKey(paramB);
        }

        internal static bool EnsureEitherOr(this TwitterParametersCollection parameters, string paramA, string paramB)
        {
            return parameters.ContainsKey(paramA) || parameters.ContainsKey(paramB);
        }

        internal static bool IsFalse(this bool predicate)
        {
            return !predicate;
        }
    }
}
