using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Console.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.LiveFireTests
{
    public class UserAccountLiveFireTests
    {
        private string testScreenName { get; set; }
        public User loggedInUserProfile { get; set; }

        public async Task<bool> DoUserAccountTest(IUserSession session, List<int> testSeq)
        {
            var successStatus = true;
            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    ConsoleOutput.PrintMessage("1.1 UsersExtensions\\GetAccountSettings", ConsoleColor.Gray);
                    var accountSettings = await session.GetAccountSettings();
                    testScreenName = accountSettings.ScreenName;
                    if (!accountSettings.twitterFaulted && !string.IsNullOrWhiteSpace(testScreenName))
                    {
                        ConsoleOutput.PrintMessage(String.Format("Screen Name: {0}", testScreenName));
                        ConsoleOutput.PrintMessage(String.Format("Time Zone: {0}", accountSettings.TimeZone.name));
                        ConsoleOutput.PrintMessage(String.Format("Trend Location: {0}",
                            accountSettings.TrendLocation.ToList()[0].name));
                    }
                    else
                        successStatus = false;
                }
                // 2
                if (testSeq.Contains(2))
                {
                    ConsoleOutput.PrintMessage("1.2 UsersExtensions\\GetVerifyCredentials", ConsoleColor.Gray);
                    loggedInUserProfile = await session.GetVerifyCredentials();
                    if (!loggedInUserProfile.twitterFaulted)
                    {
                        ConsoleOutput.PrintMessage("Credentials Verified OK.");
                        ConsoleOutput.PrintMessage(String.Format("User ID Verified OK: {0}", loggedInUserProfile.UserId));
                    }
                    else
                        successStatus = false;
                }
                // 3
                if (testSeq.Contains(3))
                {
                    ConsoleOutput.PrintMessage("1.3 UsersExtensions\\GetUserProfile", ConsoleColor.Gray);
                    var getProfile = await session.GetUserProfile(testScreenName);
                    if (!getProfile.twitterFaulted)
                    {
                        ConsoleOutput.PrintMessage(String.Format("Name: {0}", getProfile.Name));
                        ConsoleOutput.PrintMessage(String.Format("User ID: {0}", getProfile.UserId));
                        ConsoleOutput.PrintMessage(String.Format("Avatar URL: {0}", getProfile.Avatar));
                        ConsoleOutput.PrintMessage(String.Format("Followers: {0}", getProfile.Followers));
                        ConsoleOutput.PrintMessage(String.Format("Friends: {0}", getProfile.Friends));
                        ConsoleOutput.PrintMessage(String.Format("Description: {0}", getProfile.Description));
                    }
                    else
                        successStatus = false;
                }
                // 4
                if (testSeq.Contains(4))
                {
                    ConsoleOutput.PrintMessage("1.4 UsersExtensions\\ChangeAccountSettings", ConsoleColor.Gray);
                    var changeSettings = await session.ChangeAccountSettings(trend_location_woeid: "1");
                    if (!changeSettings.twitterFaulted)
                    {
                        ConsoleOutput.PrintMessage(String.Format("Trend Location: {0}",
                            changeSettings.TrendLocation.ToList()[0].name));
                    }
                    else
                        successStatus = false;
                }
                // 5
                if (testSeq.Contains(5))
                {
                    ConsoleOutput.PrintMessage("1.5 UsersExtensions\\GetBlockList - Cursored", ConsoleColor.Gray);
                    long nextcursor = -1;
                    var blockListCount = 0;

                    do
                    {
                        var blockList = await session.GetBlockList(cursor: nextcursor);
                        if (blockList.twitterFaulted) continue;
                        nextcursor = blockList.next_cursor;
                        ConsoleOutput.PrintMessage(String.Format("Previous cursor: {0} Next cursor: {1}",
                            blockList.previous_cursor, blockList.next_cursor));
                        foreach (var l in blockList.users)
                        {
                            blockListCount++;
                            ConsoleOutput.PrintMessage(String.Format("ScreenName: {0} User ID: {1} Description: {2}",
                                l.Name, l.UserId, l.Description));
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage(String.Format("Block List Count: {0}",
                        blockListCount));
                }
                // 6
                if (testSeq.Contains(6))
                {
                    ConsoleOutput.PrintMessage("1.6 UsersExtensions\\DeleteUserBlock", ConsoleColor.Gray);
                    var deleteUserBlock = await session.DeleteUserBlock(screen_name: "NickHodgeMSFT");
                    if (!deleteUserBlock.twitterFaulted)
                        ConsoleOutput.PrintMessage(String.Format("ScreenName: {0}", deleteUserBlock.ScreenName));
                    else
                        successStatus = false;
                }
                // 7
                if (testSeq.Contains(7))
                {
                    ConsoleOutput.PrintMessage("1.7 UsersExtensions\\GetUsersDetailsFull From Screennames ",
                        ConsoleColor.Gray);
                    var screennames = new List<string> {"shiftkey", "katyperry"};
                    var getUserDetailsFullFromScreenNames = await session.GetUsersDetailsFull(screen_names: screennames);
                    if (!getUserDetailsFullFromScreenNames.twitterFaulted)
                    {
                        ConsoleOutput.PrintMessage(String.Format("Returned: {0}",
                            getUserDetailsFullFromScreenNames.Count()));
                        foreach (var u in getUserDetailsFullFromScreenNames)
                        {
                            ConsoleOutput.PrintMessage(String.Format("User ID: {1} // ScreenName: {0} // Description: {2}",
                                u.Name, u.UserId, u.Description));
                        }
                    }
                    else
                        successStatus = false;
                }
                // 8
                if (testSeq.Contains(8))
                {
                    ConsoleOutput.PrintMessage("1.8 UsersExtensions\\GetUsersDetailsFull From IDs", ConsoleColor.Gray);
                    var usersids = new List<int> {21447363, 14671135};
                    var getUserDetailsFullFromIDs = await session.GetUsersDetailsFull(user_ids: usersids);
                    if (!getUserDetailsFullFromIDs.twitterFaulted)
                    {
                        ConsoleOutput.PrintMessage(String.Format("Returned: {0}", getUserDetailsFullFromIDs.Count()));
                        foreach (var u in getUserDetailsFullFromIDs)
                        {
                            ConsoleOutput.PrintMessage(String.Format(
                                "ScreenName: {0} User ID: {1} Description: {2}",
                                u.Name, u.UserId, u.Description));
                        }
                    }
                    else
                        successStatus = false;
                }
                // 9
                if (testSeq.Contains(9))
                {
                    ConsoleOutput.PrintMessage("1.9 UsersExtensions\\SearchForUsers", ConsoleColor.Gray);
                    var q = "troll";
                    var searchForUsers = await session.SearchForUsers(q, 200, 1);
                    if (!searchForUsers.twitterFaulted)
                    {
                        ConsoleOutput.PrintMessage(String.Format("Returned: {0}", searchForUsers.Count()));
                        foreach (var u in searchForUsers)
                        {
                            ConsoleOutput.PrintMessage(String.Format(
                                "ScreenName: {0} User ID: {1} Description: {2}",
                                u.Name, u.UserId, u.Description));
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
