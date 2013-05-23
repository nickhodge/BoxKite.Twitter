using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Modules.Streaming;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoxKite.Twitter
{
    public class SearchStream : ISearchStream
    {
        readonly Subject<Tweet> foundtweets = new Subject<Tweet>();
        public IObservable<Tweet> FoundTweets { get { return foundtweets; } }

        private Subject<string> searchrequests = new Subject<string>();
        public Subject<string> SearchRequests
        {
            get { return searchrequests; }
        }

        public bool IsActive { get; private set; }

        private CancellationTokenSource Cts { get; set; }
        private IUserSession UserSession { get; set; }
        public TwitterParametersCollection SearchParameters { get; set; }

        public SearchStream(IUserSession session)
        {
            Cts = new CancellationTokenSource();
            UserSession = session;
            SearchRequests.Subscribe(ChangeSearchRequest);
            IsActive = true;
        }

        public void Start()
        {
            Cts = new CancellationTokenSource();
            Task.Factory.StartNew(ProcessMessages, Cts.Token);  
        }

        public void Stop()
        {
            IsActive = false;
        }

        private void ChangeSearchRequest(string s)
        {
            Cts.Cancel();
            SearchParameters = ChangeSearchParameters(track: s);
            Start();
        }

        //https://dev.twitter.com/docs/streaming-apis/parameters
        public TwitterParametersCollection ChangeSearchParameters(string track = null, string follow = null, string locations = null)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(stall_warnings: false, delimited: false);

            if (track != null)
                parameters.CreateCommaDelimitedList("track", new List<string> { track });
            if (follow != null)
                parameters.CreateCommaDelimitedList("follow", new List<string> { follow });
            if (locations != null)
                parameters.CreateCommaDelimitedList("locations", new List<string> { locations });

            parameters.Add("filter_level", "none"); //note this can be none,low,medium
            // you can also add parameters.Add("language","en");

            return parameters;
        }

        public TwitterParametersCollection ChangeSearchParameters(IEnumerable<string> track = null, IEnumerable<string> follow = null, IEnumerable<string> locations = null)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(stall_warnings: false, delimited: false);

            if (track != null)
                parameters.CreateCommaDelimitedList("track", track);
            if (follow != null)
                parameters.CreateCommaDelimitedList("follow", follow);
            if (locations != null)
                parameters.CreateCommaDelimitedList("locations", locations);

            parameters.Add("filter_level", "none"); //note this can be none,low,medium
            // you can also add parameters.Add("language","en");

            return parameters;
        }


        private async void ProcessMessages()
        {
            var responseStream = await GetStream();
            while (!Cts.IsCancellationRequested)
            {
                string line;
                try
                {
                    line = responseStream.ReadLine();
                    if (string.IsNullOrWhiteSpace(line.Trim())) continue;

                    var obj = JsonConvert.DeserializeObject<JObject>(line);
                    if (obj["in_reply_to_user_id"] != null)
                    {
                        foundtweets.OnNext(MapFromStreamTo<Tweet>(obj.ToString()));
                        continue;
                    }
                }
                catch (Exception)
                {
                    responseStream.Dispose();
                    break;
                }
            }
        }

        private async Task<HttpResponseMessage> createOpenConnection()
        {
            if (SearchParameters.EnsureOneOf(new[] {"track", "follow", "locations"}).IsFalse())
                return null;
            var request = UserSession.CreatePost(Api.SearchStreaming("/1.1/statuses/filter.json"),
                SearchParameters);
            var c = new HttpClient {Timeout = TimeSpan.FromDays(1)};
            return await c.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, Cts.Token);
        }

        private async Task<StreamReader> GetStream()
        {
            var response = await createOpenConnection();
            var stream = await response.Content.ReadAsStreamAsync();
            var responseStream = new StreamReader(stream);
            return responseStream;
        }

        private static T MapFromStreamTo<T>(string t)
        {
            return JsonConvert.DeserializeObject<T>(t);
        }
    }
}
