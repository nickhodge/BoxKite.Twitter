using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        IObservable<DeleteEvent> DeleteEvents { get; }
        IObservable<StreamScrubGeo> ScrubGeoRequests { get; }
        IObservable<StreamLimitNotice> LimitNotices { get; }
        IObservable<StreamStatusWithheld> StatusWithheld { get; }
        IObservable<StreamUserWithheld> UserWithheld { get; }
        bool IsActive { get; set; } 
        void Start();
        void Stop();
    }
}