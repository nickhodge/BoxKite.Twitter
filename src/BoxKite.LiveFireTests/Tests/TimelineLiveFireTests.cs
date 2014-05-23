// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxKite.Twitter.Console
{
    public class TimelineLiveFireTests
    {
        public async Task<bool> DoTimelineTest(IUserSession session, List<int> testSeq)
        {
            var successStatus = true;
            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    ConsoleOutput.PrintMessage("7.1 Timeline\\GetMentions", ConsoleColor.Gray);
                    var timeline1 = await session.GetMentions(count: 100);

                    if (timeline1.OK)
                    {
                        foreach (var tweet in timeline1)
                        {
                            ConsoleOutput.PrintMessage(
                                String.Format("From: {0} // Message: {1}", tweet.User.ScreenName, tweet.Text));
                        }
                    }
                    else
                        successStatus = false;
                }

                // 2
                if (testSeq.Contains(2))
                {
                    ConsoleOutput.PrintMessage("7.2 Timeline\\GetUserTimeline", ConsoleColor.Gray);
                    var timeline2 = await session.GetUserTimeline(screen_name: "shiftkey", count: 20);

                    if (timeline2.OK)
                    {
                        foreach (var tweet in timeline2)
                        {
                            ConsoleOutput.PrintMessage(
                                String.Format("From: {0} // Message: {1}", tweet.User.ScreenName, tweet.Text));
                        }
                    }
                    else
                        successStatus = false;
                }


                // 3
                if (testSeq.Contains(3))
                {
                    ConsoleOutput.PrintMessage("7.3 Timeline\\GetHomeTimeline", ConsoleColor.Gray);
                    var timeline3 = await session.GetHomeTimeline(count: 30);

                    if (timeline3.OK)
                    {
                        foreach (var tweet in timeline3)
                        {
                            ConsoleOutput.PrintMessage(
                                String.Format("From: {0} // Message: {1}", tweet.User.ScreenName, tweet.Text));
                        }
                    }
                    else
                        successStatus = false;
                }


                // 4
                if (testSeq.Contains(4))
                {
                    ConsoleOutput.PrintMessage(
                        "7.4 Timeline\\GetHomeTimeline - Paging Forward, getting new tweets, with ID mechanism",
                        ConsoleColor.Gray);
                    var timeline4 = await session.GetHomeTimeline(count: 10);
                    long largestid = 0;
                    long smallestid = 0;

                    if (timeline4.OK)
                    {
                        smallestid = timeline4.ToList()[0].Id;
                        foreach (var tweet in timeline4)
                        {
                            ConsoleOutput.PrintMessage(
                                String.Format("ID: {0} // From: {1} // Message: {2}", tweet.Id, tweet.User.ScreenName,
                                    tweet.Text));
                            if (tweet.Id > largestid)
                                largestid = tweet.Id;
                            if (tweet.Id < smallestid)
                                smallestid = tweet.Id;
                        }
                        ConsoleOutput.PrintMessage(
                            String.Format(" LargestID: {0} // SmallestID: {1}", largestid, smallestid));

                        ConsoleOutput.PrintMessage("Waiting 20 seconds");
                        await Task.Delay(TimeSpan.FromSeconds(20));

                        ConsoleOutput.PrintMessage("Now Updating Home Timeline, should add newer messages");
                        var timeline41 = await session.GetHomeTimeline(since_id: largestid, count: 10);
                        if (timeline41.OK)
                        {
                            foreach (var tweet in timeline41)
                            {
                                ConsoleOutput.PrintMessage(
                                    String.Format("ID: {0} // From: {1} // Message: {2}", tweet.Id,
                                        tweet.User.ScreenName, tweet.Text));
                                if (tweet.Id > largestid)
                                    largestid = tweet.Id;
                                if (tweet.Id < smallestid)
                                    smallestid = tweet.Id;
                            }
                        }

                        ConsoleOutput.PrintMessage(
                            String.Format(" LargestID: {0} // SmallestID: {1}", largestid, smallestid));


                        ConsoleOutput.PrintMessage("Now Updating Home Timeline, should show older messages");
                        var timeline42 = await session.GetHomeTimeline(max_id: smallestid, count: 10);
                        if (timeline42.OK)
                        {
                            foreach (var tweet in timeline42)
                            {
                                ConsoleOutput.PrintMessage(
                                    String.Format("ID: {0} // From: {1} // Message: {2}", tweet.Id,
                                        tweet.User.ScreenName, tweet.Text));
                                if (tweet.Id > largestid)
                                    largestid = tweet.Id;
                                if (tweet.Id < smallestid)
                                    smallestid = tweet.Id;
                            }
                        }

                        else
                            successStatus = false;
                    }
                }
                // 5
                if (testSeq.Contains(5))
                {
                    ConsoleOutput.PrintMessage("7.5 Timeline\\GetRetweetsOfMe", ConsoleColor.Gray);
                    var timeline5 = await session.GetRetweetsOfMe(count: 30);

                    if (timeline5.OK)
                    {
                        foreach (var tweet in timeline5)
                        {
                            ConsoleOutput.PrintMessage(
                                String.Format("From: {0} // Message: {1}", tweet.User.ScreenName, tweet.Text));
                        }
                    }

                    else
                        successStatus = false;
                }


                // 6
                if (testSeq.Contains(6))
                {
                    ConsoleOutput.PrintMessage(
                        "7.6 Timeline\\GetHomeTimeline - Paging Backward, getting older tweets with ID mechanism",
                        ConsoleColor.Gray);

                    long smallestid = 0;
                    long largestid = 0;
                    int howManyToGet = 100;
                    int pagingSize = 10;

                    do
                    {
                        var timeline6 = await session.GetHomeTimeline(count: pagingSize, max_id: smallestid);
                        if (timeline6.OK)
                        {
                            smallestid = timeline6.ToList()[0].Id; // grab the first for comparator
                            foreach (var tweet in timeline6)
                            {
                                ConsoleOutput.PrintMessage(
                                    String.Format("ID: {0} // From: {1} // Message: {2}", tweet.Id,
                                        tweet.User.ScreenName,
                                        tweet.Text));
                                if (tweet.Id < smallestid) smallestid = tweet.Id;
                                if (tweet.Id > largestid) largestid = tweet.Id;
                                howManyToGet--;
                            }
                            ConsoleOutput.PrintMessage(
                                String.Format("SmallestID: {0}", smallestid),
                                ConsoleColor.Cyan);
                        }
                        else
                        {
                            successStatus = false;
                            TwitterLiveFireUserControl.PrintTwitterErrors(timeline6.twitterControlMessage);
                            break;
                        }
                    } while (howManyToGet > 0);
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