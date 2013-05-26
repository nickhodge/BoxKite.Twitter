// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using System.Runtime.Serialization;

namespace BoxKite.Twitter.Models
{
    [DataContract]
    public class TwitterCredentials
    {
        [DataMember]
        public bool Valid { get; set; }
        [DataMember]
        public string ConsumerKey { get; set; }
        [DataMember]
        public string ConsumerSecret { get; set; }
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string TokenSecret { get; set; }
        [DataMember]
        public string ScreenName { get; set; }
        [DataMember]
        public int UserID { get; set; }

        [IgnoreDataMember]
        static TwitterCredentials _null = new TwitterCredentials { Valid = false };

        [IgnoreDataMember]
        public static TwitterCredentials Null
        {
            get { return _null; }
        }
    }
}
