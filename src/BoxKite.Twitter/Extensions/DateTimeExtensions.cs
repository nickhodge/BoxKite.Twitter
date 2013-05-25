// (c) 2012// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

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
            var currentOffset = DateTimeOffset.Now.Offset;

            var timeSince = currentTime - pastTime;
            if (timeSince >= new TimeSpan(24, 0, 0))
            {
                var translated = pastTime.ToOffset(currentOffset);

                if (timeSince < new TimeSpan(48, 0, 0))
                {
                    return "Yesterday, " + translated.ToString("hh:mm:ss tt");
                }

                return translated.ToString("hh:mm:ss tt, dd/MM/yy");
            }

            if (timeSince < new TimeSpan(0, 1, 0))
            {
                return "< 1 minute ago";
            }

            if (timeSince < new TimeSpan(1, 0, 0))
            {
                return timeSince > new TimeSpan(0, 1, 59)
                           ? timeSince.Minutes + " minutes ago"
                           : timeSince.Minutes + " minute ago";
            }

            if (timeSince < new TimeSpan(24, 0, 0))
            {
                return timeSince > new TimeSpan(1, 59, 59)
                           ? timeSince.Hours + " hours ago"
                           : timeSince.Hours + " hour ago";
            }
            return pastTime.ToOffset(currentOffset).ToString("hh:mm:ss tt");
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
