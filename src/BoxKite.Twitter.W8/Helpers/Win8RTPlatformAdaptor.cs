// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

// This code is from @VikingCode aka Paul Jenkins!
// http://pastebin.com/Y2PbrbRy

using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace BoxKite.Twitter
{
    public class Win8RTPlatformAdaptor : IPlatformAdaptor
    {
        private byte[] hmackey;

        public void DisplayAuthInBrowser(string u)
        {
            //   Process.Start(u);
        }

        public void AssignKey(byte[] key)
        {
            hmackey = key;
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            var crypt = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            var sigBuffer = CryptographicEngine.Sign(crypt.CreateKey(CryptographicBuffer.CreateFromByteArray(hmackey)), CryptographicBuffer.CreateFromByteArray(buffer));
            return sigBuffer.ToArray();
        }
    }
}