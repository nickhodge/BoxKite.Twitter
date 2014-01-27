// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;

namespace BoxKite.Twitter.Models
{
    public interface IStreamEvent
    {
        string EventName { get; set; }
        User TargetUser { get; set; }
        User SourceUser { get; set; }
        DateTimeOffset CreatedAt { get; set; }
    }
}
