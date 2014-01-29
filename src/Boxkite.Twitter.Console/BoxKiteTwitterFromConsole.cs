// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Threading;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Console
{
    public class BoxKiteTwitterFromConsole
    {
        public static TwitterConnection twitterConnection;
 
        private static void Main(string[] args)
        {
            ConsoleOutput.PrintMessage("Welcome to BoxKite.Twitter Console");
            ConsoleOutput.PrintMessage("(control-c ends)");
            System.Console.CancelKeyPress += cancelStreamHandler;

            var twittercredentials = ManageTwitterCredentials.GetTwitterCredentialsFromFile();

            if (twittercredentials == null)
            {
                // there are no Credentials on file, so create a new set
                // first, get the Twitter Client (also known as Consumer) Key and Secret from my service
                var twitterClientKeys = ClientKeyManager.GetTwitterClientKeys().Result;

                // make a new connection
                twitterConnection = new TwitterConnection(twitterClientKeys.Item1, twitterClientKeys.Item2);

                // PIN based authentication
                var oauth = twitterConnection.BeginAuthentication().Result;

                // if the response is null, something is wrong with the initial request to OAuth
                if (!string.IsNullOrWhiteSpace(oauth))
                {
                    ConsoleOutput.PrintMessage("Pin: ");
                    var pin = System.Console.ReadLine();
                    twittercredentials = twitterConnection.CompleteAuthentication(pin, oauth).Result;

                    ManageTwitterCredentials.SaveTwitterCredentialsToFile(twittercredentials);
                }
                else
                {
                    ConsoleOutput.PrintError("Cannot OAuth with Twitter");
                }
            }


            if (twittercredentials != null)
            {
                twitterConnection = new TwitterConnection(twittercredentials);

                twitterConnection.Start();

                ConsoleOutput.PrintMessage(twitterConnection.TwitterCredentials.ScreenName +
                                           " is authorised to use BoxKite.Twitter.");

                var session = twitterConnection.Session;
                var userstream = twitterConnection.UserStream;

                userstream.Events.Subscribe(
                    t => ConsoleOutput.PrintMessage(t.EventName));

                //userstream.Events.Subscribe(e => ConsoleOutput.PrintEvent(e, ConsoleColor.Yellow));
                //userstream.DirectMessages.Subscribe(
                //    d => ConsoleOutput.PrintDirect(d, ConsoleColor.Magenta, ConsoleColor.Black));
                //userstream.Start();

                while (userstream.IsActive)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }





                /*var x = session.SendTweet("d realnickhodge testing & ampersands");

                        if (x.IsFaulted)
                        {
                            ConsoleOutput.PrintMessage("bugger");
                        }

                        */



                //var locations = new List<string> { "150.700493", "-34.081953", "151.284828", "-33.593316" };
                //var locations = new List<string> { "-180", "-90", "180", "90" };
                // var track = new List<string> { "qanda" };



                //searchstream = session.StartSearchStream(track: track);
                /*
                        mainTwitterAccount.SearchTimeLine.Subscribe(t =>
                                                           {
                                                               ConsoleOutput.PrintTweet(t, ConsoleColor.Green);
                                                           });

                        mainTwitterAccount.StartSearch("twitter");

                        while (true)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(0.5));
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

                /*
                mainTwitterAccount.TimeLine.Subscribe(t => ConsoleOutput.PrintTweet(t));
                mainTwitterAccount.Mentions.Subscribe(
                    t => ConsoleOutput.PrintTweet(t, ConsoleColor.White, ConsoleColor.DarkGray));

                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }

                Console.ConsoleOutput.PrintMessage("Event stream has stoppped.");

                         var locations = new List<string> { "-34.081953", "150.700493", "-33.593316", "151.284828" };
                            searchstream = session.StartSearchStream(locations: locations);
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

            ConsoleOutput.PrintMessage("All Finished");
            System.Console.ReadLine();

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
            ConsoleOutput.PrintMessage("All finished.", ConsoleColor.Blue);
            Thread.Sleep(TimeSpan.FromSeconds(1.3));
        }
    }
}
