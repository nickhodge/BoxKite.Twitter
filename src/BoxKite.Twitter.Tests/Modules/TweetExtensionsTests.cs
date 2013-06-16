// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests
{
    [TestClass]
    public class TweetExtensionsTests
    {
        readonly TestableSession session = new TestableSession();

        [TestMethod]
        public async Task Create_And_Send_Tweet()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweet.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/statuses/update.json");

            var singletweet = await session.SendTweet("Maybe he'll finally find his keys. #peterfalk");

            Assert.IsNotNull(singletweet);
            Assert.IsTrue(singletweet.Id == 243145735212777472);
        }

        [TestMethod]
        public async Task Create_And_Send_Tweet_with_location()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweetwithgeotag.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/statuses/update.json");

            var singletweet = await session.SendTweet("Maybe he'll finally find his keys. #peterfalk", 37.7821120598956, -122.400612831116);

            Assert.IsNotNull(singletweet);
            Assert.IsTrue(singletweet.Entities.Hashtags.ToList()[0].Text == "peterfalk");
            Assert.IsTrue(singletweet.Id == 243145735212777472);
            singletweet.Location.coordinates.Count().ShouldBeEquivalentTo(2);
        }

        [TestMethod]
        public async Task Delete_Tweet()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweet.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/statuses/destroy/243145735212777472.json");

            var deleted = await session.DeleteTweet("243145735212777472");

            Assert.IsTrue(deleted.Status);
        }

        [TestMethod]
        public async Task Create_Reply_To_Tweet()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweetreply.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/statuses/update.json");
            var replytweet = new Tweet
                             {
                                 Id = 193577612956811264,
                                 User = new User {UserId = 823083},
                                 RawText = "Replying to this faked tweet"
                             };
            var tweetreply = await session.ReplyToTweet(replytweet, "@migueldeicaza my issue is if an assembly is compiled against System.Json.dll http://t.co/CByp6ds6 it wont work on the iphone");

            Assert.IsNotNull(tweetreply);
            Assert.IsTrue(tweetreply.Id == 193579947615453184);
            Assert.IsTrue(tweetreply.InReplyToId == 193577612956811264);
            Assert.IsTrue(tweetreply.InReplyToUserId == 823083);
        }

        [TestMethod]
        public async Task Create_Reply_To_Tweet_with_Location()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweetreply.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/statuses/update.json");
            var replytweet = new Tweet
            {
                Id = 193577612956811264,
                User = new User { UserId = 823083 },
                RawText = "Replying to this faked tweet"
            };
            var tweetreply = await session.ReplyToTweet(replytweet, "@migueldeicaza my issue is if an assembly is compiled against System.Json.dll http://t.co/CByp6ds6 it wont work on the iphone", 37.7821120598956, -122.400612831116);

            Assert.IsNotNull(tweetreply);
            Assert.IsTrue(tweetreply.Id == 193579947615453184);
            Assert.IsTrue(tweetreply.InReplyToId == 193577612956811264);
            Assert.IsTrue(tweetreply.InReplyToUserId == 823083);
            tweetreply.Location.coordinates.Count().ShouldBeEquivalentTo(2);
        }

        [TestMethod]
        public async Task Create_ReTweet()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\retweet.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/statuses/retweet/243149503589400576.json");
            var otweet = new Tweet
            {
                Id = 243149503589400576,
                RawText = "Replying to this faked tweet"
            };

            var retweet = await session.Retweet(otweet);

            Assert.IsNotNull(retweet);
            Assert.IsTrue(retweet.Id == 243149503589400576); // id of the retweet itself
            retweet.Text.ShouldBeEquivalentTo("RT @kurrik: tcptrace and imagemagick - two command line tools TOTALLY worth learning");
        }

        [TestMethod]
        public async Task Get_Tweet()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweet.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/show.json");

            var rtweet = await session.GetTweet(243145735212777472);

            Assert.IsNotNull(rtweet);
            Assert.IsTrue(rtweet.Id == 243145735212777472); // id of the retweet itself
        }

        [TestMethod]
        public async Task Get_Retweets_Of_Tweet()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\retweetslist.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/retweets/243145735212777472.json");

            var retweetslist = await session.GetRetweets(new Tweet {Id = 243145735212777472});

            Assert.IsNotNull(retweetslist);
            Assert.IsTrue(retweetslist.ToList().Count > 0);
            Assert.IsTrue(retweetslist.ToList().Count == 4);
         }

        [TestMethod]
        public async Task Create_And_Send_Tweet_With_Image()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweetreply.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/statuses/update_with_media.json");

            var singletweet = await session.SendTweetWithImage("Maybe he'll finally find his keys. #peterfalk","test.jpg", new Byte[]{});

            Assert.IsNotNull(singletweet);
            Assert.IsTrue(singletweet.Id == 193579947615453184);
        }
    }
}
