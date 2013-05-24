// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using BoxKite.Twitter.Modules.Streaming;

namespace BoxKite.Twitter.Modules
{
    public static class SearchStreamExtensions
    {
        
        public static ISearchStream StartSearchStream(this IUserSession session, string track=null, string follow=null, string locations=null)
        {
            var searchStream = new SearchStream(session);
            searchStream.SearchParameters = searchStream.ChangeSearchParameters(track, follow, locations);
            return searchStream;
        }

        public static ISearchStream StartSearchStream(this IUserSession session, IEnumerable<string> track = null, IEnumerable<string> follow = null, IEnumerable<string> locations = null)
        {
            var searchStream = new SearchStream(session);
            searchStream.SearchParameters = searchStream.ChangeSearchParameters(track, follow, locations);
            return searchStream;
        }
    }
}
