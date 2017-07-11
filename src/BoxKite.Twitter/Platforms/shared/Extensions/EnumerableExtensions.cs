// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.Linq;

namespace BoxKite.Twitter.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool HasAny<T>(this IEnumerable<T> source)
        {
            return source != null && source.Any();
        }
    }
}