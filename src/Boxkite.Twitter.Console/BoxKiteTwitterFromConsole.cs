// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Threading;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Console
{
    public class BoxKiteTwitterFromConsole
    {
        public static IUserStream userstream;
        public static ISearchStream searchstream;

        private static void Main(string[] args)
        {
            ConsoleOutput.PrintMessage("Welcome to BoxKite.Twitter Console");
            ConsoleOutput.PrintMessage("(control-c ends)");

            var tw = new TwitterConnection( new DesktopPlatformAdaptor());
            var x = tw.BeginAuthentication().Result;
            if (x)
            {
                System.Console.Write("PIN: ");
                var pin = System.Console.ReadLine();
                var y = tw.CompleteAuthentication(pin).Result;
                if (y != null)
                {
                    y.TimeLine.Subscribe(t => ConsoleOutput.PrintTweet(t, ConsoleColor.Green));
                    y.Start();

                    while (true)
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(2));
                    }

                }
            }

        }
        




        public static void PrintTwitterErrors(TwitterControlMessage tcm)
        {
            ConsoleOutput.PrintMessage("START: TWITTER CONTROL MESSAGE");
            ConsoleOutput.PrintError(String.Format("http reason: {0}", tcm.http_reason));
            ConsoleOutput.PrintError(String.Format("http status code: {0}", tcm.http_status_code));
            ConsoleOutput.PrintError(String.Format("twitter error code: {0}", tcm.twitter_error_code));
            ConsoleOutput.PrintError(String.Format("twitter error message: {0}", tcm.twitter_error_message));
            ConsoleOutput.PrintError(String.Format("API rates: {0}/{1} Resets {2}",
                tcm.twitter_rate_limit_remaining,
                tcm.twitter_rate_limit_limit, tcm.twitter_rate_limit_reset));
            ConsoleOutput.PrintMessage("END: TWITTER CONTROL MESSAGE");
        }

        private static void cancelStreamHandler(object sender, ConsoleCancelEventArgs e)
        {
            if (userstream != null)
                userstream.Stop();
            ConsoleOutput.PrintMessage("All finished.", ConsoleColor.Blue);
            Thread.Sleep(TimeSpan.FromSeconds(1.3));
        }
    }
}
