using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Modules.Streaming;

namespace BoxKite.Twitter.Tests.Modules
{
    public static class UserStreamExtensions
    {
        public static IUserStream GetUserStream(this IUserSession session)
        {
            Func<Task<HttpResponseMessage>> startConnection =
                () =>
                {
                    var resp = ((TestableSession)session).MakeResponse();
                    return resp;
                };

            return new BoxKite.Twitter.UserStream(startConnection);
        }
    }
}