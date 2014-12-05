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
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoxKite.Twitter
{
    public class SearchStream : ISearchStream
    {
        readonly Subject<Tweet> foundtweets = new Subject<Tweet>();
        public IObservable<Tweet> FoundTweets { get { return foundtweets; } }

        private Subject<StreamSearchRequest> searchrequests = new Subject<StreamSearchRequest>();
        public Subject<StreamSearchRequest> SearchRequests{ get { return searchrequests; } }

        readonly Subject<bool> _searchStreamActive = new Subject<bool>();
        public IObservable<bool> SearchStreamActive { get { return _searchStreamActive; } }
        public CancellationTokenSource CancelSearchStream { get; set; }
        public TwitterParametersCollection SearchParameters { get; set; }
        public TimeSpan TimeoutDelay { get; set; }
        public Func<Task<HttpResponseMessage>> CreateOpenConnection { get; set; }

        // Implementation internals
         readonly Subject<string> _readLines = new Subject<string>();
        private IObservable<string> readLinesObservable { get { return _readLines; } }
        private IUserSession parentSession { get; set; }

        public SearchStream(IUserSession session)
        {
            parentSession = session;
            SearchRequests.Subscribe(ChangeSearchRequest);
        }

        public void Start()
        {
            CancelSearchStream = new CancellationTokenSource();
            Task.Factory.StartNew(ProcessMessages, CancelSearchStream.Token);  
            _searchStreamActive.OnNext(true);
        }

        public void Stop()
        {
            CancelSearchStream.Cancel();
            _searchStreamActive.OnNext(false);
        }

        private void ChangeSearchRequest(StreamSearchRequest sr)
        {
            Stop();
            SearchParameters = ChangeSearchParameters(sr);
            Start();
        }


        //https://dev.twitter.com/docs/streaming-apis/parameters
        public TwitterParametersCollection ChangeSearchParameters(StreamSearchRequest searchRequest)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(stall_warnings: false, delimited: false);

            if (searchRequest.Tracks.HasAny())
                parameters.CreateCommaDelimitedList("track", searchRequest.Tracks);
            if (searchRequest.Follows.HasAny())
                parameters.CreateCommaDelimitedList("follow", searchRequest.Follows);
            if (searchRequest.Locations.HasAny())
                parameters.CreateCommaDelimitedList("locations", searchRequest.Locations);

            parameters.Add("filter_level", searchRequest.FilterLevel);
            parameters.Add("language", searchRequest.Language);

            return parameters;
        }

        public TwitterParametersCollection ChangeSearchParameters(string track = null, string follow = null, string locations = null, string filterlevel = "none", string language = "en")
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(stall_warnings: false, delimited: false);

            if (track != null)
                parameters.CreateCommaDelimitedList("track", new List<string> { track });
            if (follow != null)
                parameters.CreateCommaDelimitedList("follow", new List<string> { follow });
            if (locations != null)
                parameters.CreateCommaDelimitedList("locations", new List<string> { locations });

            parameters.Add("filter_level", filterlevel);
            parameters.Add("language", language);

            return parameters;
        }

        public TwitterParametersCollection ChangeSearchParameters(IEnumerable<string> track = null, IEnumerable<string> follow = null, IEnumerable<string> locations = null, string filterlevel = "none", string language = "en")
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(stall_warnings: false, delimited: false);

            if (track != null)
                parameters.CreateCommaDelimitedList("track", track);
            if (follow != null)
                parameters.CreateCommaDelimitedList("follow", follow);
            if (locations != null)
                parameters.CreateCommaDelimitedList("locations", locations);

            parameters.Add("filter_level", filterlevel);
            parameters.Add("language", language);

            return parameters;
        }

        private void ProcessMessages()
        {
            Task.Factory.StartNew(ReadLines, CancelSearchStream.Token);
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
                if (line == "<html>") // needs embellishment
                {
                    Stop();
                }
                var obj = JsonConvert.DeserializeObject<JObject>(line);
                if (obj["in_reply_to_user_id"] != null)
                {
                    foundtweets.OnNext(MapFromStreamTo<Tweet>(obj.ToString()));
                }
#endregion
            });
        }

         // Previously, this used the IEnumerable<string>.ToObservable / yield pattern
        // but this doesnt permit try/catch IOErrors; which might occur if the underlying connection dies
        // therefore, a little more verbose, and with usings to catch disposable style objects
        private async void ReadLines()
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
                            while (!CancelSearchStream.IsCancellationRequested)
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

        private static T MapFromStreamTo<T>(string t)
        {
            return JsonConvert.DeserializeObject<T>(t);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
