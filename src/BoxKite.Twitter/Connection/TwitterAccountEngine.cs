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
        // controls
        private const int maxbackoff = 450;
        private const int pagingSize = 50;
        private const int sinceIdPagingSize = 200;
        //

        // status bits
        private readonly Subject<bool> backFillCompleted = new Subject<bool>();
        private readonly Subject<bool> userStreamConnected = new Subject<bool>();
        //

        // largestSeenIds
        // TBD: these should/could be properties on Account so they can be persisted across launches
        private long homeTimeLineLargestSeenId;
        private long directMessagesReceivedLargestSeenId;
        private long directMessagesSentLargestSeenId;
        private long retweetsOfMeLargestSeenId;
        private long mentionsOfMeLargestSeenId;
        private long myTweetsLargestSeenId;
        //

        readonly Subject<Tweet> _timeline = new Subject<Tweet>();
        public IObservable<Tweet> TimeLine { get { return _timeline; } }

        readonly Subject<Tweet> _retweetsOnTimeline = new Subject<Tweet>();
        public IObservable<Tweet> RetweetsOnTimeLine { get { return _retweetsOnTimeline; } }

        readonly Subject<Tweet> _mentions = new Subject<Tweet>();
        public IObservable<Tweet> Mentions { get { return _mentions; } }

        readonly Subject<Tweet> _mytweets = new Subject<Tweet>();
        public IObservable<Tweet> MyTweets { get { return _mytweets; } }

        readonly Subject<DirectMessage> _directmessages = new Subject<DirectMessage>();
        public IObservable<DirectMessage> DirectMessages { get { return _directmessages; } }

        readonly Subject<User> _usersseen = new Subject<User>();
        public IObservable<User> UsersSeen { get { return _usersseen; } }

        readonly Subject<DeleteEventStatus> _streamdeleteevent = new Subject<DeleteEventStatus>();
        public IObservable<DeleteEventStatus> StreamDeleteEvent { get { return _streamdeleteevent; } }

        private readonly List<long> _tweetIdsRegister = new List<long>();
        readonly Subject<long> _tweetsseen = new Subject<long>();
        public IObservable<long> TweetsSeen { get { return _tweetsseen; } }

        /*
         * NOTE: scrubgeo, statuswithheld, limitnotices, userwithheld and friends stream currently
         * not .Subscribed to from the UserStream
         */

        private CancellationTokenSource TwitterCommunication;

        private void AddToHomeTimeLine(Tweet t)
        {
            // only Publish if unique
            if (_tweetIdsRegister.Contains(t.Id)) return;
            _tweetIdsRegister.Add(t.Id);
            _tweetsseen.OnNext(t.Id);
            _timeline.OnNext(t);
            _usersseen.OnNext(t.User);
        }

        public void Start()
        {
            TwitterCommunication = new CancellationTokenSource();
            //
            UserStream = Session.GetUserStream(_eventAggregator);

            // this watches the userstream, if there is a disconnect event, do something about it
            _eventAggregator.GetEvent<TwitterUserStreamDisconnectEvent>().Subscribe(ManageUserStreamDisconnect);

            // Separate stream events start 
            StartStreamEvents();

            StartUserStreams();
           
            // MORE MAGIC HAPPENS HERE
            // The Userstreams only get tweets/direct messages from the point the connection is opened. 
            // Historical tweets/direct messages have to be gathered using the traditional paging/cursoring APIs
            // (Request/Response REST).
            // but the higher level client doesnt want to worry about all that complexity.
            // in the BackfillPump, we gather these tweets/direct messages and pump them into the correct Observable
            Task.Factory.StartNew(ProcessBackfillPump);

            userStreamConnected.Where(status => status.IsFalse()).Subscribe(StartPollingUpdates);
        }

        public void Stop()
        {
            TwitterCommunication.Cancel();
            UserStream.Stop();
            userStreamConnected.OnNext(false);
        }

        private void StartUserStreams()
        {
            // All tweets to the HomeTimeLine
            UserStream.Tweets.Subscribe(AddToHomeTimeLine);

            // MAGIC HAPPENS HERE
            // there is no specific "catch" for mentions in the Userstream, but here we can fake it!
            // Using LINQ, we can ask RX to show us incoming tweets that contain the screen name of the current user
            // then push this into the "Mentions" Observable
            UserStream.Tweets.Where(t => t.Text.ToLower().Contains(accountDetails.ScreenName.ToLower())).Subscribe(_mentions.OnNext);

            // specifically grab the Retweets in the timeline and show them
            // obviously this can be added at a higher level, too.
            UserStream.Tweets.Where(t => t.RetweetedStatus != null).Subscribe(_retweetsOnTimeline.OnNext);

            // Treat Direct Messages separately
            UserStream.DirectMessages.Subscribe(_directmessages.OnNext);

            // Pull out my tweets, and publish separately
            UserStream.Tweets.Where(t => t.User.UserId == accountDetails.UserId).Subscribe(_mytweets.OnNext);

            // also grab the delete events and publish
            UserStream.DeleteEvents.Subscribe(de => _streamdeleteevent.OnNext(de.DeleteEventStatus));

            UserStream.Start();
            userStreamConnected.OnNext(true);
        }

        private void ManageUserStreamDisconnect(TwitterUserStreamDisconnectEvent disconnectEvent)
        {
            userStreamConnected.OnNext(false); // push message saying userStream is no longer connected
        }

        private void StartPollingUpdates(bool status)
        {
            backFillCompleted.Where(st => st == true).Subscribe(s =>
            {
                var observable = Observable.Timer(TimeSpan.FromMinutes(1));
                observable.Subscribe(async t =>
                {
                    homeTimeLineLargestSeenId = await GetHomeTimeLine_Failover(homeTimeLineLargestSeenId);
                    directMessagesReceivedLargestSeenId = await GetDirectMessages_Received_Failover(directMessagesReceivedLargestSeenId);
                    directMessagesSentLargestSeenId = await GetDirectMessages_Sent_Failover(directMessagesSentLargestSeenId);
                    retweetsOfMeLargestSeenId = await GetRTOfMe_Failover(retweetsOfMeLargestSeenId);
                    mentionsOfMeLargestSeenId = await GetMentions_Failover(mentionsOfMeLargestSeenId);
                    myTweetsLargestSeenId = await GetMyTweets_Failover(myTweetsLargestSeenId);
                });
            });
        }


        // FAIL-OVER TO PULL REQUESTS
        //TODO: DRY these methods with <Func> goodness

        private async Task<long> GetHomeTimeLine_Failover(long sinceid)
        {
            long largestseenid = 0;

            var hometl = await Session.GetHomeTimeline(count: sinceIdPagingSize, since_id: sinceid);
            if (!hometl.OK) return largestseenid;
            foreach (var tweet in hometl)
            {
                AddToHomeTimeLine(tweet);
                if (tweet.Id > sinceid) largestseenid = tweet.Id;
            }
            return largestseenid;
        }

        private async Task<long> GetMentions_Failover(long sinceid)
        {
            long largestseenid = 0;

            var mentionsofme = await Session.GetMentions(count: sinceIdPagingSize, since_id: sinceid);
            if (!mentionsofme.OK) return largestseenid;
            foreach (var tweet in mentionsofme)
            {
                _mentions.OnNext(tweet);
                if (tweet.Id > sinceid) largestseenid = tweet.Id;
            }
            return largestseenid;
        }

        private async Task<long> GetRTOfMe_Failover(long sinceid)
        {
            long largestseenid = 0;

            var rtofme = await Session.GetRetweetsOfMe(count: pagingSize, since_id: sinceid);
            if (!rtofme.OK) return largestseenid;
            foreach (var tweet in rtofme)
            {
                _mentions.OnNext(tweet);
                if (tweet.Id > sinceid) largestseenid = tweet.Id;
            }
            return largestseenid;
        }

        private async Task<long> GetDirectMessages_Received_Failover(long sinceid)
        {
            long largestseenid = 0;

            var dmrecd = await Session.GetDirectMessages(count: pagingSize, since_id: sinceid);
            if (!dmrecd.OK) return largestseenid;
            foreach (var dm in dmrecd)
            {
                _directmessages.OnNext(dm);
                if (dm.Id > sinceid) largestseenid = dm.Id;
            }
            return largestseenid;
        }

        private async Task<long> GetDirectMessages_Sent_Failover(long sinceid)
        {
            long largestseenid = 0;
            
            var mysentdms = await Session.GetDirectMessagesSent(count: pagingSize, since_id: sinceid);
            if (!mysentdms.OK) return largestseenid;

            foreach (var dm in mysentdms)
            {
                _directmessages.OnNext(dm);
                if (dm.Id > sinceid) largestseenid = dm.Id;
            }
            return largestseenid;
        }

        private async Task<long> GetMyTweets_Failover(long sinceid)
        {
           long largestseenid = 0;
            
            var hometl = await Session.GetUserTimeline(user_id: accountDetails.UserId, count: pagingSize, since_id: sinceid);
            if (!hometl.OK) return largestseenid;

            foreach (var tweet in hometl)
            {
                _mytweets.OnNext(tweet);
                if (tweet.Id > largestseenid) largestseenid = tweet.Id;
            }
            return largestseenid;
        }



        // BACKFILLS
        private async void ProcessBackfillPump()
        {
            homeTimeLineLargestSeenId = await GetHomeTimeLine_Backfill();
            directMessagesReceivedLargestSeenId = await GetDirectMessages_Received_Backfill();
            directMessagesSentLargestSeenId = await GetDirectMessages_Sent_Backfill();
            retweetsOfMeLargestSeenId = await GetRTOfMe_Backfill();
            mentionsOfMeLargestSeenId = await GetMentions_Backfill();
            myTweetsLargestSeenId = await GetMyTweets_Backfill();
            backFillCompleted.OnNext(true);
        }

        // these grab tweets/dms from history rather from the current stream
        //TODO: DRY these methods with <Func> goodness
        private async Task<long> GetHomeTimeLine_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 50;
            int backofftimer = 30;

            do
            {
                var hometl = await Session.GetHomeTimeline(count: pagingSize, max_id: smallestid);
                if (hometl.OK)
                {
                    smallestid = long.MaxValue;
                    foreach (var tweet in hometl)
                    {
                        AddToHomeTimeLine(tweet);
                        if (tweet.Id < smallestid) smallestid = tweet.Id;
                        if (tweet.Id > largestid) largestid = tweet.Id;
                        backfillQuota--;
                    }
                    await Task.Delay(_multiFetchBackoffTimer);
                }
                else
                {
                    // The Backoff will trigger 7 times before just giving up
                    // once at 30s, 60s, 1m, 2m, 4m, 8m and then 16m
                    // note that the last call into this will be 1m above the 15 "rate limit reset" window 
                    Task.Delay(TimeSpan.FromSeconds(backofftimer));
                    if (backofftimer > maxbackoff)
                        break;
                    backofftimer = backofftimer * 2;
                }
            } while (backfillQuota > 0);
            return largestid;
        }

        private async Task<long> GetMentions_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 50;
            int backofftimer = 30;

            do
            {
                var mentionsofme = await Session.GetMentions(count: pagingSize, max_id: smallestid);
                if (mentionsofme.OK)
                {
                    smallestid = long.MaxValue;
                    foreach (var tweet in mentionsofme)
                    {
                        _mentions.OnNext(tweet);
                        if (tweet.Id < smallestid) smallestid = tweet.Id;
                        if (tweet.Id > largestid) largestid = tweet.Id;
                        backfillQuota--;
                    }
                    await Task.Delay(_multiFetchBackoffTimer);
                }
                else
                {
                    // The Backoff will trigger 7 times before just giving up
                    // once at 30s, 60s, 1m, 2m, 4m, 8m and then 16m
                    // note that the last call into this will be 1m above the 15 "rate limit reset" window 
                    Task.Delay(TimeSpan.FromSeconds(backofftimer));
                    if (backofftimer > maxbackoff)
                        break;
                    backofftimer = backofftimer * 2;
                }
            } while (backfillQuota > 0);
            return largestid;
        }

        private async Task<long> GetRTOfMe_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 50;
            int backofftimer = 30;

            do
            {
                var rtofme = await Session.GetRetweetsOfMe(count: pagingSize, max_id: smallestid);
                if (rtofme.OK)
                {
                    smallestid = long.MaxValue;
                    foreach (var tweet in rtofme)
                    {
                        _mentions.OnNext(tweet);
                        if (tweet.Id < smallestid) smallestid = tweet.Id;
                        if (tweet.Id > largestid) largestid = tweet.Id;
                        backfillQuota--;
                    }
                    await Task.Delay(_multiFetchBackoffTimer);
                }
                else
                {
                    // The Backoff will trigger 7 times before just giving up
                    // once at 30s, 60s, 1m, 2m, 4m, 8m and then 16m
                    // note that the last call into this will be 1m above the 15 "rate limit reset" window 
                    Task.Delay(TimeSpan.FromSeconds(backofftimer));
                    if (backofftimer > maxbackoff)
                        break;
                    backofftimer = backofftimer * 2;
                }
            } while (backfillQuota > 0);
            return largestid;
        }

        private async Task<long> GetDirectMessages_Received_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 50;
            int backofftimer = 30;

            do
            {
                var dmrecd = await Session.GetDirectMessages(count: pagingSize, max_id: smallestid);
                if (dmrecd.OK)
                {
                    smallestid = long.MaxValue;
                    foreach (var dm in dmrecd)
                    {
                        _directmessages.OnNext(dm);
                        if (dm.Id < smallestid) smallestid = dm.Id;
                        if (dm.Id > largestid) largestid = dm.Id;
                        backfillQuota--;
                    }
                    await Task.Delay(_multiFetchBackoffTimer);
                }
                else
                {
                    // The Backoff will trigger 7 times before just giving up
                    // once at 30s, 60s, 1m, 2m, 4m, 8m and then 16m
                    // note that the last call into this will be 1m above the 15 "rate limit reset" window 
                    Task.Delay(TimeSpan.FromSeconds(backofftimer));
                    if (backofftimer > maxbackoff)
                        break;
                    backofftimer = backofftimer * 2;
                }
            } while (backfillQuota > 0);
            return largestid;
        }

        private async Task<long> GetDirectMessages_Sent_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 50;
            int backofftimer = 30;

            do
            {
                var mysentdms = await Session.GetDirectMessagesSent(count: pagingSize, max_id: smallestid);
                if (mysentdms.OK)
                {
                    smallestid = long.MaxValue;
                    foreach (var dm in mysentdms)
                    {
                        _directmessages.OnNext(dm);
                        if (dm.Id < smallestid) smallestid = dm.Id;
                        if (dm.Id > largestid) largestid = dm.Id;
                        backfillQuota--;
                    }
                    await Task.Delay(_multiFetchBackoffTimer);
                }
                else
                {
                    // The Backoff will trigger 7 times before just giving up
                    // once at 30s, 60s, 1m, 2m, 4m, 8m and then 16m
                    // note that the last call into this will be 1m above the 15 "rate limit reset" window 
                    Task.Delay(TimeSpan.FromSeconds(backofftimer));
                    if (backofftimer > maxbackoff)
                        break;
                    backofftimer = backofftimer * 2;
                }
            } while (backfillQuota > 0);
            return largestid;
        }

        private async Task<long> GetMyTweets_Backfill()
        {
            long smallestid = 0;
            long largestid = 0;
            int backfillQuota = 50;
            int backofftimer = 30;

            do
            {
                var hometl = await Session.GetUserTimeline(user_id:accountDetails.UserId, count: pagingSize, max_id: smallestid);
                if (hometl.OK)
                {
                    smallestid = long.MaxValue;
                    foreach (var tweet in hometl)
                    {
                        _mytweets.OnNext(tweet);
                        if (tweet.Id < smallestid) smallestid = tweet.Id;
                        if (tweet.Id > largestid) largestid = tweet.Id;
                        backfillQuota--;
                    }
                    await Task.Delay(_multiFetchBackoffTimer);
                }
                else
                {
                    // The Backoff will trigger 7 times before just giving up
                    // once at 30s, 60s, 1m, 2m, 4m, 8m and then 16m
                    // note that the last call into this will be 1m above the 15 "rate limit reset" window 
                    Task.Delay(TimeSpan.FromSeconds(backofftimer));
                    if (backofftimer > maxbackoff)
                        break;
                    backofftimer = backofftimer * 2;
                }
            } while (backfillQuota > 0);
            return largestid;
        }

    }
}