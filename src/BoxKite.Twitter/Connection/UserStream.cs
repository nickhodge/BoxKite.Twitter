// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoxKite.Twitter
{
    public class UserStream : IUserStream
    {
        readonly Subject<Tweet> tweets = new Subject<Tweet>();
        readonly Subject<DirectMessage> directmessages = new Subject<DirectMessage>();
        readonly Subject<IStreamEvent> events = new Subject<IStreamEvent>();
        readonly Subject<DeleteEvent> deleteevents = new Subject<DeleteEvent>();
        readonly Subject<StreamScrubGeo> scrubgeorequests = new Subject<StreamScrubGeo>();
        readonly Subject<StreamLimitNotice> limitnotices = new Subject<StreamLimitNotice>();
        readonly Subject<StreamStatusWithheld> statuswithheld = new Subject<StreamStatusWithheld>();
        readonly Subject<StreamUserWithheld> userwithheld = new Subject<StreamUserWithheld>();
        readonly Subject<IEnumerable<long>> friends = new Subject<IEnumerable<long>>();

        readonly Func<Task<HttpResponseMessage>> createOpenConnection;

        public IObservable<Tweet> Tweets { get { return tweets; } }
        public IObservable<DirectMessage> DirectMessages { get { return directmessages; } }
        public IObservable<IStreamEvent> Events { get { return events; } }
        public IObservable<DeleteEvent> DeleteEvents { get { return deleteevents; } }
        public IObservable<StreamScrubGeo> ScrubGeoRequests { get { return scrubgeorequests; } }
        public IObservable<StreamLimitNotice> LimitNotices { get { return limitnotices; } }
        public IObservable<StreamStatusWithheld> StatusWithheld { get { return statuswithheld; } }
        public IObservable<StreamUserWithheld> UserWithheld { get { return userwithheld; } }
        public IObservable<IEnumerable<long>> Friends { get { return friends; } }

        readonly Subject<bool> _userStreamActive = new Subject<bool>();
        public IObservable<bool> UserStreamActive { get { return _userStreamActive; } }
        public CancellationTokenSource CancelUserStream { get; set; }
        public TimeSpan delay = TimeSpan.FromSeconds(5);

        public UserStream(Func<Task<HttpResponseMessage>> createOpenConnection)
        {
            this.createOpenConnection = createOpenConnection;
            CancelUserStream = new CancellationTokenSource();
            _userStreamActive.OnNext(true);
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
            friends.Dispose();
            tweets.Dispose();
            directmessages.Dispose();
            events.Dispose();
            directmessages.Dispose();
            scrubgeorequests.Dispose();
            _userStreamActive.OnNext(false);
            CancelUserStream.Cancel();
        }

        private async void ProcessMessages()
        {
            try
            {
                var responseStream = await GetStream();
                var x = ObserveLines(responseStream);
                x.Subscribe(line =>
                {
#if (DEBUG)
                    Debug.WriteLine(line);
#endif
                    try
                    {
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
                            directmessages.OnNext(MapFromStreamTo<DirectMessage>(obj["direct_message"].ToString()));
                            return;
                        }

                        if (obj["in_reply_to_user_id"] != null)
                        {
                            tweets.OnNext(MapFromStreamTo<Tweet>(obj.ToString()));
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
                            events.OnNext(MapFromEventInStream(obj));
                            return;
                        }

                        if (obj["scrub_geo"] != null)
                        {
                            scrubgeorequests.OnNext(MapFromStreamTo<StreamScrubGeo>(obj["scrub_geo"].ToString()));
                            return;
                        }

                        if (obj["limit"] != null)
                        {
                            limitnotices.OnNext(MapFromStreamTo<StreamLimitNotice>(obj["limit"].ToString()));
                            return;
                        }

                        if (obj["delete"] != null)
                        {
                            deleteevents.OnNext(MapFromStreamTo<DeleteEvent>(obj["delete"].ToString()));
                            return;
                        }

                        if (obj["status_withheld"] != null)
                        {
                            statuswithheld.OnNext(
                                MapFromStreamTo<StreamStatusWithheld>(obj["status_withheld"].ToString()));
                            return;
                        }

                        if (obj["user_withheld"] != null)
                        {
                            userwithheld.OnNext(MapFromStreamTo<StreamUserWithheld>(obj["user_withheld"].ToString()));
                            return;
                        }

                        if (obj["disconnect"] != null)
                        {
                            var disconnect = MapFromStreamTo<StreamDisconnect>(obj["disconnect"].ToString());
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
                    }
                    catch (InvalidCastException)
                    {
                        //ignore it fanboy
                    }
                });
            }
            catch (Exception e)
            {
                //Stop();
            }
        }

        private static IEnumerable<string> ReadLines(Stream stream)
        {
            var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var rc = reader.ReadLine();
                yield return rc;
            }
        }

        private static IObservable<string> ObserveLines(Stream stream)
        {
            return ReadLines(stream).ToObservable(Scheduler.CurrentThread);
        }

        private async Task<Stream> GetStream()
        {
            var response = await createOpenConnection();
            var stream = await response.Content.ReadAsStreamAsync();
            return stream;
        }

        private void SendFriendsMessage(IEnumerable<long> obj)
        {
            friends.OnNext(obj);
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
