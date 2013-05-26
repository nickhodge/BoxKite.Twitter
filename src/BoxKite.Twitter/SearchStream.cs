// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoxKite.Twitter
{
    public class SearchStream : ISearchStream
    {
        readonly Subject<Tweet> foundtweets = new Subject<Tweet>();
        public IObservable<Tweet> FoundTweets { get { return foundtweets; } }

        private Subject<StreamSearchRequest> searchrequests = new Subject<StreamSearchRequest>();
        public Subject<StreamSearchRequest> SearchRequests
        {
            get { return searchrequests; }
        }

        public bool IsActive { get; private set; }

        public CancellationTokenSource CancelSearchStream { get; set; }
        public TwitterParametersCollection SearchParameters { get; set; }
        public Func<Task<HttpResponseMessage>> CreateOpenConnection { get; set; }
        public IUserSession parentSession { get; set; }

        public SearchStream(IUserSession session)
        {
            parentSession = session;
            CancelSearchStream = new CancellationTokenSource();
            SearchRequests.Subscribe(ChangeSearchRequest);
        }


        public void Start()
        {
            CancelSearchStream = new CancellationTokenSource();
            IsActive = true;
            Task.Factory.StartNew(ProcessMessages, CancelSearchStream.Token);  
        }

        public void Stop()
        {
            CancelSearchStream.Cancel(); 
            IsActive = false;
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


        private async void ProcessMessages()
        {
            var responseStream = await GetStream();
            while (!CancelSearchStream.IsCancellationRequested)
            {
                string line = "";
                try
                {
                    line = responseStream.ReadLine();
                    if (string.IsNullOrWhiteSpace(line.Trim())) continue;

                    if (line == "ENDBOXKITESEARCHSTREAMTEST")
                    {
                        responseStream.Dispose();
                        Dispose();
                        break;
                    }

                    if (line == "<html>") // needs embellishment
                    {
                        var restofline = responseStream.ReadToEnd();
#if (DEBUG)
                        Debug.WriteLine(restofline);
#endif
                        responseStream.Dispose();
                        Dispose();
                        break;
                    }

                    var obj = JsonConvert.DeserializeObject<JObject>(line);
                    if (obj["in_reply_to_user_id"] != null)
                    {
                        foundtweets.Publish(MapFromStreamTo<Tweet>(obj.ToString()));
                        continue;
                    }
                }
                catch (JsonReaderException jsonex)
                {
#if (DEBUG)
                    Debug.WriteLine(line);
#endif
                    continue;
                }
                catch (Exception)
                {
                    responseStream.Dispose();
                    Dispose(); 
                    break;
                }
            }
            responseStream.Dispose();
            Dispose();
            return;
        }

        private async Task<StreamReader> GetStream()
        {
            var response = await CreateOpenConnection();
            var stream = await response.Content.ReadAsStreamAsync();
            var responseStream = new StreamReader(stream);
            return responseStream;
        }

        private static T MapFromStreamTo<T>(string t)
        {
            return JsonConvert.DeserializeObject<T>(t);
        }

        public void Dispose()
        {
            IsActive = false;
            foundtweets.Dispose();
        }
    }
}
