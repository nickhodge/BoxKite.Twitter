using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BoxKite.Twitter.Interfaces;
using System.Security.Cryptography;

namespace BoxKite.Console.Helpers
{
    public class DesktopHMACSHA1 : IHMACSHA1
    {
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
