// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BoxKite.Twitter.Tests
{
    public static class UserStreamExtensions
    {
        public static IUserStream GetUserStream(this IUserSession session)
        {
            Func<Task<HttpResponseMessage>> startConnection =
                () =>
                {
                    var resp = ((TestableUserSession)session).MakeResponse();
                    return resp;
                };

            return new UserStream(startConnection);
        }
    }
}