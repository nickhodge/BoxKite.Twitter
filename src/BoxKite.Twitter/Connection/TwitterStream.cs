// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
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
    public abstract class TwitterStream
    {
        public TimeSpan TimeoutDelay { get; set; }
        public Func<Task<HttpResponseMessage>> CreateOpenConnection { get; set; }
        public CancellationTokenSource CancelStream { get; set; }

        internal readonly Subject<StreamSignal> _streamActive = new Subject<StreamSignal>();
        public IObservable<StreamSignal> StreamActive { get { return _streamActive; } }


        // implemention things
        readonly Subject<string> _readLines = new Subject<string>();
        internal IObservable<string> readLinesObservable { get { return _readLines; } }

        public void Stop()
        {
            CancelStream.Cancel();
            _streamActive.OnNext(StreamSignal.Stopped);
        }

        // Previously, this used the IEnumerable<string>.ToObservable / yield pattern
        // but this doesnt permit try/catch IOErrors; which might occur if the underlying connection dies
        // therefore, a little more verbose, and with usings to correctly handle and de-alloc IDisposable objects
        internal async void ReadLines()
        {
            using (var response = await CreateOpenConnection())
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
                            while (!CancelStream.IsCancellationRequested)
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

        internal static IStreamEvent MapFromEventInStream(JObject e)
        {
            var eventName = e["event"].ToString();
            if (eventName.Substring(0, 5) == "list_") // as list_*
            {
                return MapFromStreamTo<ListStreamEvent>(e.ToString());
            }

            switch (eventName)
            {
                case "block":
                    return MapFromStreamTo<BlockStreamEvent>(e.ToString());
                case "unblock":
                    return MapFromStreamTo<UnblockStreamEvent>(e.ToString());
                case "favorite":
                    return MapFromStreamTo<FavoriteStreamEvent>(e.ToString());
                case "unfavorite":
                    return MapFromStreamTo<UnfavoriteStreamEvent>(e.ToString());
                case "follow":
                    return MapFromStreamTo<FollowStreamEvent>(e.ToString());
                case "unfollow":
                    return MapFromStreamTo<UnfollowStreamEvent>(e.ToString());
                default:
                    return MapFromStreamTo<StreamEvent>(e.ToString());
            }
        }

        internal static T MapFromStreamTo<T>(string t)
        {
            return JsonConvert.DeserializeObject<T>(t);
        }
    }
}
