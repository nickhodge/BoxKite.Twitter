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
        readonly Subject<Tweet> _timeline = new Subject<Tweet>();
        public IObservable<Tweet> TimeLine { get { return _timeline; } }

        readonly Subject<Tweet> _mentions = new Subject<Tweet>();
        public IObservable<Tweet> Mentions { get { return _mentions; } }

        readonly Subject<DirectMessage> _directmessages = new Subject<DirectMessage>();
        public IObservable<DirectMessage> DirectMessages { get { return _directmessages; } }

        private List<long> _tweetsSeen = new List<long>();
        private CancellationTokenSource TwitterCommunication;

        public void AddToHomeTimeLine(Tweet t)
        {
            // only Publish if unique
            if (_tweetsSeen.Contains(t.Id)) return;
            _tweetsSeen.Add(t.Id);
            _timeline.OnNext(t);
        }

        public void Start()
        {
            PublicState = "Starting Up";
            TwitterCommunication = new CancellationTokenSource();
            //
            UserStream = Session.GetUserStream();
            UserStream.Tweets.Subscribe(AddToHomeTimeLine);
            UserStream.Tweets.Where(t => t.Text.ToLower().Contains(accountDetails.ScreenName.ToLower())).Subscribe(_mentions.OnNext);
            UserStream.Start();
            //
            UserStream.DirectMessages.Subscribe(_directmessages.OnNext);
            //
            ProcessBackfillPump();
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
            Task.Factory.StartNew(GetDirectMessages_Received_Backfill);
            Task.Factory.StartNew(GetDirectMessages_Sent_Backfill);
            Task.Factory.StartNew(GetMentions_Backfill);
        }

        private async void GetHomeTimeLine_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 50;
            int pagingSize = 50;

            do
            {
                var timeline6 = await Session.GetHomeTimeline(count: pagingSize, max_id: smallestid);
                if (timeline6.OK)
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

        private async void GetMentions_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 50;
            int pagingSize = 50;

            do
            {
                var timeline6 = await Session.GetMentions(count: pagingSize, max_id: smallestid);
                if (timeline6.OK)
                {
                    smallestid = long.MaxValue;
                    foreach (var tweet in timeline6)
                    {
                        _mentions.OnNext(tweet);
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

        private async void GetDirectMessages_Received_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 20;
            int pagingSize = 20;

            do
            {
                var timeline6 = await Session.GetDirectMessages(count: pagingSize, max_id: smallestid);
                if (timeline6.OK)
                {
                    smallestid = long.MaxValue;
                    foreach (var dm in timeline6)
                    {
                        _directmessages.OnNext(dm);
                        if (dm.Id < smallestid) smallestid = dm.Id;
                        if (dm.Id > largestid) largestid = dm.Id;
                        backfillQuota--;
                    }
                }
                else
                {
                    break;
                }
            } while (backfillQuota > 0);
        }

        private async void GetDirectMessages_Sent_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 20;
            int pagingSize = 20;

            do
            {
                var timeline6 = await Session.GetDirectMessagesSent(count: pagingSize, max_id: smallestid);
                if (timeline6.OK)
                {
                    smallestid = long.MaxValue;
                    foreach (var dm in timeline6)
                    {
                        _directmessages.OnNext(dm);
                        if (dm.Id < smallestid) smallestid = dm.Id;
                        if (dm.Id > largestid) largestid = dm.Id;
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