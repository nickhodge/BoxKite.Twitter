// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public static class SearchStreamExtensions
    {
        
        public static ISearchStream StartSearchStream(this IUserSession session, string track=null, string follow=null, string locations=null)
        {
             var searchStream = new SearchStream(session);
            searchStream.SearchParameters = searchStream.ChangeSearchParameters(track, follow, locations);
            Func<Task<HttpResponseMessage>> startConnection = () =>
            {
                if (searchStream.SearchParameters.EnsureOneOf(new[] { "track", "follow", "locations" }).IsFalse())
                    return null;
                var request = session.CreatePost(Api.SearchStreaming("/1.1/statuses/filter.json"),
                    searchStream.SearchParameters);
                var c = new HttpClient { Timeout = TimeSpan.FromDays(1) };
                return c.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, searchStream.CancelSearchStream.Token);

            };
            searchStream.CreateOpenConnection = startConnection;
            return searchStream;
        }

        public static ISearchStream StartSearchStream(this IUserSession session, IEnumerable<string> track = null, IEnumerable<string> follow = null, IEnumerable<string> locations = null)
        {
            var searchStream = new SearchStream(session);
            searchStream.SearchParameters = searchStream.ChangeSearchParameters(track, follow, locations);
            Func<Task<HttpResponseMessage>> startConnection = () =>
            {
                if (searchStream.SearchParameters.EnsureOneOf(new[] { "track", "follow", "locations" }).IsFalse())
                    return null;
                var request = session.CreatePost(Api.SearchStreaming("/1.1/statuses/filter.json"),
                    searchStream.SearchParameters);
                var c = new HttpClient { Timeout = TimeSpan.FromDays(1) };
                return c.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, searchStream.CancelSearchStream.Token);

            };
            searchStream.CreateOpenConnection = startConnection;
            return searchStream;
        }
    }
}
