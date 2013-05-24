// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Reactive.Subjects;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Modules.Streaming
{
    public interface ISearchStream
    {
        IObservable<Tweet> FoundTweets { get; }
        Subject<string> SearchRequests { get; }
        TwitterParametersCollection SearchParameters { get; set; }
        bool IsActive { get; }
        void Start();
    }
}