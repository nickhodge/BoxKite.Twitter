// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BoxKite.Twitter.Console.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Console
{
    public class BoxKiteTwitterFromConsole_AppAuth
    {
        public static TwitterConnection twitterConnection;

        private static void Main(string[] args)
        {
            ConsoleOutput.PrintMessage("Welcome to BoxKite.Twitter Console (App Auth Tests)");
            ConsoleOutput.PrintMessage("(control-c ends)");
            System.Console.CancelKeyPress += cancelStreamHandler;

            twitterConnection = new TwitterConnection("3izxqWiej34yTlofisw", "uncicYQtDx5SoWth1I9xcn5vrpczUct1Oz9ydwTY4");

            twitterConnection.StartSearchStreaming("v8sc");
            twitterConnection.SearchTimeLine.Subscribe(t => ConsoleOutput.PrintTweet(t));

            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }

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

        private static void cancelStreamHandler(object sender, ConsoleCancelEventArgs e)
        {
            ConsoleOutput.PrintMessage("All finished.", ConsoleColor.Blue);
            Thread.Sleep(TimeSpan.FromSeconds(1.3));
        }
    }
}
