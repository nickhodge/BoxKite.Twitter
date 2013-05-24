// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL


namespace BoxKite.Twitter.Interfaces
{
    public interface IPlatformAdaptor
    {
        void DisplayAuthInBrowser(string oauthPINunlockURL);
        void AssignKey(byte[] key);
        byte[] ComputeHash(byte[] buffer);
    }
}
