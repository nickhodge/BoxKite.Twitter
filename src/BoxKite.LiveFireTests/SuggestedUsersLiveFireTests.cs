using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Console.Helpers;

namespace BoxKite.LiveFireTests
{
    public class SuggestedUsersLiveFireTests
    {
        public async Task<bool> DoSuggestedUsersTest(IUserSession session, List<int> testSeq)
        {
            var successStatus = true;
            var suggestedSlug = "";

            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    //-33.884097,151.134796
                    ConsoleOutput.PrintMessage("9.1 SuggestedUsers\\GetSuggestedLists", ConsoleColor.Gray);
                    var suggs1 = await session.GetSuggestedLists();

                    if (!suggs1.ToList()[0].twitterFaulted)
                    {
                        foreach (var sgg in suggs1)
                        {
                            ConsoleOutput.PrintMessage(
                                     String.Format("Name: {0} // Slug: {1}", sgg.Name, sgg.Slug));
                            suggestedSlug = sgg.Slug;
                        }
                    }
                }
                else
                    successStatus = false;


                // 2
                if (testSeq.Contains(2))
                {
                    //-33.884097,151.134796
                    ConsoleOutput.PrintMessage("9.2 SuggestedUsers\\GetSuggestedUsers", ConsoleColor.Gray);
                    var suggs2 = await session.GetSuggestedUsers(slug:suggestedSlug);

                    if (!suggs2.twitterFaulted)
                    {
                        foreach (var sgu in suggs2.Users)
                        {
                            ConsoleOutput.PrintMessage(
                                     String.Format("UserID: {0} // ScreenName: {1}",sgu.UserId, sgu.ScreenName));
                        }
                    }
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