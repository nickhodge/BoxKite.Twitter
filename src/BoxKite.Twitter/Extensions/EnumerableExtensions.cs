using System.Collections.Generic;
using System.Linq;

namespace BoxKite.Twitter.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool HasAny<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return false;

            return source.Any();
        }
    }
}