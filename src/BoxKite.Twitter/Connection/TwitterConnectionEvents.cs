// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public partial class TwitterAccount : BindableBase
    {
        readonly Subject<StreamEvent> _streamevents = new Subject<StreamEvent>();
        public IObservable<StreamEvent> StreamEvents { get { return _streamevents; } }

        private void StartStreamEvents()
        {
            UserStream.Events.Subscribe(_streamevents.OnNext);
            //
            Task.Factory.StartNew(ProcessEventsFill_Pump);
        }

        private void ProcessEventsFill_Pump()
        {
            
        }

        private async void GetFriendshipRequestsIncoming_Backfill()
        {
            long nextcursor = -1;
            var ff2ListCount = 0;

            do
            {
                var ff2List = await Session.GetFriendshipRequestsIncoming(cursor: nextcursor);
                if (ff2List.twitterFaulted)
                {
                    break;
                };
                nextcursor = ff2List.next_cursor;
                foreach (var l in ff2List.IDs)
                {
                    ConsoleOutput.PrintMessage(String.Format("User ID: {0}", l));
                }
            } while (nextcursor != 0);
        }

    }
}
