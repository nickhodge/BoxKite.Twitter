// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Models;
using Reactive.EventAggregator;

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

                    var searchstream = session.StartSearchStream(new EventAggregator(), track: trendToFollow);
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
                    var userids2 = new List<int>(); 

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
                            userids2.Add(l);
                            ff5ListCount++;
                            ConsoleOutput.PrintMessage(String.Format("User ID: {0}", l));
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage(String.Format("Total Outstanding Outgoing Friend Requests Count: {0}",
                       ff5ListCount));

                    if (userids2.Any())
                    {
                        var userlist2 = await session.GetUsersDetailsFull(user_ids: userids2);
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
                        }
                    }
                }

                // 3
                if (testSeq.Contains(3))
                {
                    ConsoleOutput.PrintMessage("11.3 Combo\\GetFriendshipRequestsIncoming, then hydrate User Details", ConsoleColor.Gray);

                    long nextcursor = -1;
                    var ff5ListCount = 0;
                    var userids3 = new List<int>();

                    do
                    {
                        var ff3List = await session.GetFriendshipRequestsIncoming(cursor: nextcursor);
                        if (ff3List.twitterFaulted)
                        {
                            successStatus = false;
                            break;
                        };
                        nextcursor = ff3List.next_cursor;
                        ConsoleOutput.PrintMessage(String.Format("Previous cursor: {0} Next cursor: {1}",
                            ff3List.previous_cursor, ff3List.next_cursor));
                        foreach (var l in ff3List.IDs)
                        {
                            userids3.Add(l);
                            ff5ListCount++;
                            ConsoleOutput.PrintMessage(String.Format("User ID: {0}", l));
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage(String.Format("Total Outstanding Incoming Friend Requests Count: {0}",
                       ff5ListCount));

                    if (userids3.Any())
                    {
                        var userlist3 = await session.GetUsersDetailsFull(user_ids: userids3);
                        if (userlist3.OK)
                        {
                            foreach (var requsers in userlist3)
                            {
                                ConsoleOutput.PrintMessage(
                                    String.Format("UserID: {0} // ScreenName: {1}", requsers.UserId, requsers.ScreenName));
                            }

                        }
                        else
                        {
                            successStatus = false;
                        }
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