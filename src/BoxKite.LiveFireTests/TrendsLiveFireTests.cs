using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Console.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.LiveFireTests
{
    public class TrendsLiveFireTests
    {
        public async Task<bool> DoTrendsTest(IUserSession session, List<int> testSeq)
        {
            var successStatus = true;
            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    ConsoleOutput.PrintMessage("8.1 Trends\\GetMentions", ConsoleColor.Gray);
                    var timeline1 = await session.GetMentions(count:100);

                    if (!timeline1.ToList()[0].twitterFaulted)
                    {
                        foreach (var tweet in timeline1)
                        {
                            ConsoleOutput.PrintMessage(
                                     String.Format("From: {0} // Message: {1}", tweet.User.ScreenName, tweet.Text));                            
                        }
                     }
                    else
                        successStatus = false;
                }

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