// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoxKite.Twitter
{
    public class UserStream : IUserStream
    {
        // Public properties with private backingstores
        readonly Subject<Tweet> _tweets = new Subject<Tweet>();
        readonly Subject<DirectMessage> _directmessages = new Subject<DirectMessage>();
        readonly Subject<IStreamEvent> _events = new Subject<IStreamEvent>();
        readonly Subject<DeleteEvent> _deleteevents = new Subject<DeleteEvent>();
        readonly Subject<StreamScrubGeo> _scrubgeorequests = new Subject<StreamScrubGeo>();
        readonly Subject<StreamLimitNotice> _limitnotices = new Subject<StreamLimitNotice>();
        readonly Subject<StreamStatusWithheld> _statuswithheld = new Subject<StreamStatusWithheld>();
        readonly Subject<StreamUserWithheld> _userwithheld = new Subject<StreamUserWithheld>();
        readonly Subject<IEnumerable<long>> _friends = new Subject<IEnumerable<long>>();
        public IObservable<Tweet> Tweets { get { return _tweets; } }
        public IObservable<DirectMessage> DirectMessages { get { return _directmessages; } }
        public IObservable<IStreamEvent> Events { get { return _events; } }
        public IObservable<DeleteEvent> DeleteEvents { get { return _deleteevents; } }
        public IObservable<StreamScrubGeo> ScrubGeoRequests { get { return _scrubgeorequests; } }
        public IObservable<StreamLimitNotice> LimitNotices { get { return _limitnotices; } }
        public IObservable<StreamStatusWithheld> StatusWithheld { get { return _statuswithheld; } }
        public IObservable<StreamUserWithheld> UserWithheld { get { return _userwithheld; } }
        public IObservable<IEnumerable<long>> Friends { get { return _friends; } }
        public CancellationTokenSource CancelUserStream { get; set; }
        public TimeSpan TimeoutDelay { get; set; }

        readonly Subject<bool> _userStreamActive = new Subject<bool>();
        public IObservable<bool> UserStreamActive { get { return _userStreamActive; } }

        // implemention things
        readonly Subject<string> _readLines = new Subject<string>();
        private IObservable<string> readLinesObservable { get { return _readLines; } }
        readonly Func<Task<HttpResponseMessage>> _createOpenConnection;

        public UserStream(Func<Task<HttpResponseMessage>> createOpenConnection)
        {
            this._createOpenConnection = createOpenConnection;
            TimeoutDelay = TimeSpan.FromMinutes(2);
        }

        public void Start()
        {
            CancelUserStream = new CancellationTokenSource();
            Task.Factory.StartNew(ProcessMessages, CancelUserStream.Token);
            _userStreamActive.OnNext(true);
        }

        public void Dispose()
        {
            Stop();
        }

        public void Stop()
        {
            CancelUserStream.Cancel();
            _userStreamActive.OnNext(false);
        }

        private void ProcessMessages()
        {
            Task.Factory.StartNew(ReadLines, CancelUserStream.Token);
            readLinesObservable.Subscribe(line =>
            {
#region Main Observer work here
#if (DEBUG)
                Debug.WriteLine(line);
#endif
#if (TRACE)
                if (line == "ENDBOXKITEUSERSTREAMTEST")
                {
                    Stop();
                }
#endif
                if (string.IsNullOrWhiteSpace(line)) return;
                var obj = JsonConvert.DeserializeObject<JObject>(line);

                //https://dev.twitter.com/docs/streaming-apis/messages

                if (obj["direct_message"] != null)
                {
                    _directmessages.OnNext(MapFromStreamTo<DirectMessage>(obj["direct_message"].ToString()));
                    return;
                }

                if (obj["in_reply_to_user_id"] != null)
                {
                    _tweets.OnNext(MapFromStreamTo<Tweet>(obj.ToString()));
                    return;
                }

                if (obj["friends"] != null)
                {
                    SendFriendsMessage(obj["friends"].Values<long>());
                    return;
                }

                // source: https://dev.twitter.com/docs/streaming-apis/messages#Events_event
                if (obj["event"] != null)
                {
                    _events.OnNext(MapFromEventInStream(obj));
                    return;
                }

                if (obj["scrub_geo"] != null)
                {
                    _scrubgeorequests.OnNext(MapFromStreamTo<StreamScrubGeo>(obj["scrub_geo"].ToString()));
                    return;
                }

                if (obj["limit"] != null)
                {
                    _limitnotices.OnNext(MapFromStreamTo<StreamLimitNotice>(obj["limit"].ToString()));
                    return;
                }

                if (obj["delete"] != null)
                {
                    _deleteevents.OnNext(MapFromStreamTo<DeleteEvent>(obj["delete"].ToString()));
                    return;
                }

                if (obj["status_withheld"] != null)
                {
                    _statuswithheld.OnNext(
                        MapFromStreamTo<StreamStatusWithheld>(obj["status_withheld"].ToString()));
                    return;
                }

                if (obj["user_withheld"] != null)
                {
                    _userwithheld.OnNext(MapFromStreamTo<StreamUserWithheld>(obj["user_withheld"].ToString()));
                    return;
                }

                if (obj["disconnect"] != null)
                {
                    var disconnect = MapFromStreamTo<StreamDisconnect>(obj["disconnect"].ToString());
                    Stop();
                    // check for non-hard disconnects & attempt reconnect
                }

                if (obj["warning"] == null) return; // no warnings, so start loop from beginning again

                if (obj["warning"]["percent_full"] != null)
                {
                    var stallwarning = MapFromStreamTo<StreamStallWarning>(obj["warning"].ToString());
                    // do something something stall warning.
                }
                if (obj["warning"]["user_id"] != null)
                {
                    var userfollowswarning =
                        MapFromStreamTo<StreamToManyFollowsWarning>(obj["warning"].ToString());
                    // do something something user follows warning this is pretty final, actually.
                    Stop();
                }
#endregion           
            });
            while (!CancelUserStream.IsCancellationRequested)
            {
                // spin
            }
        }

        // Previously, this used the IEnumerable<string>.ToObservable / yield pattern
        // but this doesnt permit try/catch IOErrors; which might occur if the underlying connection dies
        // therefore, a little more verbose, and with usings to catch disposable style objects
        private async void ReadLines()
        {
            using (var response = await _createOpenConnection())
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
#if (!TRACE)                    
                    stream.ReadTimeout = TimeoutDelay.Milliseconds; // set read timeout in millisecs
#endif
                    using (var reader = new StreamReader(stream))
                    {
                        try
                        {
                            while (!CancelUserStream.IsCancellationRequested)
                            {
                                var line = await reader.ReadLineAsync();
                                _readLines.OnNext(line);
                            }
                        }
                        catch (Exception) // catch all, especially for IOExceptions when connection fails/stops
                        {
                            Stop();
                        }
                    }
                }
            }
        }

        private void SendFriendsMessage(IEnumerable<long> obj)
        {
            _friends.OnNext(obj);
        }

        private static IStreamEvent MapFromEventInStream(JObject e)
        {
            string eventName = e["event"].ToString();
            if (eventName.Substring(0, 5) == "list_") // as list_*
            {
                return MapFromStreamTo<ListStreamEvent>(e.ToString());
            }

            switch (eventName)
            {
                case "favorite":
                case "unfavorite":
                    return MapFromStreamTo<TweetStreamEvent>(e.ToString());
                default:
                    return MapFromStreamTo<StreamEvent>(e.ToString());
            }
        }

        private static T MapFromStreamTo<T>(string t)
        {
            return JsonConvert.DeserializeObject<T>(t);
        }
    }
}
