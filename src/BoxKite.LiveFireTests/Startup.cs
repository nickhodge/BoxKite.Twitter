using System;
using System.Collections.Generic;
using System.Threading;
using BoxKite.Twitter;
using BoxKite.Twitter.Modules.Streaming;
using BoxKite.Twitter.Console.Helpers;

namespace BoxKite.LiveFireTests
{
    public class Startup
    {
        public static IUserStream userstream;
        public static IUserSession usersession;

        public static void Main(string[] args)
        {
            ConsoleOutput.PrintMessage("BoxKite.Twitter Live Fire Tests");
            ConsoleOutput.PrintMessage("(control-c ends at anytime)");

            var twittercredentials = ManageTwitterCredentials.MakeConnection();

            if (twittercredentials.Valid)
            {
                System.Console.CancelKeyPress += new ConsoleCancelEventHandler(cancelStreamHandler);
                var session = new UserSession(twittercredentials);
                var checkUser = session.GetVerifyCredentials().Result;
                if (!checkUser.twitterFaulted)
                {
                    ConsoleOutput.PrintMessage(twittercredentials.ScreenName + " is authorised to use BoxKite.Twitter.");

                    // put the test series number you wish to run into the init of the array
                    var testSeriesToRun = new List<int> {6};

                    // series 1 => 9
                    // series 2 => 1
                    // series 3 => 3
                    // series 4 => 3
                    // series 5 => 3
                    // series 6 => 8
                    // =============
                    // TOTAL      27

                    // Test Series 1
                    if (testSeriesToRun.Contains(1))
                    {
                        var ualft = new UserAccountLiveFireTests();
                        var testResult1 = ualft.DoUserAccountTest(session, new List<int> {1, 2, 3, 8, 9}).Result;

                        if (testResult1)
                        {
                            ConsoleOutput.PrintMessage(String.Format("1.0 User Account Tests Status: {0}", testResult1),
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage(String.Format("1.0 User Account Tests Status: {0}", testResult1),
                                ConsoleColor.Red);
                        }
                    }


                    // Test Series 2
                    if (testSeriesToRun.Contains(2))
                    {
                        var doit = new ApiManagementLiveFireTests();
                        var testResult2 = doit.DoApiTest(session, new List<int> {1}).Result;

                        if (testResult2)
                        {
                            ConsoleOutput.PrintMessage(String.Format("2.0 API Management Tests Status: {0}", testResult2),
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage(String.Format("2.0 API Management Tests Status: {0}", testResult2),
                                ConsoleColor.Red);
                        }
                    }

                    // Test Series 3
                    if (testSeriesToRun.Contains(3))
                    {
                        var dms = new DirectMessagesLiveFireTests();
                        var testResult3 = dms.DoDMTest(session, new List<int> {3}).Result;

                        if (testResult3)
                        {
                            ConsoleOutput.PrintMessage(String.Format("3.0 Direct Messages Tests Status: {0}", testResult3),
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage(String.Format("3.0 Direct Messages Tests Status: {0}", testResult3),
                                ConsoleColor.Red);
                        }
                    }


                    // Test Series 4
                    if (testSeriesToRun.Contains(4))
                    {
                        var tws = new TweetsLiveFireTests();
                        var testResult4 = tws.DoTweetTest(session, new List<int> {2,3}).Result;

                        if (testResult4)
                        {
                            ConsoleOutput.PrintMessage(String.Format("4.0 Tweet Tests Status: {0}", testResult4),
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage(String.Format("4.0 Tweet Tests Status: {0}", testResult4),
                                ConsoleColor.Red);
                        }
                    }

                    // Test Series 5
                    if (testSeriesToRun.Contains(5))
                    {
                        var fvs = new FavouritesLiveFireTests();
                        var testResult5 = fvs.DoFavouritesTest(session, new List<int> { 1,2, 3 }).Result;

                        if (testResult5)
                        {
                            ConsoleOutput.PrintMessage(String.Format("5.0 Favourite Tests Status: {0}", testResult5),
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage(String.Format("5.0 Favourite Tests Status: {0}", testResult5),
                                ConsoleColor.Red);
                        }
                    }


                    // Test Series 6
                    if (testSeriesToRun.Contains(6))
                    {
                        var ffvs = new FriendsFollowersLiveFireTests();
                        var testResult6 = ffvs.DoFriendsFollowersTest(session, new List<int> { 7 }).Result;

                        if (testResult6)
                        {
                            ConsoleOutput.PrintMessage(String.Format("6.0 FriendsFollowers Tests Status: {0}", testResult6),
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage(String.Format("6.0 FriendsFollowers Tests Status: {0}", testResult6),
                                ConsoleColor.Red);
                        }
                    }

                }
                else
                {
                    ConsoleOutput.PrintError("Unable to Authorise."); 
                }
             }
            ConsoleOutput.PrintMessage("Press Return to close window");
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
