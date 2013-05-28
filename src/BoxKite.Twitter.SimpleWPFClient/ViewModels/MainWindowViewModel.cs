using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;
using BoxKite.Twitter;

namespace BoxKite.WPFSimpleClient
{
    public class MainWindowViewModel : BindableBase
    {
        // Collections containing the Tweets to be displayed
        // These are populated by ".Subscribing" to BoxKite.Twitter's channels of Tweets/DMs
        public ObservableCollection<Tweet> homeTimeLineTweets { get; set; }
        public ObservableCollection<Tweet> mentionsTimeLineTweets { get; set; }
        public ObservableCollection<DirectMessage> dmTimeLineTweets { get; set; }
        public ObservableCollection<Tweet> searchTweets { get; set; }

        private TwitterAccount _mainTwitterAccount;
        public TwitterAccount mainTwitterAccount
        {
            get { return _mainTwitterAccount; }
            set { SetProperty(ref _mainTwitterAccount, value); }
        }

        private bool _canExecuteCommands;
        public bool canExecuteCommands
        {
            get { return _canExecuteCommands; }
            set { SetProperty(ref _canExecuteCommands, value); }
        }

        private ICommand _sendTweetCommand;
        public ICommand SendTweetCommand
        {
            get
            {
                return _sendTweetCommand ?? (new CommandHandler(SendTweet, _canExecuteCommands));
            }
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (new CommandHandler(StartSearch, _canExecuteCommands));
            }
        }

        private string _tweetText;
        public string TweetText
        {
            get { return _tweetText; }
            set { SetProperty(ref _tweetText, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        private string _searchPerformance;
        public string SearchPerformance
        {
            get { return _searchPerformance; }
            set { SetProperty(ref _searchPerformance, value); }
        }

        private bool _searchingOn;
        public bool SearchingOn
        {
            get { return _searchingOn; }
            set { SetProperty(ref _searchingOn, value); }
        }

        private bool _loggingOn;
        public bool LoggingOn
        {
            get { return _loggingOn; }
            set { SetProperty(ref _loggingOn, value); }
        }

        // holds start/end times for performance measurement
        private DateTime _startSearchTime;
        private DateTime _endSearchTime;

        public MainWindowViewModel()
        {
            this.homeTimeLineTweets = new ObservableCollection<Tweet>();
            this.mentionsTimeLineTweets = new ObservableCollection<Tweet>();
            this.dmTimeLineTweets = new ObservableCollection<DirectMessage>();
            this.searchTweets = new ObservableCollection<Tweet>();
            canExecuteCommands = true;
            SearchingOn = false;
            LoggingOn = true;
            TweetText = "";
            SearchText = "";
            SearchPerformance = "";
        }

        // wire the BoxKite.Twitter channels to Collections of Tweets
        // Note: "ObserveOn.Synchronization"; ensures all the background tasks getting the messages
        //       pop onto the correct thread where the Collections live. All works async under the covers, so that's nice
        public void Connect()
        {
            mainTwitterAccount.TimeLine.ObserveOn(SynchronizationContext.Current).Subscribe(t => homeTimeLineTweets.Add(t));
            mainTwitterAccount.Mentions.ObserveOn(SynchronizationContext.Current).Subscribe(m => mentionsTimeLineTweets.Add(m));
            mainTwitterAccount.DirectMessages.ObserveOn(SynchronizationContext.Current).Subscribe(d => dmTimeLineTweets.Add(d));
            mainTwitterAccount.SearchTimeLine.ObserveOn(SynchronizationContext.Current).Subscribe(s => searchTweets.Add(s));
            mainTwitterAccount.Start();
            canExecuteCommands = true;
        }

        public async void SendTweet()
        {
            var st = await mainTwitterAccount.Session.SendTweet(TweetText);
            if (st.OK)
                TweetText = "";
        }

        public void StartSearch()
        {
            if (SearchingOn) // the app is getting responses from search, now turn it off & get performance
            {
                _endSearchTime = DateTime.Now;
                mainTwitterAccount.StopSearch();
                // calc the performance by getting the time difference & number of tweets collected
                TimeSpan searchDuration = _endSearchTime - _startSearchTime;
                double searchInSeconds = searchDuration.TotalSeconds;
                int searchFound = searchTweets.Count();
                double perf = (searchFound / searchInSeconds);
                SearchPerformance = String.Format("Tweets per second: {0}", perf.ToString("0.00"));
                SearchingOn = false;
                searchTweets.Clear();
            }
            else // turn on the search
            {
                _startSearchTime = DateTime.Now;
                SearchPerformance = "";
                mainTwitterAccount.StartSearch(_searchText);
                SearchingOn = true;
            }
        }
    }
}
