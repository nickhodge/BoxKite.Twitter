// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Models;

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

                    if (combo1.OK)
                    {
                        trendToFollow = combo1[0].Name; // grab the first
                        foreach (var trnd in combo1)
                        {
                            ConsoleOutput.PrintMessage(
                                String.Format("Trend Test: {0}", trnd.Name));
                        }
                    }
                    else
                    {
                        successStatus = false;
                        throw new Exception("cannot trends");
                    }

                    var searchstream = session.StartSearchStream(track: trendToFollow);
                    searchstream.FoundTweets.Subscribe(t => ConsoleOutput.PrintTweet(t));
                    searchstream.Start();

                    while (searchstream.IsActive)
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(2));
                        searchstream.CancelSearchStream.Cancel();
                        searchstream.Stop();  
                    }
                }


                // 2
                if (testSeq.Contains(2))
                {
                    ConsoleOutput.PrintMessage("11.2 Combo\\GetFriendshipRequestsOutgoing, then hydrate User Details", ConsoleColor.Gray);

                    long nextcursor = -1;
                    var ff5ListCount = 0;
                    var userids = new List<int>(); 

                    do
                    {
                        var ff2List = await session.GetFriendshipRequestsOutgoing(cursor: nextcursor);
                        if (ff2List.twitterFaulted)
                        {
                            successStatus = false;
                            break;
                        };
                        nextcursor = ff2List.next_cursor;
                        ConsoleOutput.PrintMessage(String.Format("Previous cursor: {0} Next cursor: {1}",
                            ff2List.previous_cursor, ff2List.next_cursor));
                        foreach (var l in ff2List.IDs)
                        {
                            userids.Add(l);
                            ff5ListCount++;
                            ConsoleOutput.PrintMessage(String.Format("User ID: {0}", l));
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage(String.Format("Total Outstanding Friend Requests Count: {0}",
                       ff5ListCount));

                    var userlist2 = await session.GetUsersDetailsFull(user_ids: userids);
                    if (userlist2.OK)
                    {
                        foreach (var requsers in userlist2)
                        {
                            ConsoleOutput.PrintMessage(
                                     String.Format("UserID: {0} // ScreenName: {1}", requsers.UserId, requsers.ScreenName));
                        }
 
                    }
                    else
                    {
                        successStatus = false;
                    };
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