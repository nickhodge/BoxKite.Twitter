// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
// UNLESS NOTED ALTERNATIVE SOURCE

using System;

namespace BoxKite.Twitter.Extensions
{
    public static class DateTimeExtensions
    {
        private const int SECOND = 1;
        private const int MINUTE = 60 * SECOND;
        private const int HOUR = 60 * MINUTE;
        private const int DAY = 24 * HOUR;
        private const int MONTH = 30 * DAY;

        public static DateTime FromUnixEpochSeconds(this double unixEpochSeconds)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dtDateTime.AddSeconds(unixEpochSeconds).ToLocalTime();
        }

        public static string ToShortTimeString(this DateTimeOffset date)
        {
            return string.Format("h:mm tt", date);
        }

        public static string ToFriendlyText(this DateTimeOffset pastTime, string friendlyDistance)
        {
            return ToFriendlyText(pastTime, DateTimeOffset.UtcNow) + " " + friendlyDistance;
        }

        public static string ToFriendlyText(this DateTimeOffset pastTime)
        {
            return ToFriendlyText(pastTime, DateTimeOffset.UtcNow);
        }

        public static string ToFriendlyText(this DateTimeOffset dateTime, DateTimeOffset currentTime)
        {
            bool isFuture = (DateTime.UtcNow.Ticks < dateTime.Ticks);

            // Nick Hodge Note; 30th August 2013
            // slighly modified from the original because if the clock of the local machine 
            // has drifted forward (ie "isFuture"), then show "Just Now" rather than a 
            // 'in 13 seconds' ... which is weird for users and tweets.
            // to return to normal operation, take out the || isFuture
            // I've left the 'inFuture' in the other if statements.

            if (DateTime.UtcNow.Ticks == dateTime.Ticks || isFuture)
            {
                return "Just now";
            } 

            var ts = DateTime.UtcNow.Ticks < dateTime.Ticks ? new TimeSpan(dateTime.Ticks - DateTime.UtcNow.Ticks) : new TimeSpan(DateTime.UtcNow.Ticks - dateTime.Ticks);

            double delta = Math.Round(ts.TotalSeconds);

            if (delta < 1)
            {
                return "Just now";
            }

            if (delta < 1 * MINUTE)
            {
                return isFuture ? "in " + (ts.Seconds == 1 ? "one second" : ts.Seconds + " seconds") : ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 2 * MINUTE)
            {
                return isFuture ? "in a minute" : "a minute ago";
            }
            if (delta < 45 * MINUTE)
            {
                return isFuture ? "in " + ts.Minutes + " minutes" : ts.Minutes + " minutes ago";
            }
            if (delta < 90 * MINUTE)
            {
                return isFuture ? "in an hour" : "an hour ago";
            }
            if (delta < 24 * HOUR)
            {
                return isFuture ? "in " + ts.Hours + " hours" : ts.Hours + " hours ago";
            }
            if (delta < 48 * HOUR)
            {
                return isFuture ? "tomorrow" : "yesterday";
            }
            if (delta < 30 * DAY)
            {
                return isFuture ? "in " + ts.Days + " days" : ts.Days + " days ago";
            }
            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return isFuture ? "in " + (months <= 1 ? "one month" : months + " months") : months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return isFuture ? "in " + (years <= 1 ? "one year" : years + " years") : years <= 1 ? "one year ago" : years + " years ago";
            }
        }

        public static string FormatDayOfMonth(this DateTimeOffset dateTime)
        {
            var dayOfMonth = dateTime.Day;

            if (dayOfMonth == 1 || dayOfMonth == 21 || dayOfMonth == 31)
                return string.Format("{0}st", dayOfMonth);

            if (dayOfMonth == 2 || dayOfMonth == 22)
                return string.Format("{0}nd", dayOfMonth);

            if (dayOfMonth == 3 || dayOfMonth == 23)
                return string.Format("{0}rd", dayOfMonth);

            return string.Format("{0}th", dayOfMonth);
        }
    }

}
