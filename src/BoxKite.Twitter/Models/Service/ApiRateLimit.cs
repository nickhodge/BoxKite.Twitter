using System;
using System.Collections.Generic;
using BoxKite.Twitter.Extensions;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models.Service
{
    public class ApiRateStatusCall : TwitterControlBase
    {
        [JsonIgnore]
        public Dictionary<string, ApiRateStatus> ApiRateStatuses { get; set; } 
    }

    public class ApiRateStatus
    {
        public string apipath { get; set; }
        public int remaining { get; set; }
        public int limit { get; set; }
        public double reset { get; set; }
        public DateTime ResetTime
        {
            get { return reset.FromUnixEpochSeconds(); }
        }
    }
}