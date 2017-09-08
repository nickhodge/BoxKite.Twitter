// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

// This code is modified from a version supplied from @VikingCode aka Paul Jenkins!
// http://pastebin.com/Y2PbrbRy

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace BoxKite.Twitter
{
    public class WinUWPPlatformAdaptor : IPlatformAdaptor
    {
        private byte[] hmackey;


        public void DisplayAuthInBrowser(string oauthPINunlockURL)
        {
            throw new NotImplementedException();
        }

        public async Task<string> AuthWithBroker(string oauthuri, string callbackuri)
        {
            var StartUri = new Uri(oauthuri);
            var EndUri = new Uri(callbackuri);
            var WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        StartUri,
                                                        EndUri);
            return WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success ? WebAuthenticationResult.ResponseData.ToString() : null;
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