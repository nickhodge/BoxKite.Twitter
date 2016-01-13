// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public interface ITwitterConnection
    {
        IPlatformAdaptor PlatformAdaptor { get; set; }

        User AccountDetails { get; set; }
        TwitterCredentials TwitterCredentials { get; set; }
        AccountSettings AccountSettings { get; set; }

        ITwitterSession TwitterSession { get; }

        IApplicationSession ApplicationSession { get; set; }
        IUserSession UserSession { get; set; }
        IUserStream UserStream { get; set; }
        ISearchStream SearchStream { get; set; }

        Task<string> BeginUserAuthentication();
        Task<TwitterCredentials> CompleteUserAuthentication(string pin, string oAuthToken);
        Task<TwitterCredentials> XAuthentication(string xauthusername, string xauthpassword);
        Task<bool> VerifyUserCredentials();


        long HomeTimeLineLargestSeenId { get; set; }
        long DirectMessagesReceivedLargestSeenId { get; set; }
        long DirectMessagesSentLargestSeenId { get; set; }
        long RetweetsOfMeLargestSeenId { get; set; }
        long MentionsOfMeLargestSeenId { get; set; }
        long MyTweetsLargestSeenId { get; set; }

        IObservable<Tweet> TimeLine { get; }
        IObservable<Tweet> RetweetsOnTimeLine { get; }
        IObservable<Tweet> Mentions { get; }
        IObservable<Tweet> MyTweets { get; }
        IObservable<DirectMessage> DirectMessages { get; }
        IObservable<User> UsersSeen { get; }
        IObservable<DeleteEventStatus> StreamDeleteEvent { get; }
        IObservable<long> TweetsSeen { get; }

        void StartUserStreaming();
        void StopUserStreaming();

        IObservable<IStreamEvent> StreamEvents { get; }

        long _searchLargestSeenId { get; set; }
        IObservable<Tweet> SearchTimeLine { get; }
        void StartSearchStreaming(string textToSearch);
        void StopSearchStreaming();
    }
}
