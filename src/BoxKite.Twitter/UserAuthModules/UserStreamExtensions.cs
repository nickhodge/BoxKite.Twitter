// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using Reactive.EventAggregator;

namespace BoxKite.Twitter
{
    public static class UserStreamExtensions
    {
        public static IUserStream GetUserStream(this IUserSession session, IEventAggregator _eventAggregator)
        {
            Func<Task<HttpResponseMessage>> startConnection =
                () =>
                {
                    var parameters = new SortedDictionary<string, string>();
                    var request = session.CreateGet(Api.UserStreaming("/1.1/user.json"), parameters);
                    var c = new HttpClient() { Timeout = TimeSpan.FromDays(1) };
                    return c.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                };

            return new UserStream(startConnection, _eventAggregator);
        }
    }
}
