using System;
using System.Collections.Generic;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Modules.Streaming;

namespace BoxKite.Twitter.Tests.Modules
{
    public class TestableStream : IUserStream
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IObservable<Tweet> Tweets { get; private set; }
        public IObservable<IEnumerable<long>> Friends { get; private set; }
        public IObservable<DirectMessage> DirectMessages { get; private set; }
        public IObservable<Event> Events { get; private set; }
        public bool IsActive { get; set; }
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}