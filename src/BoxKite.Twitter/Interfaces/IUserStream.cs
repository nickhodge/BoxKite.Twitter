using System;
using System.Collections.Generic;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Models.Stream;

namespace BoxKite.Twitter.Modules.Streaming
{
    public interface IUserStream : IDisposable
    {
        IObservable<Tweet> Tweets { get; }
        IObservable<IEnumerable<long>> Friends { get; }
        IObservable<DirectMessage> DirectMessages { get; }
        IObservable<StreamEvent> Events { get; }
        bool IsActive { get; set; } 
        void Start();
        void Stop();
    }
}