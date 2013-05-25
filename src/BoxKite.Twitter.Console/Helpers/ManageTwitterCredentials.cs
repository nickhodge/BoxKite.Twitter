// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster

using System;
using System.IO;
using BoxKite.Twitter.Models;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Console
{
    public class ManageTwitterCredentials
    {
        private const string BoxKiteTwitterCredentialsStore = "BoxKiteTwitterCredentials.bk";

        public static TwitterCredentials MakeConnection()
        {
            var twitterCreds = new TwitterCredentials();
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),BoxKiteTwitterCredentialsStore);
            if (File.Exists(fileName))
            {
                twitterCreds = GetCredentialsFromFile(fileName);
            }
            if (twitterCreds.Valid) return twitterCreds;

            twitterCreds = TwitterConnection.GetTwitterCredentials();
            SaveCredentialsToFile(fileName, twitterCreds);

            return twitterCreds;
        }

        public static TwitterCredentials GetCredentialsFromFile(string fileName)
        {
            var json = File.ReadAllText(fileName);
            var clearText = json.DecryptString().ToInsecureString();
            return JsonConvert.DeserializeObject<TwitterCredentials>(clearText);
        }

        public static void SaveCredentialsToFile(string fileName, TwitterCredentials tc)
        {
            var json = JsonConvert.SerializeObject(tc, Formatting.Indented);
            var cypherText = json.ToSecureString().EncryptString();
            File.WriteAllText(fileName, cypherText);
        }

    }
}
