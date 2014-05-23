// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.IO;

namespace BoxKite.Twitter.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ReadFully(this Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

    }
}
