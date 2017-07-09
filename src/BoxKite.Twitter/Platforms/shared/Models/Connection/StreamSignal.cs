// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

namespace BoxKite.Twitter.Models
{
    /// <summary>
    /// Add some semantics to signalling in Observables
    /// </summary>
    public enum StreamSignal
    {
        None = 0,
        Started = 1,
        Stopped = 2,
        Completed = 3,
        PleaseStart = 4,
        PleaseStop = 5
    }
}