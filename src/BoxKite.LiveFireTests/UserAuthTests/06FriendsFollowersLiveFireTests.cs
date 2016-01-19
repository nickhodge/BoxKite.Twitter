// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoxKite.Twitter;

namespace BoxKite.Twitter.Console
{
    public class FriendsFollowersLiveFireTests
    {
        public async Task<bool> DoFriendsFollowersTest(IUserSession session, List<int> testSeq)
        {
            var successStatus = true;
            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    ConsoleOutput.PrintMessage("6.1 FriendsFollowers\\GetFriendsIDs", ConsoleColor.Gray);

                    long nextcursor = -1;
                    var ffListCount = 0;

                    do
                    {
                        var ff1List = await session.GetFriendsIDs(cursor: nextcursor);
                        if (ff1List.twitterFaulted)
                        {
                            successStatus = false;
                            break;
                        };
                        nextcursor = ff1List.next_cursor;
                        ConsoleOutput.PrintMessage(
                            $"Previous cursor: {ff1List.previous_cursor} Next cursor: {ff1List.next_cursor}");
                        foreach (var l in ff1List.UserIDs)
                        {
                            ffListCount++;
                            ConsoleOutput.PrintMessage($"User ID: {l}");
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage($"Total Friends List Count: {ffListCount}");
                }

                // 2
                if (testSeq.Contains(2))
                {
                    ConsoleOutput.PrintMessage("6.2 FriendsFollowers\\GetFollowersIDs", ConsoleColor.Gray);

                    long nextcursor = -1;
                    var ff2ListCount = 0;

                    do
                    {
                        var ff2List = await session.GetFollowersIDs(cursor: nextcursor);
                        if (ff2List.twitterFaulted)
                        {
                            successStatus = false;
                            break;
                        };
                        nextcursor = ff2List.next_cursor;
                        ConsoleOutput.PrintMessage(
                            $"Previous cursor: {ff2List.previous_cursor} Next cursor: {ff2List.next_cursor}");
                        foreach (var l in ff2List.UserIDs)
                        {
                            ff2ListCount++;
                            ConsoleOutput.PrintMessage($"User ID: {l}");
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage($"Total Followers List Count: {ff2ListCount}");
                }

                // 3
                if (testSeq.Contains(3))
                {
                    ConsoleOutput.PrintMessage("6.3 FriendsFollowers\\GetFriendships", ConsoleColor.Gray);
                    var ff3 = await session.GetFriendships(new List<string>{"katyperry","shiftkey"});

                    if (ff3.OK)
                    {
                        foreach (var t in ff3)
                        {
                            ConsoleOutput.PrintMessage(
                                $"Name: {t.ScreenName} // UserID: {t.UserId}");
                            foreach (var c in t.Connections)
                            {
                                ConsoleOutput.PrintMessage(
                                    $"Connection: {c.ToString()}");
                            }
                        }
                    }
                    else
                        successStatus = false;
                }


                // 4
                if (testSeq.Contains(4))
                {
                    ConsoleOutput.PrintMessage("6.4 FriendsFollowers\\GetFriendshipRequestsIncoming", ConsoleColor.Gray);

                    long nextcursor = -1;
                    var ff4ListCount = 0;

                    do
                    {
                        var ff4List = await session.GetFriendshipRequestsIncoming(cursor: nextcursor);
                        if (ff4List.twitterFaulted)
                        {
                            successStatus = false;
                            break;
                        };
                        nextcursor = ff4List.next_cursor;
                        ConsoleOutput.PrintMessage(
                            $"Previous cursor: {ff4List.previous_cursor} Next cursor: {ff4List.next_cursor}");
                        foreach (var l in ff4List.UserIDs)
                        {
                            ff4ListCount++;
                            ConsoleOutput.PrintMessage($"User ID: {l}");
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage($"Total Followers List Count: {ff4ListCount}");
                }

                // 5
                if (testSeq.Contains(5))
                {
                    ConsoleOutput.PrintMessage("6.5 FriendsFollowers\\GetFriendshipRequestsOutgoing", ConsoleColor.Gray);

                    long nextcursor = -1;
                    var ff5ListCount = 0;

                    do
                    {
                        var ff5List = await session.GetFriendshipRequestsOutgoing(cursor: nextcursor);
                        if (ff5List.twitterFaulted)
                        {
                            successStatus = false;
                            break;
                        };
                        nextcursor = ff5List.next_cursor;
                        ConsoleOutput.PrintMessage(
                            $"Previous cursor: {ff5List.previous_cursor} Next cursor: {ff5List.next_cursor}");
                        foreach (var l in ff5List.UserIDs)
                        {
                            ff5ListCount++;
                            ConsoleOutput.PrintMessage($"User ID: {l}");
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage($"Total Followers List Count: {ff5ListCount}");
                }

                // 6
                if (testSeq.Contains(6))
                {
                    ConsoleOutput.PrintMessage("6.6 FriendsFollowers\\CreateFriendship", ConsoleColor.Gray);
                    var ff6 = await session.CreateFriendship("katyperry");
                    if (ff6.OK)
                    {
                        ConsoleOutput.PrintMessage($"Name: {ff6.Name}");
                        ConsoleOutput.PrintMessage($"User ID: {ff6.UserId}");
                        ConsoleOutput.PrintMessage($"Avatar URL: {ff6.Avatar}");
                        ConsoleOutput.PrintMessage($"Followers: {ff6.Followers}");
                        ConsoleOutput.PrintMessage($"Friends: {ff6.Friends}");
                        ConsoleOutput.PrintMessage($"Description: {ff6.Description}");
                    }
                    else
                        successStatus = false;
                }

                // 7
                if (testSeq.Contains(7))
                {
                    ConsoleOutput.PrintMessage("6.7 FriendsFollowers\\DeleteFriendship", ConsoleColor.Gray);
                    var ff7 = await session.DeleteFriendship(user_id: 21447363);
                    if (ff7.OK)
                    {
                        ConsoleOutput.PrintMessage($"Name: {ff7.Name}");
                        ConsoleOutput.PrintMessage($"User ID: {ff7.UserId}");
                        ConsoleOutput.PrintMessage($"Avatar URL: {ff7.Avatar}");
                        ConsoleOutput.PrintMessage($"Followers: {ff7.Followers}");
                        ConsoleOutput.PrintMessage($"Friends: {ff7.Friends}");
                        ConsoleOutput.PrintMessage($"Description: {ff7.Description}");
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