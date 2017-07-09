// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class TweetExtensionsTests
    {
        readonly TestableUserSession session = new TestableUserSession();

        [Fact]
        public async Task Create_And_Send_Tweet()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweet.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/statuses/update.json");

            var singletweet = await session.SendTweet("Maybe he'll finally find his keys. #peterfalk");

            singletweet.Should().NotBeNull();
            singletweet.Id.Should().Be(243145735212777472);
        }

        [Fact]
        public async Task Create_And_Send_Tweet_with_location()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweetwithgeotag.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/statuses/update.json");

            var singletweet = await session.SendTweet("Maybe he'll finally find his keys. #peterfalk", 37.7821120598956, -122.400612831116);

            singletweet.Should().NotBeNull();
            singletweet.Entities.Hashtags.ToList()[0].Text.Should().Be("peterfalk");
            singletweet.Id.Should().Be(243145735212777472);
            singletweet.Location.Coordinates.Count().ShouldBeEquivalentTo(2);
        }

        [Fact]
        public async Task Delete_Tweet()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweet.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/statuses/destroy/243145735212777472.json");

            var deleted = await session.DeleteTweet("243145735212777472");

            deleted.Status.Should().BeTrue();
        }

        [Fact]
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

            tweetreply.Should().NotBeNull();
            tweetreply.Id.Should().Be(193579947615453184);
            tweetreply.InReplyToId.Should().Be(193577612956811264);
            tweetreply.InReplyToUserId.Should().Be(823083);
        }

        [Fact]
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

            tweetreply.Should().NotBeNull();
            tweetreply.Id.Should().Be(193579947615453184);
            tweetreply.InReplyToId.Should().Be(193577612956811264);
            tweetreply.InReplyToUserId.Should().Be(823083);
            tweetreply.Location.Coordinates.Count().ShouldBeEquivalentTo(2);
        }

        [Fact]
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

            retweet.Should().NotBeNull();
            retweet.Id.Should().Be(243149503589400576); // id of the retweet itself
            retweet.Text.ShouldBeEquivalentTo("RT @kurrik: tcptrace and imagemagick - two command line tools TOTALLY worth learning");
        }

        [Fact]
        public async Task Get_Tweet()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweet.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/show.json");

            var rtweet = await session.GetTweet(243145735212777472);

            rtweet.Should().NotBeNull();

            rtweet.Retweeted.Value.Should().BeFalse();
            rtweet.Id.Should().Be(243145735212777472); // id of the retweet itself
        }

        [Fact]
        public async Task Get_Retweets_Of_Tweet()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\retweetslist.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/retweets/243145735212777472.json");

            var retweetslist = await session.GetRetweets(new Tweet {Id = 243145735212777472});

            retweetslist.Should().NotBeNull();
            retweetslist.ToList().Count.Should().BeGreaterThan(0);
            retweetslist.ToList().Count.Should().Be(4);
        }

        [Fact]
        public async Task Create_And_Send_Tweet_With_Image()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\singletweetreply.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/statuses/update_with_media.json");

            var singletweet = await session.SendTweetWithImage("Maybe he'll finally find his keys. #peterfalk","test.jpg", new Byte[]{});

            singletweet.Should().NotBeNull();
            singletweet.Id.Should().Be(193579947615453184);
        }

        [Fact]
        public async Task Get_Tweet_with_Extended_Entities()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\tweets\\560049149836808192.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/show.json");

            var eetweet = await session.GetTweet(560049149836808192);

            eetweet.Should().NotBeNull();
            eetweet.ExtendedEntities.Should().NotBeNull();

            var eetweetmedialist = eetweet.ExtendedEntities.Media.ToList();

            eetweetmedialist.Count().Should().Be(1);
            eetweetmedialist[0].Id.Should().Be(560049056895209473);
            eetweetmedialist[0].ExpandedUrl.Should().Be("http://twitter.com/ActuallyNPH/status/560049149836808192/video/1");


            eetweet.Id.Should().Be(560049149836808192); // id of the tweet itself
        }
    }
}
