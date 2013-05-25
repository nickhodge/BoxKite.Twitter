// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public interface IUserStream : IDisposable
    {
        IObservable<Tweet> Tweets { get; }
        IObservable<IEnumerable<long>> Friends { get; }
        IObservable<DirectMessage> DirectMessages { get; }
        IObservable<StreamEvent> Events { get; }
        IObservable<DeleteEvent> DeleteEvents { get; }
        IObservable<StreamScrubGeo> ScrubGeoRequests { get; }
        IObservable<StreamLimitNotice> LimitNotices { get; }
        IObservable<StreamStatusWithheld> StatusWithheld { get; }
        IObservable<StreamUserWithheld> UserWithheld { get; }
        bool IsActive { get; } 
        void Start();
        void Stop();
    }
}