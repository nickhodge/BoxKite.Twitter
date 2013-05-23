using System;
using System.Dynamic;
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