// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoxKite.Twitter
{
    public class SearchStream : TwitterStream, ISearchStream
    {
        readonly Subject<Tweet> _foundtweets = new Subject<Tweet>();
        public IObservable<Tweet> FoundTweets { get { return _foundtweets; } }

        private readonly Subject<StreamSearchRequest> _searchrequests = new Subject<StreamSearchRequest>();
        public Subject<StreamSearchRequest> SearchRequests{ get { return _searchrequests; } }
        public TwitterParametersCollection SearchParameters { get; set; }

        public SearchStream()
        {
            SearchRequests.Subscribe(ChangeSearchRequest);
        }

        public void Start()
        {
            CancelStream = new CancellationTokenSource();
            Task.Factory.StartNew(ProcessMessages, CancelStream.Token);
            _streamActive.OnNext(true);
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
            Task.Factory.StartNew(ReadLines, CancelStream.Token);
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
                if (line == "<html>") // todo: needs embellishment, especially as Twitter uses HTML embedded in stream for errors
                {
                    Stop();
                }
                var obj = JsonConvert.DeserializeObject<JObject>(line);
                if (obj["in_reply_to_user_id"] != null)
                {
                    _foundtweets.OnNext(MapFromStreamTo<Tweet>(obj.ToString()));
                }
#endregion
            });
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
