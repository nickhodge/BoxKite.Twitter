// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public partial class TwitterConnection
    {
        readonly Subject<IStreamEvent> _streamevents = new Subject<IStreamEvent>();
        public IObservable<IStreamEvent> StreamEvents { get { return _streamevents; } }

        private void StartStreamEvents()
        {
            UserStream.Events.Subscribe(_streamevents.OnNext);
            //
            Task.Factory.StartNew(ProcessEventsFill_Pump);
        }

        private void ProcessEventsFill_Pump()
        {
            // not implemented in public twitter api
        }

    }
}
