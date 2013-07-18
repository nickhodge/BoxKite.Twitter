// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using Microsoft.SqlServer.Server;
using Reactive.EventAggregator;

namespace BoxKite.Twitter.Console
{
    public class BoxKiteTwitterFromConsole
    {
        public static IUserStream userstream;
        public static ISearchStream searchstream;
        public static TwitterConnection twitterConnection;
        public static TwitterAccount mainTwitterAccount;

        private static void Main(string[] args)
        {
            ConsoleOutput.PrintMessage("Welcome to BoxKite.Twitter Console");
            ConsoleOutput.PrintMessage("(control-c ends)");
            System.Console.CancelKeyPress += new ConsoleCancelEventHandler(cancelStreamHandler);

            //var twittercredentials = ManageTwitterCredentials.MakeConnection();

            twitterConnection = new TwitterConnection("3izxqWiej34yTlofisw","uncicYQtDx5SoWth1I9xcn5vrpczUct1Oz9ydwTY4");

            DoPINDisplay(twitterConnection);
            ConsoleOutput.PrintMessage("Pin: ");
            var pin = System.Console.ReadLine();
            var mainTwitterAccount = AuthPIN(pin, twitterConnection).Result;

            if (mainTwitterAccount != null)
            {
                mainTwitterAccount.Start();

                var session = mainTwitterAccount.Session;

                ConsoleOutput.PrintMessage(mainTwitterAccount._TwitterCredentials.ScreenName + " is authorised to use BoxKite.Twitter.");

                /*var x = session.SendTweet("d realnickhodge testing & ampersands");

                        if (x.IsFaulted)
                        {
                            ConsoleOutput.PrintMessage("bugger");
                        }

                        */

                /*
                        
                        //var locations = new List<string> { "150.700493", "-34.081953", "151.284828", "-33.593316" };
                        //var locations = new List<string> { "-180", "-90", "180", "90" };
                        var track = new List<string> { "qanda" };

                        searchstream = session.StartSearchStream(track:track);

                        //searchstream = session.StartSearchStream(track: track);

                        var tweetcount = 0;
                        double minutes = 10;
                        searchstream.FoundTweets.Subscribe(t =>
                                                           {
                                                               ConsoleOutput.PrintTweet(t, ConsoleColor.Green);
                                                               tweetcount++;
                                                           });
                        searchstream.Start();

                        while (searchstream.IsActive)
                        {
                            Thread.Sleep(TimeSpan.FromMinutes(minutes));
                            searchstream.Stop();
                            double twpm = tweetcount / minutes;
                            double twps = twpm / 60;
                            ConsoleOutput.PrintMessage(String.Format("Tweets per minute: {0}", twpm.ToString("0,0.00")), ConsoleColor.Cyan);
                            ConsoleOutput.PrintMessage(String.Format("Tweets per second: {0}", twps.ToString("0,0.00")), ConsoleColor.Cyan);
                        }

                        */

                /*
                        //var fileName = @"C:\Users\Nick\Pictures\My Online Avatars\666.jpg";
                        //if (File.Exists(fileName))
                        //{
                        //var newImage = File.ReadAllBytes(fileName);

                        var sr = FilesHelper.FromFile("sampleimage\\MaggieThatcherRules.jpg");

                        // var x = session.SendTweetWithImage("Testing Image Upload. You can Ignore", Path.GetFileName(fileName),newImage).Result;

                        using (var fs = new FileStream(sr, FileMode.Open, FileAccess.Read))
                        {                               
                            
                            //var x = session.ChangeAccountProfileImage("MaggieThatcherRules.jpg", fs).Result;

                            var x = session.SendTweetWithImage("Maggies Rules", "maggie.jpg", fs).Result;

                            if (x.twitterFaulted)
                            {
                                PrintTwitterErrors(x.twitterControlMessage);
                            }
                            else
                            {
                                ConsoleOutput.PrintTweet(x, ConsoleColor.Green);
                            }

                        }

                        */

                mainTwitterAccount.TimeLine.Subscribe(t => ConsoleOutput.PrintTweet(t));

                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }

                Console.ConsoleOutput.PrintMessage("Event stream has stoppped.");

                /*
                       userstream = session.GetUserStream();
                         userstream.Tweets.Subscribe(t => ConsoleOutput.PrintTweet(t, ConsoleColor.Green));
                         userstream.Events.Subscribe(e => ConsoleOutput.PrintEvent(e, ConsoleColor.Yellow));
                         userstream.DirectMessages.Subscribe(
                             d => ConsoleOutput.PrintDirect(d, ConsoleColor.Magenta, ConsoleColor.Black));
                         userstream.Start();

                         while (userstream.IsActive)
                         {
                             Thread.Sleep(TimeSpan.FromSeconds(0.5));
                         } */



                /*
                         * 
                         * //var locations = new List<string> { "-34.081953", "150.700493", "-33.593316", "151.284828" };
                            //searchstream = session.StartSearchStream(locations: locations);
                            searchstream = session.StartSearchStream(track: "hazel");
                            searchstream.FoundTweets.Subscribe(t => ConsoleOutput.PrintTweet(t, ConsoleColor.Green));
                            searchstream.Start();

                            while (searchstream.IsActive)
                            {
                                Thread.Sleep(TimeSpan.FromMinutes(1));
                                var sr = new StreamSearchRequest();
                                sr.tracks.Add("xbox");
                                searchstream.SearchRequests.Publish(sr);
                            }
                         * 
                         */


                /*
                        var x = session.GetMentions(count:100).Result;

                        foreach (var tweet in x)
                        {
                            ConsoleOutputPrintTweet(tweet);
                        }
                        
                    
                         session.GetFavourites(count: 10)
                            .Subscribe(t => ConsoleOutputPrintTweet(t, ConsoleColor.White, ConsoleColor.Black));
                        */

            }
            else
            {
                ConsoleOutput.PrintMessage("Credentials could not be verified.", ConsoleColor.Red);
            }

            ConsoleOutput.PrintMessage("All Finished");
            System.Console.ReadLine();

        }

        public static async void DoPINDisplay(TwitterConnection twitterConnection)
        {
            await twitterConnection.BeginAuthentication();
        }

        public static async Task<TwitterAccount> AuthPIN(string authPIN, TwitterConnection twitterConnection)
        {
            // after entering the PIN, and clicking OK, this method is run
            if (!string.IsNullOrWhiteSpace(authPIN))
            {
                var twitteraccount = await twitterConnection.CompleteAuthentication(authPIN);
                if (twitteraccount == null) // oops, not a good auth
                {
                    return null;
                }
                return twitteraccount;
            }
            return null;
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
