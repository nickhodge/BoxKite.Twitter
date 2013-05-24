// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Threading;
using BoxKite.Console.Helpers;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models.Stream;
using BoxKite.Twitter.Modules;
using BoxKite.Twitter.Modules.Streaming;
using BoxKite.Twitter.Console.Helpers;


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

            var twittercredentials = ManageTwitterCredentials.MakeConnection();

            if (twittercredentials.Valid)
            {
                System.Console.CancelKeyPress += new ConsoleCancelEventHandler(cancelStreamHandler);
                var session = new UserSession(twittercredentials, new DesktopPlatformAdaptor() );
                var checkUser = session.GetVerifyCredentials().Result;
                if (!checkUser.twitterFaulted)
                {
                    ConsoleOutput.PrintMessage(twittercredentials.ScreenName + " is authorised to use BoxKite.Twitter.");

                    var accountSettings = session.GetAccountSettings().Result;
                    if (!accountSettings.twitterFaulted)
                    {
                        //var locations = new List<string> { "-34.081953", "150.700493", "-33.593316", "151.284828" };
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
                    }

                       /*userstream = session.GetUserStream();
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
                    var x = session.GetMentions(count:100).Result;

                    foreach (var tweet in x)
                    {
                        PrintTweet(tweet);
                    }
                        
                    
                     session.GetFavourites(count: 10)
                        .Subscribe(t => PrintTweet(t, ConsoleColor.White, ConsoleColor.Black));

                    var fileName = @"C:\Users\Nick\Pictures\My Online Avatars\666.jpg";
                    if (File.Exists(fileName))
                    {
                        var newImage = File.ReadAllBytes(fileName);

                        // var x = session.SendTweetWithImage("Testing Image Upload. You can Ignore", Path.GetFileName(fileName),newImage).Result;

                        var x = session.ChangeAccountProfileImage("che.png", newImage).Result;

                        if (x.twitterFaulted)
                        {
                            PrintMessage(String.Format("Twitter Error: {0} {1}",x.TwitterControlMessage.twitter_error_code,x.TwitterControlMessage.twitter_error_message), ConsoleColor.Red);
                        }
                        else
                        {
                            PrintMessage("All is well");
                        }
                    }
                    */
                }
                else
                {
                    ConsoleOutput.PrintMessage(String.Format("Credentials could not be verified: {0}", checkUser.twitterControlMessage.twitter_error_message), ConsoleColor.Red);
                }
            }
            else
            {
                ConsoleOutput.PrintMessage("Authenticator could not start. Do you have the correct Client/Consumer IDs and secrets?", ConsoleColor.Red);
            }
            System.Console.ReadLine();
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
