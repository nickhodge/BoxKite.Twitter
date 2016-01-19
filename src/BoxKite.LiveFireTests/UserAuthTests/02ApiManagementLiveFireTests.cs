// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxKite.Twitter.Console
{
    public class ApiManagementLiveFireTests
    {
        public async Task<bool> DoApiTest(ITwitterSession session, List<int> testSeq)
        {
            var successStatus = true;
            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    ConsoleOutput.PrintMessage("ApiManagement\\GetCurrentAPIStatus", ConsoleColor.Gray);
                    var currentapi = await session.GetCurrentApiStatus();
                    if (currentapi.OK)
                    {
                        foreach (var x in currentapi.APIRateStatuses)
                        {
                            ConsoleOutput.PrintMessage(
                                $"API on: {x.Key} Calls: {x.Value.Remaining}/{x.Value.Limit} Resets: {x.Value.ResetTime}");
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