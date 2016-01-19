// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL
using System;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Console
{
    public class TwitterLiveFireDoUserAuthLoginFlow
    {
        private static TwitterCredentials twitterCredentials;
        public static void Main(string[] args)
        {
            ConsoleOutput.PrintMessage("BoxKite.Twitter Live Fire Tests (Initiate User Auth)");
            ConsoleOutput.PrintMessage("(control-c ends at anytime)");

            var twitterConnection = new TwitterConnection("3izxqWiej34yTlofisw", "uncicYQtDx5SoWth1I9xcn5vrpczUct1Oz9ydwTY4");

            var authString = twitterConnection.BeginUserAuthentication().Result;
 
            // if the response is null, something is wrong with the initial request to OAuth
            if (!string.IsNullOrWhiteSpace(authString))
            {
                ConsoleOutput.PrintMessage("Pin: ");
                var pin = System.Console.ReadLine();
                twitterCredentials = twitterConnection.CompleteUserAuthentication(pin, authString).Result;
            }

            if (twitterCredentials.Valid)
            {
                ConsoleOutput.PrintMessage(twitterCredentials.ScreenName + " is authorised to use BoxKite.Twitter.");
            }
            else
            {
                ConsoleOutput.PrintMessage("Something Went Wrong during User Authentication Dance.");              
            }
            ConsoleOutput.PrintMessage("Press Return to close window");
            System.Console.ReadLine();
        }

        public static void PrintTwitterErrors(TwitterControlMessage tcm)
        {
            ConsoleOutput.PrintMessage("START: TWITTER CONTROL MESSAGE");
            ConsoleOutput.PrintError($"http reason: {tcm.http_reason}");
            ConsoleOutput.PrintError($"http status code: {tcm.http_status_code}");
            ConsoleOutput.PrintError($"twitter error code: {tcm.twitter_error_code}");
            ConsoleOutput.PrintError($"twitter error message: {tcm.twitter_error_message}");
            ConsoleOutput.PrintError(
                $"API rates: {tcm.twitter_rate_limit_remaining}/{tcm.twitter_rate_limit_limit} Resets {tcm.twitter_rate_limit_reset}");
            ConsoleOutput.PrintMessage("END: TWITTER CONTROL MESSAGE");
        }
    }
}
