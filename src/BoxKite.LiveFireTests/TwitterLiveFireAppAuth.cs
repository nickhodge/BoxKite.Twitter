// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL
using System;
using System.Collections.Generic;
using System.Threading;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Console
{
    public class TwitterLiveFireAppAuth
    {
        public static TwitterConnection twitterConnection;

        public static void Main(string[] args)
        {
            ConsoleOutput.PrintMessage("BoxKite.Twitter Live Fire Tests (Application Auth)");
            ConsoleOutput.PrintMessage("(control-c ends at anytime)");

            System.Console.CancelKeyPress += cancelStreamHandler;

            twitterConnection = new TwitterConnection("3izxqWiej34yTlofisw", "uncicYQtDx5SoWth1I9xcn5vrpczUct1Oz9ydwTY4");

            // put the test series number you wish to run into the init of the array
            // then in each test group, put the numbers of the tests you would like to run
            // NOTE: some tests require a previous test to work successfully
            // NOTE: some tests post/delete items. This *is* a live fire test!

            var testSeriesToRun = new List<int> {12};

            // Calls tested by Test Series
            // series 12=> 5 (Application Only Auth tests)
            // =============
            // TOTAL       5

            // Test Series 12 Application Only Auth tests

            if (testSeriesToRun.Contains(12))
            {
                var cmbs = new ApplicationOnlyAuthFireTests();
                var testResult12 = cmbs.DoApplicationOnlyAuthFireTests(twitterConnection, new List<int> { 2 }).Result;

                if (testResult12)
                {
                    ConsoleOutput.PrintMessage(
                        $"12.0 Application Auth Tests Status: {testResult12}",
                        ConsoleColor.White);
                }
                else
                {
                    ConsoleOutput.PrintMessage(
                        $"12.0 Application Auth Tests Status: {testResult12}",
                        ConsoleColor.Red);
                }
            }
            ConsoleOutput.PrintMessage("Press Return to close window");
            System.Console.ReadLine();
        }

        private static void cancelStreamHandler(object sender, ConsoleCancelEventArgs e)
        {
            if (twitterConnection != null)
                twitterConnection.StopUserStreaming();
            ConsoleOutput.PrintMessage("All finished.", ConsoleColor.Blue);
            Thread.Sleep(TimeSpan.FromSeconds(1.3));
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
