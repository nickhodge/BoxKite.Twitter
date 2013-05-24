// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;

namespace BoxKite.Twitter.Models.Stream
{
    public class StreamSearchRequest
    {
        public IList<string> follows;
        public IList<string> tracks;
        public IList<string> locations;

        public StreamSearchRequest()
        {
            follows = new List<string>();
            tracks = new List<string>();
            locations = new List<string>();
        }
    }
}
