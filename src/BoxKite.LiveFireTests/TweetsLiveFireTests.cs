using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Console.Helpers;
using BoxKite.Twitter.Models;

namespace BoxKite.LiveFireTests
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

                    if (!tweets1.twitterFaulted)
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

                    if (!tweets2.twitterFaulted)
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

                    if (!tweets3.twitterFaulted)
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