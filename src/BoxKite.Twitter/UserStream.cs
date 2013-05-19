using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Modules.Streaming;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoxKite.Twitter
{
    internal class UserStream : IUserStream
    {
        readonly Func<Task<HttpResponseMessage>> createOpenConnection;
        readonly Subject<Tweet> tweets = new Subject<Tweet>();
        readonly Subject<DirectMessage> directmessages = new Subject<DirectMessage>();
        readonly Subject<Event> events = new Subject<Event>();
        readonly Subject<IEnumerable<long>> friends = new Subject<IEnumerable<long>>();
        readonly TimeSpan initialDelay = TimeSpan.FromSeconds(20);


        public IObservable<Tweet> Tweets { get { return tweets; } }
        public IObservable<DirectMessage> DirectMessages { get { return directmessages; } }
        public IObservable<Event> Events { get { return events; } }
        public IObservable<IEnumerable<long>> Friends { get { return friends; } }


        public bool _isActive = true;
        public TimeSpan delay = TimeSpan.FromSeconds(20);

        public bool IsActive { get { return _isActive; } set { _isActive = value; } }

        public UserStream(Func<Task<HttpResponseMessage>> createOpenConnection)
        {
            this.createOpenConnection = createOpenConnection;
        }

        public void Start()
        {
            Task.Factory.StartNew(ProcessMessages)
                .ContinueWith(HandleExceptionsIfRaised);
        }

        private void HandleExceptionsIfRaised(Task obj)
        {
            if (obj.Exception != null)
            {
                SendToAllSubscribers(obj.Exception);
            }

            if (obj.IsFaulted)
            {
                SendToAllSubscribers(new Exception("Stream is faulted"));
            }

            if (obj.IsCanceled)
            {
                SendToAllSubscribers(new Exception("Stream is cancelled"));
            }
        }

        private void SendToAllSubscribers(Exception exception)
        {
            tweets.OnError(exception);
            friends.OnError(exception);
            directmessages.OnError(exception);
            events.OnError(exception);
        }

        public void Stop()
        {
            _isActive = false;
        }

        private async void ProcessMessages()
        {
            var responseStream = await GetStream();
            while (_isActive)
            {
                // reconnect if the stream was closed previously
                if (responseStream == null)
                {
                    await Task.Delay(delay);
                    responseStream = await GetStream();
                }

                //get the line of JSON from the stream
                string line;
                try
                {
                    line = responseStream.ReadLine();
                }
                catch (IOException)
                {
                    delay += initialDelay;
                    responseStream.Dispose();
                    responseStream = null;
                    line = "";
                }

                if (delay.TotalMinutes <= 2)
                {
                    // TODO: give up
                }

                if (String.IsNullOrEmpty(line)) continue;
#if DEBUG
                Debug.WriteLine(line);
#endif

                // we have a valid connection - clear delay
                delay = TimeSpan.Zero;
                try
                {
                    var obj = JsonConvert.DeserializeObject<dynamic>(line);

                    //https://dev.twitter.com/docs/streaming-apis/messages
                    if (obj.friends != null)
                    {
                        SendFriendsMessage(obj.friends.Values<long>());
                        continue;
                    }

                    if (obj["event"] != null) // gotta use array indexing as event is a reserved word in C#
                    {
                        var eventValue = (JValue) obj["event"];
                        var eventText = eventValue.Value<string>();

                        var target = (JToken) obj["target"];
                        var source = (JToken) obj["source"];
                        var target_object = (JToken) obj["target_object"];
                        var created_at = (JToken) obj["target_object"];
                        var created_atText = eventValue.Value<string>();
                        var timestamp = created_atText.ToDateTimeOffset();


                        // TODO: raise on appropriate feed
                        if (eventText.StartsWith("list"))
                        {
                            //return a ListEvent
                        }
                        else
                        {
                            var e = new TweetEvent
                                    {
                                        EventName = eventText,
                                        Source = MapToStreamUser(target.ToString()),
                                        Target = MapToStreamUser(source.ToString()),
                                        TargetObject = MapToStreamTweet(target_object.ToString())
                                    };

                            events.OnNext(e);
                        }
                        continue;
                    }

                    if (obj.direct_message != null)
                    {
                        directmessages.OnNext(MapToStreamDM(obj.direct_message.ToString()));
                        continue;
                    }

                    if (obj.scrub_geo != null)
                    {
                        continue;
                    }

                    if (obj.limit != null)
                    {
                        continue;
                    }

                    if (obj.delete != null)
                    {
                        events.OnNext(MapToDeleteEvent(obj.delete.status.ToString()));
                        continue;
                    }

                    // fall through
                    tweets.OnNext(MapToStreamTweet(obj.ToString()));
                }
                catch (Exception x)
                {
#if DEBUG
                    Debug.WriteLine(x.ToString());
#endif
                    continue;
                }
            }
        }


        private Tweet MapToStreamTweet(string t)
        {
            return JsonConvert.DeserializeObject<Tweet>(t);
        }

        private DirectMessage MapToStreamDM(string dm)
        {
            return JsonConvert.DeserializeObject<DirectMessage>(dm);
        }

        private User MapToStreamUser(string user)
        {
            return JsonConvert.DeserializeObject<User>(user);
        }

        private DeleteEvent MapToDeleteEvent(string deleteevent)
        {
            return JsonConvert.DeserializeObject<DeleteEvent>(deleteevent);
        }

        private async Task<StreamReader> GetStream()
        {
            var response = await createOpenConnection();
            var stream = await response.Content.ReadAsStreamAsync();

            var responseStream = new StreamReader(stream);
            return responseStream;
        }

        private void SendFriendsMessage(IEnumerable<long> obj)
        {
            friends.OnNext(obj);
        }

        public void Dispose()
        {
            _isActive = false;
            friends.Dispose();
            tweets.Dispose();
            directmessages.Dispose();
            events.Dispose();
        }
    }
}
