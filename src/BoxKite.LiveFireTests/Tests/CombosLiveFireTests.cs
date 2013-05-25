// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BoxKite.Twitter.Console
{
    public class CombosFireTests
    {
        public async Task<bool> DoCombosTest(IUserSession session, List<int> testSeq)
        {
            var successStatus = true;
            var trendToFollow = "";
            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    //-33.884097,151.134796
                    ConsoleOutput.PrintMessage("11.1 Combo\\GetTrendsByLocation then Searchstream for 2 minutes", ConsoleColor.Gray);
                    var combo1 = await session.GetTrendsByLocation(latitude:-33.884097,longitude:151.134796);

                    if (!combo1.twitterFaulted)
                    {
                        trendToFollow = combo1[0].Name; // grab the first
                        foreach (var trnd in combo1)
                        {
                            ConsoleOutput.PrintMessage(
                                String.Format("Name: {0} // Url: {1}", trnd.Name, trnd.Url));
                        }
                    }
                    else
                    {
                        successStatus = false;
                        throw new Exception("cannot trends");
                    }

                    var searchstream = session.StartSearchStream(track: trendToFollow);
                    //searchstream.FoundTweets.Subscribe(t => ConsoleOutput.PrintTweet(t));
                    searchstream.Start();

                    while (searchstream.IsActive)
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(2));
                        searchstream.CancelSearchStream.Cancel();
                        searchstream.Stop();  
                    }
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