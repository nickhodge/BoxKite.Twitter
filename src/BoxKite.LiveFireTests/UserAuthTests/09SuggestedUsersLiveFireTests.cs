// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxKite.Twitter.Console
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

                    if (suggs1.OK)
                    {
                        foreach (var sgg in suggs1)
                        {
                            ConsoleOutput.PrintMessage(
                                $"Name: {sgg.Name} // Slug: {sgg.Slug}");
                            suggestedSlug = sgg.Slug;
                        }
                    }
                    else
                        successStatus = false;
                }
             

                // 2
                if (testSeq.Contains(2))
                {
                    //-33.884097,151.134796
                    ConsoleOutput.PrintMessage("9.2 SuggestedUsers\\GetSuggestedUsers", ConsoleColor.Gray);
                    var suggs2 = await session.GetSuggestedUsers(slug:suggestedSlug);

                    if (suggs2.OK)
                    {
                        foreach (var sgu in suggs2.Users)
                        {
                            ConsoleOutput.PrintMessage(
                                $"UserID: {sgu.UserId} // ScreenName: {sgu.ScreenName}");
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