using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Console.Helpers;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Models.Service;
using BoxKite.Twitter.Modules;

namespace BoxKite.LiveFireTests
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

                    var lists1 = await session.GetLists();

                    if (!lists1.twitterFaulted)
                    {
                        foreach (var lst in lists1)
                        {
                            ConsoleOutput.PrintMessage(
                                     String.Format("ID: {5} // Slug: {0} // Name: {1} // {2} // {3} // {4}", lst.Slug, lst.Name, lst.CreatedTime,lst.MemberCount,lst.Mode,lst.Id));
                            twList = lst;
                        }
                    }
                }
                else
                    successStatus = false;


                // 2
                if (testSeq.Contains(2))
                {
                    ConsoleOutput.PrintMessage("10.2 Lists\\GetListTimeline", ConsoleColor.Gray);

                    var lists2 = await session.GetListTimeline(list_id: twList.Id, slug:twList.Slug);

                    if (!lists2.twitterFaulted)
                    {
                        foreach (var tweet in lists2)
                        {
                            ConsoleOutput.PrintMessage(
                                    String.Format("From: {0} // Message: {1}", tweet.User.ScreenName, tweet.Text));
                        }
                    }
                }
                else
                    successStatus = false;

                // 3
                if (testSeq.Contains(3))
                {
                    ConsoleOutput.PrintMessage("10.3 Lists\\GetMyListsUserIsMemberOf - Cursored", ConsoleColor.Gray);

                    long nextcursor = -1;
                    var listCount = 0;

                    do
                    {
                        var lists3 =
                            await session.GetMyListsUserIsMemberOf(screen_name: "shiftkey", cursor: nextcursor);
                        if (lists3.twitterFaulted)
                        {
                            successStatus = false;
                            break;
                        }
                        nextcursor = lists3.next_cursor;
                        ConsoleOutput.PrintMessage(String.Format("Previous cursor: {0} Next cursor: {1}",
                            lists3.previous_cursor, lists3.next_cursor), ConsoleColor.Magenta);
                        foreach (var lst in lists3.lists)
                        {
                            listCount++;
                            ConsoleOutput.PrintMessage(
                                     String.Format("ID: {5} // Slug: {0} // Name: {1} // {2} // {3} // {4}", lst.Slug, lst.Name, lst.CreatedTime, lst.MemberCount, lst.Mode, lst.Id));
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage(String.Format("List Membership Count: {0}",
                        listCount));

                }
                else
                    successStatus = false;


                // 4
                if (testSeq.Contains(4))
                {
                    ConsoleOutput.PrintMessage("10.4 Lists\\GetMembersOnList - Cursored", ConsoleColor.Gray);

                    long nextcursor = -1;

                    do
                    {
                        var lists4 =
                            await session.GetMembersOnList(list_id: 52908745, slug: "autechheads", owner_id: 800364, owner_screen_name: "NickHodgeMSFT", cursor: nextcursor);
                        if (lists4.twitterFaulted)
                        {
                            successStatus = false;
                            break;
                        }
                        nextcursor = lists4.next_cursor;
                        ConsoleOutput.PrintMessage(String.Format("Previous cursor: {0} Next cursor: {1}",
                            lists4.previous_cursor, lists4.next_cursor), ConsoleColor.Magenta);
                        foreach (var lusr in lists4.users)
                        {
                            ConsoleOutput.PrintMessage(
                                         String.Format("UserID: {0} // ScreenName: {1}", lusr.UserId, lusr.ScreenName));
                        }
                    } while (nextcursor != 0);
                }
                else
                    successStatus = false;


                // 5
                if (testSeq.Contains(5))
                {
                    ConsoleOutput.PrintMessage("10.5 Lists\\IsUserOnList", ConsoleColor.Gray);
                    var testScreenName = "coatsy";    
                    var lists5 =
                        await session.IsUserOnList(screen_name: testScreenName, list_id: 52908745, owner_id: 800364);

                    if (!lists5.twitterFaulted)
                    {
                        if (lists5.ScreenName == null)
                            ConsoleOutput.PrintMessage(String.Format("UserID: {0} is NOT ON the list", testScreenName));
                        else
                            ConsoleOutput.PrintMessage(String.Format("ScreenName: {0} is on the list", lists5.ScreenName));
                    }
                    else
                        successStatus = false;
                }




                // 6
                if (testSeq.Contains(6))
                {
                    ConsoleOutput.PrintMessage("10.6 Lists\\AddUserToMyList", ConsoleColor.Gray);
                    var testScreenName = "shiftkey";
                    var lists6 =
                        await session.AddUserToMyList(list_id: 52908745, user_id_to_add: 14671135, screen_name_to_add:"shiftkey");

                    if (lists6.Status)
                    {
                        ConsoleOutput.PrintMessage(String.Format("ScreenName: {0} is added the list", testScreenName));
                    }
                }


                // 7
                if (testSeq.Contains(7))
                {
                    ConsoleOutput.PrintMessage("10.7 Lists\\DeleteUsersFromList", ConsoleColor.Gray);
                    var testScreenName = "coatsy";
                    var testSlug = "autechheads";
                    var testOwner = "NickHodgeMSFT";
                    var testScreenNames = new List<string> {testScreenName};


                    var lists7 =
                        await session.DeleteUsersFromList(slug: testSlug,owner_screen_name: testOwner, screen_names:testScreenNames);

                    if (lists7.Status)
                    {
                        ConsoleOutput.PrintMessage(String.Format("ScreenName: {0} is added the list", testScreenName));
                    }
                    else
                    {
                        TwitterLiveFireAppControl.PrintTwitterErrors(lists7.TwitterControlMessage);
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