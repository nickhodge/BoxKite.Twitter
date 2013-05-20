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
                    var testSeriesToRun = new List<int> {3};


                    // Test Series 1
                    if (testSeriesToRun.Contains(1))
                    {
                        var ualft = new UserAccountLiveFireTests();
                        var testResult1 = ualft.DoUserAccountTest(session, new List<int> {1, 2, 3, 8, 9}).Result;

                        if (testResult1)
                        {
                            ConsoleOutput.PrintMessage(String.Format("User Account Tests Status: {0}", testResult1),
                                ConsoleColor.DarkBlue);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage(String.Format("User Account Tests Status: {0}", testResult1),
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
                            ConsoleOutput.PrintMessage(String.Format("User Account Tests Status: {0}", testResult2),
                                ConsoleColor.DarkBlue);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage(String.Format("User Account Tests Status: {0}", testResult2),
                                ConsoleColor.Red);
                        }
                    }



                    // Test Series 3
                    if (testSeriesToRun.Contains(3))
                    {
                        var doit = new ApiManagementLiveFireTests();
                        var testResult2 = doit.DoApiTest(session, new List<int> { 1 }).Result;

                        if (testResult2)
                        {
                            ConsoleOutput.PrintMessage(String.Format("User Account Tests Status: {0}", testResult2),
                                ConsoleColor.DarkBlue);
                        }
                        else
                        {
                            ConsoleOutput.PrintMessage(String.Format("User Account Tests Status: {0}", testResult2),
                                ConsoleColor.Red);
                        }
                    }

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
