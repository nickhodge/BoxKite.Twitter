// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxKite.Twitter.Console
{
    public class ApiManagementLiveFireTests
    {
        public async Task<bool> DoApiTest(IUserSession session, List<int> testSeq)
        {
            var successStatus = true;
            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    ConsoleOutput.PrintMessage("ApiManagement\\GetCurrentAPIStatus", ConsoleColor.Gray);
                    var currentapi = await session.GetCurrentAPIStatus();
                    if (!currentapi.twitterFaulted)
                    {
                        foreach (var x in currentapi.ApiRateStatuses)
                        {
                            ConsoleOutput.PrintMessage(
                                String.Format("API on: {0} Calls: {1}/{2} Resets: {3}", x.Key, x.Value.remaining, x.Value.limit, x.Value.ResetTime ));
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