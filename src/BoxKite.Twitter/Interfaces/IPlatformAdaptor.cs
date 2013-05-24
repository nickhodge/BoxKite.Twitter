namespace BoxKite.Twitter.Interfaces
{
    public interface IPlatformAdaptor
    {
        void DisplayAuthInBrowser(string oauthPINunlockURL);
        void AssignKey(byte[] key);
        byte[] ComputeHash(byte[] buffer);
    }
}
