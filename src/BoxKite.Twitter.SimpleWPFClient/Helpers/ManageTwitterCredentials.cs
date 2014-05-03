// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.IO;
using BoxKite.Twitter.Models;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Helpers
{
    public class ManageTwitterCredentials
    {
        // file name to save (securely) the credentials
        private const string BoxKiteTwitterCredentialsStore = "BoxKiteTwitterCredentials.bk";

        public static TwitterCredentials GetCredentialsFromFile()
        {
            var twitterCreds = new TwitterCredentials();
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), BoxKiteTwitterCredentialsStore);
            if (!File.Exists(fileName)) return twitterCreds;

            var json = File.ReadAllText(fileName);
            var clearText = json.DecryptString().ToInsecureString();

            // note that BoxKite.Twitter.Models.TwitterCredentials has DataContracts assigned
            twitterCreds = JsonConvert.DeserializeObject<TwitterCredentials>(clearText);
            return twitterCreds;
        }

        public static void SaveCredentialsToFile(TwitterCredentials tc)
        {
            var json = JsonConvert.SerializeObject(tc, Formatting.Indented);
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), BoxKiteTwitterCredentialsStore);
            var cypherText = json.ToSecureString().EncryptString();
            File.WriteAllText(fileName, cypherText);
        }

    }
}
