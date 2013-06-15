// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.IO;
using Windows.Security.Credentials;
using BoxKite.Twitter.Models;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Helpers
{
    public class ManageTwitterCredentials
    {
        private const string BoxKiteTwitterCredentialsStoreKey = "BoxKiteTwitterCredentials";

        public static TwitterCredentials GetCredentialsFromFile()
        {
            var twitterCreds = new TwitterCredentials();
            try
            {
                var vault = new PasswordVault();
                var creds = vault.FindAllByResource(BoxKiteTwitterCredentialsStoreKey);
                foreach (var c in (IEnumerable<PasswordCredential>)creds)
                {
                    twitterCreds = JsonConvert.DeserializeObject<TwitterCredentials>(c.ToString());
                } 
            }
            catch (Exception)
            {
                
            }
            return twitterCreds;
        }

        public static void SaveCredentialsToFile(TwitterCredentials tc)
        {
            var json = JsonConvert.SerializeObject(tc, Formatting.Indented);
            var vault = new PasswordVault();
            var c = new PasswordCredential(BoxKiteTwitterCredentialsStoreKey, json, ""); 
            vault.Add(c);
        }

    }
}
