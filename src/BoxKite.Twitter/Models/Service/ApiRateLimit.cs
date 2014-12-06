// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using BoxKite.Twitter.Extensions;

namespace BoxKite.Twitter.Models
{
    public class ApiRateStatusResponse : TwitterControlBase
    {
        public string APIContext { get; set; }
        public Dictionary<string, APIRateStatus> APIRateStatuses { get; set; } 
    }

    public class APIRateStatus
    {
        public string APIPath { get; set; }
        public int Remaining { get; set; }
        public int Limit { get; set; }
        public double Reset { get; set; }
        public DateTimeOffset ResetTime
        {
            get { return Reset.FromUnixEpochSeconds(); }
        }
    }
}