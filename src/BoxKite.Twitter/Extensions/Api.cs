// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;

namespace BoxKite.Twitter.Extensions
{
    public static class TwitterApi
    {
        public static string UserAgent()
        {
            return "BoxKite.Twitter/1.0";
        }

        public static string Resolve(string format, params object[] args)
        {
            if (format.StartsWith("/"))
                format = format.Substring(1);

            return String.Concat("https://api.twitter.com/", String.Format(format, args));
        }

        public static string Upload(string format, params object[] args)
        {
            if (format.StartsWith("/"))
                format = format.Substring(1);

            return String.Concat("https://api.twitter.com/", String.Format(format, args));
        }

        public static string UserStreaming(string format, params object[] args)
        {
            if (format.StartsWith("/"))
                format = format.Substring(1);

            return String.Concat("https://userstream.twitter.com/", String.Format(format, args));
        }


        public static string SearchStreaming(string format, params object[] args)
        {
            if (format.StartsWith("/"))
                format = format.Substring(1);

            return String.Concat("https://stream.twitter.com/", String.Format(format, args));
        }

    }
}