using System;
using System.IO;
using System.Reflection;

namespace BoxKite.Twitter.Console.Helpers
{
    public static class FilesHelper
    {
        public static string FromFile(string path)
        {
            return Path.Combine(AssemblyDirectory, path);
        }

        private static string AssemblyDirectory
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
