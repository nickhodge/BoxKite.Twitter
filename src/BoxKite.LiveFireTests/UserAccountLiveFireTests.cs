using System;
using System.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Console.Helpers;
using BoxKite.Twitter.Models.Service;

namespace BoxKite.LiveFireTests
{
    public class UserAccountLiveFireTests
    {
        public string testScreenName;
        public int testUserId;

        public async Task<bool> DoUserAccountTest(IUserSession session)
        {
            var successStatus = true;
            try
            {
                // 1
                ConsoleOutput.PrintMessage("UsersExtensions\\GetAccountSettings",ConsoleColor.Gray);
                var accountSettings = await session.GetAccountSettings();
                testScreenName = accountSettings.ScreenName;
                if (!accountSettings.twitterFaulted && !string.IsNullOrWhiteSpace(testScreenName))
                {
                    ConsoleOutput.PrintMessage(String.Format("Screen Name: {0}",testScreenName));
                    ConsoleOutput.PrintMessage(String.Format("Time Zone: {0}", accountSettings.TimeZone.name));
                    ConsoleOutput.PrintMessage(String.Format("Trend Location: {0}", accountSettings.TrendLocation.ToList()[0].name));
                }
                else
                {
                    successStatus = false;
                }

                // 2
                ConsoleOutput.PrintMessage("UsersExtensions\\GetVerifyCredentials", ConsoleColor.Gray);
                var verifyCredentials = await session.GetVerifyCredentials();
                testUserId = verifyCredentials.UserId;
                if (!verifyCredentials.twitterFaulted)
                {
                    ConsoleOutput.PrintMessage("Credentials Verified OK.");
                    ConsoleOutput.PrintMessage(String.Format("User ID Verified OK: {0}", testUserId));
                }
                else
                {
                    successStatus = false;
                }

                // 3
                ConsoleOutput.PrintMessage("UsersExtensions\\GetUserProfile", ConsoleColor.Gray);
                var getProfile = await session.GetUserProfile(testScreenName);
                if (!getProfile.twitterFaulted)
                {
                    ConsoleOutput.PrintMessage(String.Format("Name: {0}", getProfile.Name));
                    ConsoleOutput.PrintMessage(String.Format("User ID: {0}", getProfile.UserId));
                    ConsoleOutput.PrintMessage(String.Format("Avatar URL: {0}", getProfile.Avatar));
                    ConsoleOutput.PrintMessage(String.Format("Followers: {0}", getProfile.Followers));
                    ConsoleOutput.PrintMessage(String.Format("Friends: {0}", getProfile.Friends));
                }
                else
                {
                    successStatus = false;
                }

                // 4
                ConsoleOutput.PrintMessage("UsersExtensions\\ChangeAccountSettings", ConsoleColor.Gray);
                var changeSettings = await session.ChangeAccountSettings(trend_location_woeid:"1");
                if (!changeSettings.twitterFaulted)
                {
                    ConsoleOutput.PrintMessage(String.Format("Trend Location: {0}", changeSettings.TrendLocation.ToList()[0].name));
                }
                else
                {
                    successStatus = false;
                }

                // 5
                ConsoleOutput.PrintMessage("UsersExtensions\\GetBlockList", ConsoleColor.Gray);
                var blockList = await session.GetBlockList();
                if (!blockList.twitterFaulted)
                {
                    ConsoleOutput.PrintMessage(String.Format("Previous: {0} Next cursors: {1}", blockList.previous_cursor.ToString(), blockList.next_cursor.ToString()));
                    ConsoleOutput.PrintMessage(String.Format("Number in Block List: {0}", blockList.users.Count()));
                    foreach(var l in blockList.users)
                    {
                        ConsoleOutput.PrintMessage(String.Format("ScreenName: {0} User ID: {1};",l.Name,l.UserId));
                    };
                }

                // 6
                ConsoleOutput.PrintMessage("UsersExtensions\\DeleteUserBlock", ConsoleColor.Gray);
                var deleteUserBlock = await session.DeleteUserBlock(screen_name: "NickHodgeMSFT");
                if (!deleteUserBlock.twitterFaulted)
                {
                    ConsoleOutput.PrintMessage(String.Format("ScreenName: {0}",deleteUserBlock.ScreenName));
                }

                else
                {
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
