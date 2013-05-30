// (c) 2012// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
// UNLESS NOTED ALTERNATIVE SOURCE

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
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

        // getting credentials commanding
        private bool _needsCredentialsCommands;
        public bool needsCredentialsCommands
        {
            get { return _needsCredentialsCommands; }
            set { SetProperty(ref _needsCredentialsCommands, value); }
        }

        private bool _stage1;
        public bool stage1
        {
            get { return _stage1; }
            set { SetProperty(ref _stage1, value); }
        }

        private bool _stage2;
        public bool stage2
        {
            get { return _stage2; }
            set { SetProperty(ref _stage2, value); }
        }

        private ICommand _getPINCommand;
        public ICommand getPINCommand
        {
            get
            {
                return _getPINCommand ?? (new CommandHandler(DoPINDisplay, _needsCredentialsCommands));
            }
        }

        // storage for the PIN when entered
        private string _authPIN;
        public string authPIN
        {
            get { return _authPIN; }
            set { SetProperty(ref _authPIN, value); }
        }

        private ICommand _acceptCommand;
        public ICommand acceptPINCommand
        {
            get
            {
                return _acceptCommand ?? (new CommandHandler(AuthPIN, _needsCredentialsCommands));
            }
        }

        // normal view commanding
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

        // storage for the TextBox where a tweet is composed (two-way binding)
        private string _tweetText;
        public string TweetText
        {
            get { return _tweetText; }
            set { SetProperty(ref _tweetText, value); }
        }

        // storage for the TextBox where a search text is entered (two-way binding)
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

        private bool _loggedOn;
        public bool LoggedOn
        {
            get { return _loggedOn; }
            set { SetProperty(ref _loggedOn, value); }
        }

        public MainWindowViewModel()
        {
            this.homeTimeLineTweets = new ObservableCollection<Tweet>();
            this.mentionsTimeLineTweets = new ObservableCollection<Tweet>();
            this.dmTimeLineTweets = new ObservableCollection<DirectMessage>();
            this.searchTweets = new ObservableCollection<Tweet>();

            // setup screen state machine; this makes visible the correct UI depending
            // on the state of the connection with twitter
            canExecuteCommands = true;
            SearchingOn = false;
            LoggingOn = true;
            LoggedOn = false;
            needsCredentialsCommands = false;
            stage1 = false;
            stage2 = false;
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
            needsCredentialsCommands = false;
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
                var endSearchTime = DateTime.Now;
                mainTwitterAccount.StopSearch();
                // firstly, get the "oldest" tweet
                var correctedStartSearchTime = searchTweets.Min(t => t.Time);
                // calc the performance by getting the time difference & number of tweets collected
                TimeSpan searchDuration = endSearchTime - correctedStartSearchTime;
                double searchInSeconds = searchDuration.TotalSeconds;
                int searchFound = searchTweets.Count();
                double perf = (searchFound / searchInSeconds);

                // and display
                SearchPerformance = String.Format("Tw/sec: {0} Tw/min: {1}", perf.ToString("0.00"), (perf * 60).ToString("0.00"));
                SearchingOn = false;
                
                // using below for memory cleanup testing
                //searchTweets.Clear();
                //GC.Collect();
            }
            else // turn on the search
            {
                searchTweets.Clear();
                SearchPerformance = "";
                mainTwitterAccount.StartSearch(_searchText);
                SearchingOn = true;
            }
        }

        public async void DoPINDisplay()
        {
            // this is the result of the user accepting the "go into browser"
            stage1 = false;
            stage2 = true;
            // this will ask the connection to display a browser window, going to twitter
            // asking the user to authenticate themselves, then authorise BoxKite to access their
            // twitter account
            await App.twitterConnection.BeginAuthentication();
        }

        public async void AuthPIN()
        {
            // after entering the PIN, and clicking OK, this method is run
            if (!string.IsNullOrWhiteSpace(authPIN))
            {
                stage2 = false;
                LoggingOn = true;
                var twitteraccount = await App.twitterConnection.CompleteAuthentication(authPIN);
                if (twitteraccount == null) // oops, not a good auth
                {
                    authPIN = "";
                    stage1 = true; // go around again :-(
                    LoggingOn = false;
                }
                mainTwitterAccount = twitteraccount;
                Connect();
                LoggingOn = false;
                LoggedOn = true;
                ManageTwitterCredentials.SaveCredentialsToFile(mainTwitterAccount._TwitterCredentials);
            }
        }

      }
}
