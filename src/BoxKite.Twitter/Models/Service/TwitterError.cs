// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL
namespace BoxKite.Twitter.Models
{
    public class TwitterError
    {
        public Error[] errors { get; set; }
    }

    public class Error
    {
        public string message { get; set; }
        public int code { get; set; }
    }
}
