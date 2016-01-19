// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL
using System;
using System.Collections.Generic;
using System.Threading;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Console
{
    public class TwitterLiveFireUserAuth
    {
        public static IUserStream userstream;
        public static IUserSession usersession;

        public static void Main(string[] args)
        {
            ConsoleOutput.PrintMessage("BoxKite.Twitter Live Fire Tests (User Auth)");
            ConsoleOutput.PrintMessage("(control-c ends at anytime)");

            var twittercredentials = ManageTwitterCredentials.GetTwitterCredentialsFromFile();

            if (twittercredentials.Valid)
            {
                System.Console.CancelKeyPress += cancelStreamHandler;
                var session = new UserSession(twittercredentials, new DesktopPlatformAdaptor());
                var checkUser = session.GetVerifyCredentials().Result;
                if (checkUser.OK)
                {
                    ConsoleOutput.PrintMessage(twittercredentials.ScreenName + " is authorised to use BoxKite.Twitter.");

                    // put the test series number you wish to run into the init of the array
                    // then in each test group, put the numbers of the tests you would like to run
                    // NOTE: some tests require a previous test to work successfully
                    // NOTE: some tests post/delete items. This *is* a live fire test!

                    var testSeriesToRun = new List<int> {1};

                    // Calls tested by Test Series
                    // series 1 => 11 (UserAccounts & Auth)
                    // series 2 => 1 (API Management)
                    // series 3 => 5 (Direct Messages)
                    // series 4 => 5 (Tweets)
                    // series 5 => 3 (Favourites)
                    // series 6 => 8 (Friends/Followers)
                    // series 7 => 6 (TimeLine)
                    // series 8 => 3 (Trends)
                    // series 9 => 2 (SuggestedUsers)
                    // series 10=> 7 (Lists)
                    // series 11=> 3 (Combos)
                    // =============
                    // TOTAL      54

                    // Test Series 1
                    if (testSeriesToRun.Contains(1))
                    {
                        var ualft = new UserAccountLiveFireTests();
                        var testResult1 = ualft.DoUserAccountTest(session, new List<int> {10}).Result;

                        if (testResult1)
                        {
                            ConsoleOutput.PrintMessage($"1.0 User Account Tests Status: {testResult1}",
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage($"1.0 User Account Tests Status: {testResult1}",
                                ConsoleColor.Red);
                        }
                    }


                    // Test Series 2
                    if (testSeriesToRun.Contains(2))
                    {
                        var doit = new ApiManagementLiveFireTests();
                        var testResult2 = doit.DoApiTest(session, new List<int> {1 ,2}).Result;

                        if (testResult2)
                        {
                            ConsoleOutput.PrintMessage($"2.0 API Management Tests Status: {testResult2}",
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage($"2.0 API Management Tests Status: {testResult2}",
                                ConsoleColor.Red);
                        }
                    }

                    // Test Series 3
                    if (testSeriesToRun.Contains(3))
                    {
                        var dms = new DirectMessagesLiveFireTests();
                        var testResult3 = dms.DoDMTest(session, new List<int> {5}).Result;

                        if (testResult3)
                        {
                            ConsoleOutput.PrintMessage($"3.0 Direct Messages Tests Status: {testResult3}",
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage($"3.0 Direct Messages Tests Status: {testResult3}",
                                ConsoleColor.Red);
                        }
                    }


                    // Test Series 4
                    if (testSeriesToRun.Contains(4))
                    {
                        var tws = new TweetsLiveFireTests();
                        var testResult4 = tws.DoTweetTest(session, new List<int> {5}).Result;

                        if (testResult4)
                        {
                            ConsoleOutput.PrintMessage($"4.0 Tweet Tests Status: {testResult4}",
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage($"4.0 Tweet Tests Status: {testResult4}",
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
                            ConsoleOutput.PrintMessage($"5.0 Favourite Tests Status: {testResult5}",
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage($"5.0 Favourite Tests Status: {testResult5}",
                                ConsoleColor.Red);
                        }
                    }


                    // Test Series 6
                    if (testSeriesToRun.Contains(6))
                    {
                        var ffvs = new FriendsFollowersLiveFireTests();
                        var testResult6 = ffvs.DoFriendsFollowersTest(session, new List<int> { 4,5 }).Result;

                        if (testResult6)
                        {
                            ConsoleOutput.PrintMessage($"6.0 FriendsFollowers Tests Status: {testResult6}",
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage($"6.0 FriendsFollowers Tests Status: {testResult6}",
                                ConsoleColor.Red);
                        }
                    }

                    // Test Series 7
                    if (testSeriesToRun.Contains(7))
                    {
                        var tmln = new TimelineLiveFireTests();
                        var testResult7 = tmln.DoTimelineTest(session, new List<int> { 6 }).Result;

                        if (testResult7)
                        {
                            ConsoleOutput.PrintMessage($"7.0 Timeline Tests Status: {testResult7}",
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage($"7.0 Timeline Tests Status: {testResult7}",
                                ConsoleColor.Red);
                        }
                    }


                    // Test Series 8(also tests SearchFor in SearchExtensions)
                    if (testSeriesToRun.Contains(8))
                    {
                        var trln = new TrendsLiveFireTests();
                        var testResult8 = trln.DoTrendsTest(session, new List<int> { 4,5}).Result;

                        if (testResult8)
                        {
                            ConsoleOutput.PrintMessage($"8.0 Trends (and SearchFor) Tests Status: {testResult8}",
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage($"8.0 Trends (and SearchFor) Tests Status: {testResult8}",
                                ConsoleColor.Red);
                        }
                    }


                    // Test Series 9(also tests SearchFor in SearchExtensions)
                    if (testSeriesToRun.Contains(9))
                    {
                        var trln = new SuggestedUsersLiveFireTests();
                        var testResult9 = trln.DoSuggestedUsersTest(session, new List<int> { 1, 2 }).Result;

                        if (testResult9)
                        {
                            ConsoleOutput.PrintMessage($"9.0 SuggestedUsers Tests Status: {testResult9}",
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage($"9.0 SuggestedUsers Tests Status: {testResult9}",
                                ConsoleColor.Red);
                        }
                    }

                  
                    // Test Series 10 Lists
                    if (testSeriesToRun.Contains(10))
                    {
                        var lsts = new ListsLiveFireTests();
                        var testResult10 = lsts.DoListsTest(session, new List<int> { 1, 5, 7 }).Result;

                        if (testResult10)
                        {
                            ConsoleOutput.PrintMessage($"10.0 Lists Tests Status: {testResult10}",
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage($"10.0 Lists Tests Status: {testResult10}",
                                ConsoleColor.Red);
                        }
                    }


                    // Test Series 11 Lists
                    if (testSeriesToRun.Contains(11))
                    {
                        var cmbs = new CombosFireTests();
                        var testResult11 = cmbs.DoCombosTest(session, new List<int> { 2,3 }).Result;

                        if (testResult11)
                        {
                            ConsoleOutput.PrintMessage($"11.0 Combos Tests Status: {testResult11}",
                                ConsoleColor.White);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage($"11.0 Combos Tests Status: {testResult11}",
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
