// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Console
{
    public class ListsLiveFireTests
    {
        public async Task<bool> DoListsTest(IUserSession session, List<int> testSeq)
        {
            var successStatus = true;
            var twList = new TwitterList();
            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    ConsoleOutput.PrintMessage("10.1 Lists\\GetLists", ConsoleColor.Gray);

                    var lists1 = await session.GetLists(screenName:"NickHodgeMSFT");

                    if (lists1.OK)
                    {
                        foreach (var lst in lists1)
                        {
                            ConsoleOutput.PrintMessage(
                                     String.Format("ID: {5} // Slug: {0} // Name: {1} // {2} // {3} // {4}", lst.Slug, lst.Name, lst.CreatedTime,lst.MemberCount,lst.Mode,lst.Id));
                            twList = lst;
                        }
                    }
                    else
                    {
                        TwitterLiveFireUserAuth.PrintTwitterErrors(lists1.twitterControlMessage);
                        successStatus = false;
                    }
                }
 
                // 2
                if (testSeq.Contains(2))
                {
                    ConsoleOutput.PrintMessage("10.2 Lists\\GetListTimeline", ConsoleColor.Gray);

                    var lists2 = await session.GetListTimeline(listId: twList.Id, slug:twList.Slug);

                    if (lists2.OK)
                    {
                        foreach (var tweet in lists2)
                        {
                            ConsoleOutput.PrintMessage(
                                $"From: {tweet.User.ScreenName} // Message: {tweet.Text}");
                        }
                    }
                    else
                    {
                        TwitterLiveFireUserAuth.PrintTwitterErrors(lists2.twitterControlMessage);
                        successStatus = false;
                    }
                }

                // 3
                if (testSeq.Contains(3))
                {
                    ConsoleOutput.PrintMessage("10.3 Lists\\GetMyListsUserIsMemberOf - Cursored", ConsoleColor.Gray);

                    long nextcursor = -1;
                    var listCount = 0;

                    do
                    {
                        var lists3 =
                            await session.GetMyListsUserIsMemberOf(screenName: "shiftkey", cursor: nextcursor);
                        if (lists3.twitterFaulted)
                        {                                           
                            TwitterLiveFireUserAuth.PrintTwitterErrors(lists3.twitterControlMessage);
                            successStatus = false;
                            break;
                        }
                        nextcursor = lists3.next_cursor;
                        ConsoleOutput.PrintMessage(
                            $"Previous cursor: {lists3.previous_cursor} Next cursor: {lists3.next_cursor}", ConsoleColor.Magenta);
                        foreach (var lst in lists3.lists)
                        {
                            listCount++;
                            ConsoleOutput.PrintMessage(
                                     String.Format("ID: {5} // Slug: {0} // Name: {1} // {2} // {3} // {4}", lst.Slug, lst.Name, lst.CreatedTime, lst.MemberCount, lst.Mode, lst.Id));
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage($"List Membership Count: {listCount}");
                }


                // 4
                if (testSeq.Contains(4))
                {
                    ConsoleOutput.PrintMessage("10.4 Lists\\GetMembersOnList - Cursored", ConsoleColor.Gray);

                    long nextcursor = -1;

                    do
                    {
                        var lists4 =
                            await session.GetMembersOnList(listId: 52908745, slug: "autechheads", ownerId: 800364, ownerScreenName: "NickHodgeMSFT", cursor: nextcursor);
                        if (lists4.twitterFaulted)
                        {
                            successStatus = false;
                            TwitterLiveFireUserAuth.PrintTwitterErrors(lists4.twitterControlMessage);
                            break;
                        }
                        nextcursor = lists4.next_cursor;
                        ConsoleOutput.PrintMessage(
                            $"Previous cursor: {lists4.previous_cursor} Next cursor: {lists4.next_cursor}", ConsoleColor.Magenta);
                        foreach (var lusr in lists4.users)
                        {
                            ConsoleOutput.PrintMessage(
                                $"UserID: {lusr.UserId} // ScreenName: {lusr.ScreenName}");
                        }
                    } while (nextcursor != 0);
                }


                // 5
                if (testSeq.Contains(5))
                {
                    ConsoleOutput.PrintMessage("10.5 Lists\\IsUserOnList", ConsoleColor.Gray);
                    var testScreenName = "coatsy";    
                    var lists5 =
                        await session.IsUserOnList(screenName: testScreenName, listId: 52908745, ownerId: 800364, ownerScreenName:"NickHodgeMSFT");

                    if (lists5.OK)
                    {
                        if (lists5.ScreenName == null)
                            ConsoleOutput.PrintMessage($"UserID: {testScreenName} is NOT ON the list");
                        else
                            ConsoleOutput.PrintMessage($"ScreenName: {lists5.ScreenName} is on the list");
                    }
                    else
                    {
                        TwitterLiveFireUserAuth.PrintTwitterErrors(lists5.twitterControlMessage);
                        successStatus = false;
                    }
                }

                // 6
                if (testSeq.Contains(6))
                {
                    ConsoleOutput.PrintMessage("10.6 Lists\\AddUserToMyList", ConsoleColor.Gray);
                    var testScreenName = "shiftkey";
                    var lists6 =
                        await session.AddUserToMyList(listId: 52908745, userIdToAdd: 14671135, screenNameToAdd:"shiftkey");

                    if (lists6.Status)
                    {
                        ConsoleOutput.PrintMessage($"ScreenName: {testScreenName} is added the list");
                    }
                }


                // 7
                if (testSeq.Contains(7))
                {
                    ConsoleOutput.PrintMessage("10.7 Lists\\DeleteUsersFromList", ConsoleColor.Gray);
                    var testScreenName = "coatsy";
                    var testScreenNames = new List<string> {testScreenName};

                    var lists7 =
                        await session.DeleteUsersFromList(listId: 52908745, ownerId: 800364, ownerScreenName:"NickHodgeMSFT", screenNames:testScreenNames);

                    if (lists7.Status)
                    {
                        ConsoleOutput.PrintMessage($"ScreenName: {testScreenName} is added the list");
                    }
                    else
                    {
                        TwitterLiveFireUserAuth.PrintTwitterErrors(lists7.twitterControlMessage);
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