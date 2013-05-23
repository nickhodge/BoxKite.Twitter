namespace BoxKite.Twitter.Interfaces
{
    public interface IHMACSHA1
    {
        void AssignKey(byte[] key);
        byte[] ComputeHash(byte[] buffer);
    }
}
