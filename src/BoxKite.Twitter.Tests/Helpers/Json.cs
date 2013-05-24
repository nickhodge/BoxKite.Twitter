// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BoxKite.Twitter.Tests
{
    public static class Json
    {
        public static async Task<string> FromFile(string path)
        {
            var filePath = Path.Combine(AssemblyDirectory, path);
            var reader = new StreamReader(filePath);
            return await reader.ReadToEndAsync();
        }

        static public string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
