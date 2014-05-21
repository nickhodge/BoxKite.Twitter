// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Authentication;
using BoxKite.Twitter.Models;
using Reactive.EventAggregator;

namespace BoxKite.Twitter.Console
{
    public class ApplicationOnlyAuthFireTests
    {
        public async Task<bool> DoCombosTest(IUserSession session, List<int> testSeq)
        {
            var successStatus = true;

            // first, let's make our own session with clientkey/secret
            //var twitterClientKeys = await ClientKeyManager.GetTwitterClientKeys();
            //var newsession = new UserSession(twitterClientKeys.Item1, twitterClientKeys.Item2, new DesktopPlatformAdaptor());
            var newsession = new UserSession("", "", new DesktopPlatformAdaptor());


            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    ConsoleOutput.PrintMessage("12.1 User Time Line//Application Auth Only", ConsoleColor.Gray);

                    var combo1 = await newsession.GetUserTimeline("KatyPerry");

                    if (combo1.OK)
                    {
                        foreach (var trnd in combo1)
                        {
                            ConsoleOutput.PrintMessage(
                                String.Format("Tweet Test: {0}", trnd.Text));
                        }
                    }
                    else
                    {
                        successStatus = false;
                        throw new Exception("cannot user time line app only auth");
                    }
                } // end test 1

            }
            catch (Exception e)
            {
                ConsoleOutput.PrintError(e.ToString());
                return false;
            }
            return successStatus;
        }
    }
}