using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Lifetime;
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

                    if (!lists1.ToList()[0].twitterFaulted)
                    {
                        foreach (var lst in lists1)
                        {
                            ConsoleOutput.PrintMessage(
                                     String.Format("Slug: {0} // Name: {1} // {2} // {3} // {4}", lst.Slug, lst.Name, lst.CreatedTime,lst.MemberCount,lst.Mode));
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

                    if (!lists2.ToList()[0].twitterFaulted)
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
                    ConsoleOutput.PrintMessage("10.3 Lists\\GetListMembershipForUser - Cursored", ConsoleColor.Gray);

                    long nextcursor = -1;
                    var listCount = 0;

                    do
                    {
                        var lists3 =
                            await session.GetListMembershipForUser(screen_name: "NickHodgeMSFT", cursor: nextcursor);
                        if (lists3.twitterFaulted)
                        {
                            successStatus = false;
                            break;
                        }
                        nextcursor = lists3.next_cursor;
                        ConsoleOutput.PrintMessage(String.Format("Previous cursor: {0} Next cursor: {1}",
                            lists3.previous_cursor, lists3.next_cursor));
                        foreach (var lst in lists3.lists)
                        {
                            listCount++;
                            ConsoleOutput.PrintMessage(
                                     String.Format("Slug: {0} // Name: {1} // {2} // {3} // {4}", lst.Slug, lst.Name, lst.CreatedTime, lst.MemberCount, lst.Mode));
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage(String.Format("List Membership Count: {0}",
                        listCount));

                }
                else
                    successStatus = false;

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