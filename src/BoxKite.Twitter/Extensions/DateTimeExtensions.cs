// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
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

        
    }

}
