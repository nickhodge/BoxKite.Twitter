// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Authentication;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Console
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
                    if (accountSettings.OK && !string.IsNullOrWhiteSpace(testScreenName))
                    {
                        ConsoleOutput.PrintMessage($"Screen Name: {testScreenName}");
                        ConsoleOutput.PrintMessage($"Time Zone: {accountSettings.TimeZone.name}");
                        ConsoleOutput.PrintMessage($"Trend Location: {accountSettings.TrendLocation.ToList()[0].name}");
                    }
                    else
                        successStatus = false;
                }
                // 2
                if (testSeq.Contains(2))
                {
                    ConsoleOutput.PrintMessage("1.2 UsersExtensions\\GetVerifyCredentials", ConsoleColor.Gray);
                    loggedInUserProfile = await session.GetVerifyCredentials();
                    if (loggedInUserProfile.OK)
                    {
                        ConsoleOutput.PrintMessage("Credentials Verified OK.");
                        ConsoleOutput.PrintMessage($"User ID Verified OK: {loggedInUserProfile.UserId}");
                    }
                    else
                        successStatus = false;
                }
                // 3
                if (testSeq.Contains(3))
                {
                    ConsoleOutput.PrintMessage("1.3 UsersExtensions\\GetUserProfile", ConsoleColor.Gray);
                    var getProfile = await session.GetUserProfile(testScreenName);
                    if (getProfile.OK)
                    {
                        ConsoleOutput.PrintMessage($"Name: {getProfile.Name}");
                        ConsoleOutput.PrintMessage($"User ID: {getProfile.UserId}");
                        ConsoleOutput.PrintMessage($"Avatar URL: {getProfile.Avatar}");
                        ConsoleOutput.PrintMessage($"Followers: {getProfile.Followers}");
                        ConsoleOutput.PrintMessage($"Friends: {getProfile.Friends}");
                        ConsoleOutput.PrintMessage($"Description: {getProfile.Description}");
                    }
                    else
                        successStatus = false;
                }
                // 4
                if (testSeq.Contains(4))
                {
                    ConsoleOutput.PrintMessage("1.4 UsersExtensions\\ChangeAccountSettings", ConsoleColor.Gray);
                    var changeSettings = await session.ChangeAccountSettings(trendLocationWoeid: "1");
                    if (changeSettings.OK)
                    {
                        ConsoleOutput.PrintMessage($"Trend Location: {changeSettings.TrendLocation.ToList()[0].name}");
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
                        ConsoleOutput.PrintMessage(
                            $"Previous cursor: {blockList.previous_cursor} Next cursor: {blockList.next_cursor}");
                        foreach (var l in blockList.users)
                        {
                            blockListCount++;
                            ConsoleOutput.PrintMessage(
                                $"ScreenName: {l.Name} User ID: {l.UserId} Description: {l.Description}");
                        }
                    } while (nextcursor != 0);

                    ConsoleOutput.PrintMessage($"Block List Count: {blockListCount}");
                }
                // 6
                if (testSeq.Contains(6))
                {
                    ConsoleOutput.PrintMessage("1.6 UsersExtensions\\DeleteUserBlock", ConsoleColor.Gray);
                    var deleteUserBlock = await session.DeleteUserBlock(screenName: "NickHodgeMSFT");
                    if (deleteUserBlock.OK)
                        ConsoleOutput.PrintMessage($"ScreenName: {deleteUserBlock.ScreenName}");
                    else
                        successStatus = false;
                }
                // 7
                if (testSeq.Contains(7))
                {
                    ConsoleOutput.PrintMessage("1.7 UsersExtensions\\GetUsersDetailsFull From Screennames ",
                        ConsoleColor.Gray);
                    var screennames = new List<string> {"shiftkey", "katyperry"};
                    var getUserDetailsFullFromScreenNames = await session.GetUsersDetailsFull(screenNames: screennames);
                    if (getUserDetailsFullFromScreenNames.OK)
                    {
                        ConsoleOutput.PrintMessage($"Returned: {getUserDetailsFullFromScreenNames.Count()}");
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
                    var usersids = new List<long> { 21447363, 14671135, 4503599627370241};
                    var getUserDetailsFullFromIDs = await session.GetUsersDetailsFull(userIds: usersids);
                    if (getUserDetailsFullFromIDs.OK)
                    {
                        ConsoleOutput.PrintMessage($"Returned: {getUserDetailsFullFromIDs.Count()}");
                        foreach (var u in getUserDetailsFullFromIDs)
                        {
                            ConsoleOutput.PrintMessage(
                                $"ScreenName: {u.Name} User ID: {u.UserId} Description: {u.Description}");
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
                    if (searchForUsers.OK)
                    {
                        ConsoleOutput.PrintMessage($"Returned: {searchForUsers.Count()}");
                        foreach (var u in searchForUsers)
                        {
                            ConsoleOutput.PrintMessage(
                                $"ScreenName: {u.Name} User ID: {u.UserId} Description: {u.Description}");
                        }
                    }
                    else
                        successStatus = false;
                }

                // 10
                if (testSeq.Contains(10))
                {
                    ConsoleOutput.PrintMessage("1.10 Get GetConfiguration", ConsoleColor.Gray);
                    var configUser  = await session.GetConfiguration();
                    if (configUser.OK)
                    {
                        ConsoleOutput.PrintMessage($"Returned for Max Media Per Upload: {configUser.MaxMediaPerUpload}");
                        ConsoleOutput.PrintMessage(
                            $"Returned for Max Chars for DM: {configUser.DirectMessageCharacterLimit}");
                    }
                    else
                        successStatus = false;
                }

                // 11
                if (testSeq.Contains(11))
                {
                    ConsoleOutput.PrintMessage("1.11 Start ApplicationOnlyAuth", ConsoleColor.Gray);
                    var getAuthToken = await session.StartApplicationOnlyAuth();
                    if (getAuthToken)
                    {
                        ConsoleOutput.PrintMessage($"Returned: {getAuthToken}");
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
