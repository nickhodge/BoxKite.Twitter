// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL


using System.Threading.Tasks;

namespace BoxKite.Twitter
{
    public interface IPlatformAdaptor
    {
        void DisplayAuthInBrowser(string oauthPINunlockURL);
        Task<string> AuthWithBroker(string authuri, string callbackuri);
        void AssignKey(byte[] key);
        byte[] ComputeHash(byte[] buffer);
    }
}
