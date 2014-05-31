// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using BoxKite.Twitter.Helpers;

namespace BoxKite.Twitter.Models
{
/*
      {StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
        {
          pragma: no-cache
          Cache-Control: no-store, must-revalidate, no-cache, pre-check=0, post-check=0
          Date: Sun, 12 May 2013 04:23:10 GMT
          Set-Cookie: lang=en
          Set-Cookie: guest_id=v1%3A136833259014414406; Domain=.twitter.com; Path=/; Expires=Tue, 12-May-2015 04:23:10 UTC
          Server: tfe
          status: 200 OK
          strict-transport-security: max-age=631138519
          x-access-level: read-write-directmessages
          x-frame-options: SAMEORIGIN
          x-rate-limit-limit: 15
          x-rate-limit-remaining: 13
          x-rate-limit-reset: 1368333446
          x-transaction: 0f618e4154f3be73
          x-xss-protection: 1; mode=block
          Content-Length: 169962
          Content-Type: application/json; charset=utf-8
          Expires: Tue, 31 Mar 1981 05:00:00 GMT
          Last-Modified: Sun, 12 May 2013 04:23:10 GMT
        }}
*/
    public abstract class TwitterControlBase
    {
        public bool twitterFaulted { get; set; }
        public bool OK {
            get 
            {
                return !twitterFaulted;
            }
        }
        public bool twitterOK
        {
            get
            {
                return !twitterFaulted;
            }
        }
        public TwitterControlMessage twitterControlMessage { get; set; }
    }

    public class TwitterControlMessage
    {
        public int http_status_code { get; set; } // int as I cannot work out how to extend HttpStatusCodes with 429 etc
        public string http_reason { get; set; }

        public int twitter_rate_limit_limit { get; set; }
        public int twitter_rate_limit_remaining { get; set; }
        public DateTime twitter_rate_limit_reset { get; set; }

        public int twitter_error_code { get; set; }
        public string twitter_error_message { get; set; }

        public string twitter_mediaratelimit_class { get; set; }
        public int twitter_mediaratelimit_limit { get; set; }
        public int twitter_mediaratelimit_remaining { get; set; }
    }
}
