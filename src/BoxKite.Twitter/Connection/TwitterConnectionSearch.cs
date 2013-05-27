// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public partial class TwitterAccount : BindableBase
    {
        readonly Subject<Tweet> _searchtimeline = new Subject<Tweet>();
        public IObservable<Tweet> SearchTimeLine { get { return _searchtimeline; } }

        private CancellationTokenSource TwitterSearchCommunication;

        public void StartSearch(string textToSearch)
        {
            TwitterSearchCommunication = new CancellationTokenSource();
            //
            SearchStream = Session.StartSearchStream(track:textToSearch);
            SearchStream.FoundTweets.Subscribe(_searchtimeline.OnNext);
            SearchStream.Start();
            //
            //ProcessSearchBackFill_Pump();
        }

        public void StopSearch()
        {
            TwitterSearchCommunication.Cancel();
            SearchStream.Stop();
        }

        private void ProcessSearchBackFill_Pump()
        {
            Task.Factory.StartNew(GetSearchTimeLine_Backfill);
        }

        private async void GetSearchTimeLine_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 50;
            int pagingSize = 50;

            do
            {
                var timeline6 = await Session.GetHomeTimeline(count: pagingSize, max_id: smallestid);
                if (!timeline6.twitterFaulted)
                {
                    smallestid = long.MaxValue;
                    foreach (var tweet in timeline6)
                    {
                        AddToHomeTimeLine(tweet);
                        if (tweet.Id < smallestid) smallestid = tweet.Id;
                        if (tweet.Id > largestid) largestid = tweet.Id;
                        backfillQuota--;
                    }
                }
                else
                {
                    break;
                }
            } while (backfillQuota > 0);
        }

    }
}
