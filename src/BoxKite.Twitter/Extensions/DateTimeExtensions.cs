// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
// UNLESS NOTED ALTERNATIVE SOURCE

using System;

namespace BoxKite.Twitter.Extensions
{
    public static class DateTimeExtensions
    {
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

        public static string ToFriendlyText(this DateTimeOffset pastTime, DateTimeOffset currentTime)
        {
            // this is LITERALLY from stackoverflow
            // ref: http://stackoverflow.com/questions/11/how-do-i-calculate-relative-time
            // no, really

            var ts = new TimeSpan(currentTime.Ticks - pastTime.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 30)
            {
//                return ts.Seconds == 1 ? "one second ago" : ts.Seconds
//                        + " seconds ago";
                return "Just now";
            } if (delta < 60)
            {
                return "Less than a minute ago";
            } if (delta < 90)
            {
                return "About a minute ago";
            } if (delta < 120)
            {
                return "a minute ago";
            }
            if (delta < 2700) // 45 * 60
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 5400) // 90 * 60
            {
                return "an hour ago";
            }
            if (delta < 86400)
            { // 24 * 60 * 60
                return ts.Hours + " hours ago";
            }
            if (delta < 172800)
            { // 48 * 60 * 60
                return "yesterday";
            }
            if (delta < 2592000)
            { // 30 * 24 * 60 * 60
                return ts.Days + " days ago";
            }
            if (delta < 31104000)
            { // 12 * 30 * 24 * 60 * 60
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
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
