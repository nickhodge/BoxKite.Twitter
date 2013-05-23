namespace BoxKite.Twitter.Extensions
{
    public static class Api
    {
        public static string Resolve(string format, params object[] args)
        {
            if (format.StartsWith("/"))
                format = format.Substring(1);

            return string.Concat("https://api.twitter.com/", string.Format(format, args));
        }

        public static string Upload(string format, params object[] args)
        {
            if (format.StartsWith("/"))
                format = format.Substring(1);

            return string.Concat("http://upload.twitter.com/", string.Format(format, args));
        }

        public static string UserStreaming(string format, params object[] args)
        {
            if (format.StartsWith("/"))
                format = format.Substring(1);

            return string.Concat("https://userstream.twitter.com/", string.Format(format, args));
        }


        public static string SearchStreaming(string format, params object[] args)
        {
            if (format.StartsWith("/"))
                format = format.Substring(1);

            return string.Concat("https://stream.twitter.com/", string.Format(format, args));
        }

    }
}