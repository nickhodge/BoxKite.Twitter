// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Models.Stream;

namespace BoxKite.Twitter.Modules.Streaming
{
    public interface ISearchStream
    {
        IObservable<Tweet> FoundTweets { get; }
        Subject<StreamSearchRequest> SearchRequests { get; }
        TwitterParametersCollection SearchParameters { get; set; }
        Func<Task<HttpResponseMessage>> CreateOpenConnection { get; set; }
        bool IsActive { get; }
        void Start();
    }
}