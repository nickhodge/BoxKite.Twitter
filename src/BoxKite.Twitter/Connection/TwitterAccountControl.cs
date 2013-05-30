// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Threading.Tasks;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter
{
    public partial class TwitterAccount : BindableBase
    {
        private readonly IPlatformAdaptor _platformAdaptor;
        public User accountDetails { get; set; }
        public TwitterCredentials _TwitterCredentials { get; set; }
        public AccountSettings accountSettings { get; set; }
        public IUserSession Session;
        public IUserStream UserStream;
        public ISearchStream SearchStream;

        private readonly TimeSpan _multiFetchBackoffTimer = new TimeSpan(1200);

#if (PORTABLE)
        public TwitterAccount(TwitterCredentials twitterCredentials, IPlatformAdaptor platformAdaptor)
        {
            _TwitterCredentials = twitterCredentials;
            _platformAdaptor = platformAdaptor;
            Session = new UserSession(_TwitterCredentials,_platformAdaptor);
        }
#elif (WINDOWS)        
        public TwitterAccount(TwitterCredentials twitterCredentials)
        {
            _TwitterCredentials = twitterCredentials;
            Session = new UserSession(_TwitterCredentials);
        }
#endif

        public async Task<bool> VerifyCredentials()
        {
            var checkedUser = await Session.GetVerifyCredentials();
            if (checkedUser.OK) // go deeper
            {
                accountSettings = await Session.GetAccountSettings();
                accountDetails = await Session.GetUserProfile(user_id: checkedUser.UserId);
                return accountSettings.OK && accountDetails.OK;
            }
            else
                return false; // return false here will abandon all hope
        }
    }
}
