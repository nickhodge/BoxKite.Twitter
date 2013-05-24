// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster

using System.Diagnostics;
using BoxKite.Twitter.Interfaces;
using System.Security.Cryptography;

namespace BoxKite.Console.Helpers
{
    public class DesktopPlatformAdaptor : IPlatformAdaptor
    {
        public void DisplayAuthInBrowser(string u)
        {
            Process.Start(u);
        }

        private HMACSHA1 _hmacsha1;

        public void AssignKey(byte[] key)
        {
            _hmacsha1 = new HMACSHA1(key);
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            return _hmacsha1.ComputeHash(buffer);
        }
    }
}
