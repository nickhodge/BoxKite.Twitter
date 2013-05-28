// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BoxKite.Twitter.Console.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Console
{
    public class TweetsLiveFireTests
    {
        public async Task<bool> DoTweetTest(IUserSession session, List<int> testSeq)
        {
            var successStatus = true;
            try
            {
                // 1
                long tweetid = 0;
                if (testSeq.Contains(1))
                {
                    ConsoleOutput.PrintMessage("4.1 Tweets\\SendTweet", ConsoleColor.Gray);
                    var tweets1 = await session.SendTweet("Live Fire Test only, please ignore");

                    if (tweets1.OK)
                    {
                        tweetid = tweets1.Id;
                        ConsoleOutput.PrintMessage(
                                String.Format("From: {0} // Message: {1}", tweets1.User.ScreenName, tweets1.Text ));
                    }
                    else
                        successStatus = false;
                }

                var tweets2 = new Tweet();

                // 2
                if (testSeq.Contains(2))
                {
                    ConsoleOutput.PrintMessage("4.2 Tweets\\GetTweet", ConsoleColor.Gray);
                    tweets2 = await session.GetTweet(336377569098207233);

                    if (tweets2.OK)
                    {
                        ConsoleOutput.PrintMessage(
                            String.Format("From: {0} // Message: {1}", tweets2.User.ScreenName, tweets2.Text));
                    }
                    else
                        successStatus = false;
                }

                // 3
                if (testSeq.Contains(3))
                {
                    ConsoleOutput.PrintMessage("4.3 Tweets\\GetRetweets", ConsoleColor.Gray);
                    var tweets3 = await session.GetRetweets(tweets2);

                    if (tweets3.OK)
                    {
                        foreach (var t in tweets3)
                        {
                            ConsoleOutput.PrintMessage(
                                String.Format("From: {0} // Message: {1}", t.User.ScreenName, t.Text));
                        }
                    }
                    else
                        successStatus = false;
                }

                // 4
                if (testSeq.Contains(4))
                {
                    ConsoleOutput.PrintMessage("4.4 Tweets\\SendTweetWithImage", ConsoleColor.Gray);

                    var sr = FilesHelper.FromFile("sampleimage\\Boxkite-Logo-github.jpg");

                    using (var fs = new FileStream(sr, FileMode.Open, FileAccess.Read))
                    {
                        var tweets4 =
                            await
                                session.SendTweetWithImage("Live Fire Test only, please ignore", Path.GetFileName(sr), fs);

                        if (tweets4.OK)
                        {
                            tweetid = tweets4.Id;
                            ConsoleOutput.PrintMessage(
                                String.Format("From: {0} // Message: {1}", tweets4.User.ScreenName, tweets4.Text));
                        }
                        else
                        {
                            TwitterLiveFireAppControl.PrintTwitterErrors(tweets4.twitterControlMessage);
                            successStatus = false;
                        }
                    }
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