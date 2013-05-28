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
        readonly Subject<Tweet> _searchtimeline = new Subject<Tweet>();
        public IObservable<Tweet> SearchTimeLine { get { return _searchtimeline; } }
        private string _currentSearchText = "";

        private CancellationTokenSource TwitterSearchCommunication;

        public void StartSearch(string textToSearch)
        {
            TwitterSearchCommunication = new CancellationTokenSource();
            //
            _currentSearchText = textToSearch;
            SearchStream = Session.StartSearchStream(track:textToSearch);
            SearchStream.FoundTweets.Subscribe(_searchtimeline.OnNext);
            SearchStream.Start();
            //
            ProcessSearchBackFill_Pump();
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
                //TODO: need to unhardcode SearchResultType
                var searchtl = await Session.SearchFor(searchtext:_currentSearchText, searchResponseType:SearchResultType.Recent, count: pagingSize, max_id: smallestid);
                if (searchtl.OK)
                {
                    smallestid = long.MaxValue;
                    foreach (var tweet in searchtl.Tweets)
                    {
                        _searchtimeline.OnNext(tweet);
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
