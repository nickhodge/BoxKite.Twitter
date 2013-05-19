using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using BoxKite.Twitter;
using BoxKite.Twitter.Models.Service;
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
            ConsoleOutput.PrintMessage("(control-c ends)");

            var twittercredentials = ManageTwitterCredentials.MakeConnection();

            if (twittercredentials.Valid)
            {
                System.Console.CancelKeyPress += new ConsoleCancelEventHandler(cancelStreamHandler);
                var session = new UserSession(twittercredentials);
                var checkUser = session.GetVerifyCredentials().Result;
                if (!checkUser.twitterFaulted)
                {
                    ConsoleOutput.PrintMessage(twittercredentials.ScreenName + " is authorised to use BoxKite.Twitter.");

                    var ualft = new UserAccountLiveFireTests();
                    var testResult = ualft.DoUserAccountTest(session).Result;
                    ConsoleOutput.PrintMessage(String.Format("User Account Tests Status: {0}",testResult),ConsoleColor.Cyan);

                }
            }
            ConsoleOutput.PrintMessage("Press Return to continue");
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
