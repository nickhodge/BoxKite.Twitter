// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace BoxKite.Twitter.Extensions
{
    public static class StringExtensions
    {
        public static String ToBase64String(this String source)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
        }

        public static string HTMLDecode(this string sourceString)
        {
            return WebUtility.HtmlDecode(sourceString);
        }

        public static string UrlEncode(this string inputString)
        {
            return Uri.EscapeDataString(inputString);
        }


        /// <summary>
        /// Remove last character of a string
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string TrimLastChar(this string sourceString)
        {
            return sourceString.Remove(sourceString.Length - 1, 1);
        }

        /// <summary>
        /// Sun Apr 15 02:31:50 +0000 2012 to DateTime
        /// </summary>
        /// <param name="date">In Sun Apr 15 02:31:50 +0000 2012 format</param>
        /// <returns></returns>
        public static DateTime ParseDateTime(this string date)
        {
            var tokenizer = date.Split(' ');

            //var dayOfWeek = tokenizer[0];
            var month = tokenizer[1];
            var dayInMonth = tokenizer[2];
            var time = tokenizer[3];
            //var offset = tokenizer[4];
            var year = tokenizer[5];

            var dateTime = string.Format("{0}-{1}-{2} {3}", dayInMonth, month, year, time);

            return DateTime.Parse(dateTime);
        }

        // 
        /// <summary>
        /// Sun Apr 15 02:31:50 +0000 2012 to timeoffset to now
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <remarks> based on http://stackoverflow.com/a/4975344 </remarks>
        public static DateTimeOffset ToDateTimeOffset(this string date)
        {
            DateTimeOffset timestamp;
            if (DateTimeOffset.TryParseExact(date, "ddd MMM dd HH:mm:ss K yyyy", null, DateTimeStyles.None, out timestamp))
            {
                return timestamp;
            }

            // Sun, 15 Apr 2012 02:38:21 +0000
            if (DateTimeOffset.TryParseExact(date, "ddd, dd MMM yyyy HH:mm:ss K", null, DateTimeStyles.None, out timestamp))
            {
                return timestamp;
            }

            return DateTimeOffset.MinValue;
        }

        public static string TrimAndTruncate(this string inputString, int targetLength)
        {
            return inputString.Length <= targetLength ? inputString : inputString.Trim().Substring(0, targetLength);
        }

    }
}
