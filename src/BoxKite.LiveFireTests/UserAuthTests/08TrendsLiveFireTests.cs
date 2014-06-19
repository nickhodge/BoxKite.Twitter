// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter;

namespace BoxKite.Twitter.Console
{
    public class TrendsLiveFireTests
    {
        public async Task<bool> DoTrendsTest(IUserSession session, List<int> testSeq)
        {
            var successStatus = true;
            var trendToSearch = "";
            int woeidToSearch = 1;
            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    //-33.884097,151.134796
                    ConsoleOutput.PrintMessage("8.1 Trends\\GetTrendsByLocation", ConsoleColor.Gray);
                    var trends1 = await session.GetTrendsByLocation(latitude:-33.884097,longitude:151.134796);

                    if (trends1.OK)
                    {
                        foreach (var trnd in trends1)
                        {
                            ConsoleOutput.PrintMessage(
                                     String.Format("Name: {0} // Url: {1}", trnd.Name, trnd.Url));
                            woeidToSearch = trnd.WOEID;
                        }
                    }
                    else
                        successStatus = false;
                }
                

                // 2
                if (testSeq.Contains(2))
                {
                    ConsoleOutput.PrintMessage("8.2 Trends\\GetTrendsForPlace as specified in location", ConsoleColor.Gray);
                    var trends2 = await session.GetTrendsForPlace(placeId: woeidToSearch);

                    if (trends2.OK)
                    {
                        foreach (var trnd in trends2.ToList()[0].trends)
                        {
                            ConsoleOutput.PrintMessage(
                                     String.Format("Trend: {0} // Query: {1}", trnd.name, trnd.query));
                            trendToSearch = trnd.query;
                        }

                    }
                    else
                        successStatus = false;

                    ConsoleOutput.PrintMessage("8.2.1 Search\\SearchFor from Trend", ConsoleColor.Gray);
                    var trends21 = await session.SearchFor(trendToSearch, SearchResultType.Popular, count: 20 );

                    if (trends21.OK)
                    {
                        foreach (var tweet in trends21.Tweets)
                        {
                            ConsoleOutput.PrintMessage(
                                     String.Format("From: {0} // Message: {1}", tweet.User.ScreenName, tweet.Text));
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