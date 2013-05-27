// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public partial class TwitterAccount : BindableBase
    {
        readonly Subject<Tweet> _timeline = new Subject<Tweet>();
        public IObservable<Tweet> TimeLine { get { return _timeline; } }
        public TweetsContainer HomeTimeLine { get; set; }

        public void AddToHomeTimeLine(Tweet t)
        {
            // only Publish if unique
            if(!HomeTimeLine.Contains(t.Id))
                _timeline.Publish(t);
        }

        public void Start()
        {
            PublicState = "Starting Up";
            //
            TwitterCommunication = new CancellationTokenSource();
            UserStream = Session.GetUserStream();
            UserStream.Tweets.Subscribe(AddToHomeTimeLine);

            HomeTimeLine = new TweetsContainer();
            HomeTimeLine.SubscribeTo(this.TimeLine);

            UserStream.Start();
            ProcessBackfillPump();
            //
            PublicState = "OK";
        }

        public void Stop()
        {
            PublicState = "Shutting Down";
            TwitterCommunication.Cancel();
            UserStream.Stop();
            PublicState = "";
        }

        private void ProcessBackfillPump()
        {
            Task.Factory.StartNew(GetHomeTimeLine_Backfill);           
        }

        private async void GetHomeTimeLine_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 100;
            int pagingSize = 10;

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

        // mentions = streamed + historical (need to know since_id)
        // rtofmine = streamed + historical (need to know since_id)
        // dmsofminerecd = streamed + historical (need to know since_id)
        // dmsofminesent = historical (need to know since_id)

        // later: lists
    }
}