using System;
using System.IO;
using System.Threading;
using BoxKite.Modules;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Modules.Streaming;
using ConsoleApplication1.Helpers;

namespace BoxKite.Twitter.Console
{
    public class ConsoleApp
    {
        public static IUserStream userstream;

        private static void Main(string[] args)
        {
            PrintMessage("Welcome to BoxKite.Twitter Console");
            PrintMessage("(control-c ends)");

            var twittercredentials = ManageTwitterCredentials.MakeConnection();

            if (twittercredentials.Valid)
            {
                System.Console.CancelKeyPress += new ConsoleCancelEventHandler(cancelStreamHandler);
                var session = new UserSession(twittercredentials);
                var checkUser = session.GetVerifyCredentials().Result;
                if (!checkUser.twitterFaulted)
                {
                    PrintMessage(twittercredentials.ScreenName + " is authorised to use BoxKite.Twitter.");

                    var accountSettings = session.GetAccountSettings().Result;

                    userstream = session.GetUserStream();
                    userstream.Tweets.Subscribe(t => PrintTweet(t,ConsoleColor.Green));
                    userstream.Events.Subscribe(e => PrintEvent(e, ConsoleColor.Yellow));
                    userstream.DirectMessages.Subscribe(d => PrintDirect(d, ConsoleColor.Magenta, ConsoleColor.Black));
                    userstream.Start();

                    while (userstream.IsActive)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(0.5));
                    }
                    
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
                    PrintMessage(String.Format("Credentials could not be verified: {0}",checkUser.TwitterControlMessage.twitter_error_message), ConsoleColor.Red);
                }
            }
            else
            {
                PrintMessage("Authenticator could not start. Do you have the correct Client/Consumer IDs and secrets?", ConsoleColor.Red);
               
            }
            System.Console.ReadLine();
        }

        private static void cancelStreamHandler(object sender, ConsoleCancelEventArgs e)
        {
            if (userstream != null)
                userstream.Stop();
            PrintMessage("All finished.", ConsoleColor.Blue);
            Thread.Sleep(TimeSpan.FromSeconds(1.3));
        }

        private static void PrintError(String err, ConsoleColor fgColour=ConsoleColor.Red, ConsoleColor bgColour=ConsoleColor.Black)
        {
            PrintMessage(err, ConsoleColor.Red);
        }

        private static void PrintTweet(Tweet t, ConsoleColor fgColour=ConsoleColor.Gray, ConsoleColor bgColour=ConsoleColor.Black)
        {
            System.Console.BackgroundColor = bgColour;
            if (!t.twitterFaulted)
            {
                PrintLineHeader(t.User.ScreenName, ConsoleColor.Gray);
                System.Console.ForegroundColor = fgColour;
                System.Console.WriteLine("{0}", t.Text);
            }
            else
                PrintError(t.TwitterControlMessage.twitter_error_message, fgColour, bgColour);

            System.Console.ResetColor();
        }

        private static void PrintDirect(DirectMessage d, ConsoleColor fgColour=ConsoleColor.Magenta, ConsoleColor bgColour =ConsoleColor.Blue)
        {
            System.Console.BackgroundColor = bgColour;
            if (!d.twitterFaulted)
            {
                PrintLineHeader(d.SenderScreenName, ConsoleColor.Gray);
                System.Console.ForegroundColor = fgColour;
                System.Console.WriteLine("{0}", d.Text);
            }
            else
                PrintError(d.TwitterControlMessage.twitter_error_message, fgColour, bgColour);
            System.Console.ResetColor();
        }

        private static void PrintEvent (Event e, ConsoleColor fgColour, ConsoleColor bgColour=ConsoleColor.Black)
        {
            System.Console.BackgroundColor = bgColour;
            PrintLineHeader(e.EventName, ConsoleColor.Gray);
            System.Console.ForegroundColor = fgColour;
            var sourceTweet = ((TweetEvent)e).TargetObject;
            if (sourceTweet != null)
                PrintTweet(sourceTweet, fgColour, bgColour);
            System.Console.ResetColor();
        }

        private static void PrintLineHeader(string text, ConsoleColor fgColour=ConsoleColor.Green)
        {
            System.Console.ForegroundColor = fgColour;
            System.Console.Write("{0}: ",text);
            System.Console.ResetColor();
        }

        private static void PrintMessage(string text, ConsoleColor fgColour=ConsoleColor.Green)
        {
            System.Console.ForegroundColor = fgColour;
            System.Console.WriteLine("{0}", text);
            System.Console.ResetColor();
        }
    }
}
