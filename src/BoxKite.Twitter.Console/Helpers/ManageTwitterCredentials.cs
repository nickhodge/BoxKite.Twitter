// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster

using System;
using System.IO;
using BoxKite.Twitter.Models;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Console
{
    public class ManageTwitterCredentials
    {
        private const string BoxKiteTwitterCredentialsStore = "BoxKiteTwitterCredentials.bk";

        public static TwitterCredentials GetTwitterCredentialsFromFile()
        {
            var twitterCreds = new TwitterCredentials();
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),BoxKiteTwitterCredentialsStore);
            if (File.Exists(fileName))
            {
                twitterCreds = GetCredentialsFromFile(fileName);
            }
            return twitterCreds.Valid ? twitterCreds : null;
        }

        public static void SaveTwitterCredentialsToFile(TwitterCredentials twitterCreds)
        {
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), BoxKiteTwitterCredentialsStore);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            SaveCredentialsToFile(fileName, twitterCreds);
        }


        private static TwitterCredentials GetCredentialsFromFile(string fileName)
        {
            var json = File.ReadAllText(fileName);
            var clearText = json.DecryptString().ToInsecureString();
            return JsonConvert.DeserializeObject<TwitterCredentials>(clearText);
        }

        private static void SaveCredentialsToFile(string fileName, TwitterCredentials tc)
        {
            var json = JsonConvert.SerializeObject(tc, Formatting.Indented);
            var cypherText = json.ToSecureString().EncryptString();
            File.WriteAllText(fileName, cypherText);
        }

    }
}
