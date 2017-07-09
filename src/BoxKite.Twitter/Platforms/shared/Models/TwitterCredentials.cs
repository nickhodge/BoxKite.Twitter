// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Runtime.Serialization;

namespace BoxKite.Twitter.Models
{
    [DataContract]
    public class TwitterCredentials
    {
        /// <summary>
        /// Have these Credentials been validated
        /// </summary>
        [DataMember]
        public bool Valid { get; set; }

        /// <summary>
        /// ConsumerKey (ClientKey) associated with these Credentials
        /// In Twitter, the Consumer is the API consuming/using the API requests
        /// </summary>
        [DataMember]
        public string ConsumerKey { get; set; }

        /// <summary>
        /// ConsumerSecret (ClientSecret) associated with these Credentials
        /// In Twitter, the Consumer is the API consuming/using the API requests
        /// </summary>
        [DataMember]
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// Application only OAuth2 BearerToken
        /// ref: https://dev.twitter.com/docs/auth/application-only-auth
        /// </summary>
        [DataMember]
        public string BearerToken { get; set; }

        /// <summary>
        /// Associated with these ConsumerKey/Secret and this user is a Token
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        /// <summary>
        /// Associated with these ConsumerKey/Secret and this user is a TokenSecret
        /// </summary>
        [DataMember]
        public string TokenSecret { get; set; }

        /// <summary>
        /// Twitter ScreenName associated with these Credentials
        /// </summary>
        [DataMember]
        public string ScreenName { get; set; }

        /// <summary>
        /// Twitter UserID associated with these Credentials
        /// </summary>
        [DataMember]
        public long UserID { get; set; }

        /// <summary>
        /// Unverified Credentials are not valid
        /// </summary>
        [IgnoreDataMember]
        public static TwitterCredentials _null = new TwitterCredentials { Valid = false };

        [IgnoreDataMember]
        public static TwitterCredentials Null
        {
            get { return _null; }
        }
    }
}
