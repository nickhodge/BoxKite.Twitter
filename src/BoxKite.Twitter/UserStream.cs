using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Models.Stream;
using BoxKite.Twitter.Modules.Streaming;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoxKite.Twitter
{
    public class UserStream : IUserStream
    {
        readonly Func<Task<HttpResponseMessage>> createOpenConnection;
        readonly Subject<Tweet> tweets = new Subject<Tweet>();
        readonly Subject<DirectMessage> directmessages = new Subject<DirectMessage>();
        readonly Subject<StreamEvent> events = new Subject<StreamEvent>();
        readonly Subject<IEnumerable<long>> friends = new Subject<IEnumerable<long>>();
        readonly TimeSpan initialDelay = TimeSpan.FromSeconds(20);


        public IObservable<Tweet> Tweets { get { return tweets; } }
        public IObservable<DirectMessage> DirectMessages { get { return directmessages; } }
        public IObservable<StreamEvent> Events { get { return events; } }
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

                if (line == "ENDBOXKITEUSERSTREAMTEST")
                {
                    // special, non JSON and therefore highly unlikely to be sent from the live service
                    // this is the token string used by the testing harness.
                    Stop();
                }
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

                    // source: https://dev.twitter.com/docs/streaming-apis/messages#Events_event
                    if (obj["event"] != null) // gotta use array indexing as event is a reserved word in C#
                    {
                        events.OnNext(MapFromEventInStream(obj));
                        continue;
                    }

                    if (obj.direct_message != null)
                    {
                        directmessages.OnNext(MapFromStreamTo<DirectMessage>(obj.direct_message.ToString()));
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
                        //events.OnNext(MapFromStreamTo<DeleteEvent>(obj.delete.status.ToString()));
                        //continue;
                    }

                    // fall through
                    tweets.OnNext(MapFromStreamTo<Tweet>(obj.ToString()));
                }
                catch (Exception x)
                {
                    //eat the exception for the moment
#if DEBUG
                    Debug.WriteLine(x.ToString());
#endif
                    continue;
                }
            }
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

        private static StreamEvent MapFromEventInStream(dynamic e)
        {
            string eventName = e["event"].Value.ToString();
            if (eventName.Substring(0, 5) == "list_") // as list_*
            {
                return MapFromStreamTo<ListStreamEvent>(e.ToString());
            }

            switch (eventName)
            {
                case "favorite":
                case "unfavorite":
                    return MapFromStreamTo<TweetStreamEvent>(e.ToString());
                    break;
                default:
                    return MapFromStreamTo<StreamEvent>(e.ToString());
                    break;
            }
            return null;
        }

        private static T MapFromStreamTo<T>(string t)
        {
            return JsonConvert.DeserializeObject<T>(t);
        }
    }
}
