// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class TimelineExtensionsTests
    {
        readonly TestableUserSession session = new TestableUserSession();

        [Fact]
        public async Task Get_Mentions_Timeline()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\timeline\\mentionstimeline.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/mentions_timeline.json");

            var mentions = await session.GetMentions();

            mentions.Should().NotBeNull();
            mentions.ToList()[0].Entities.Mentions.Should().HaveCount(3);
            mentions.ToList()[0].Entities.Mentions.ToList()[1].Name.Should().BeEquivalentTo("Matt Harris");
            mentions.ToList().Count.Should().BeGreaterThan(0);
            mentions.ToList().Count.Should().Be(2);
        }

        [Fact]
        public async Task Get_User_Timeline()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\timeline\\usertimeline.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/user_timeline.json");

            var usertimeline = await session.GetUserTimeline();

            usertimeline.Should().NotBeNull();
            usertimeline.ToList()[1].Entities.Urls.Should().HaveCount(1);
            usertimeline.ToList()[1].Entities.Urls.ToList()[0].ExpandedUrl.ShouldBeEquivalentTo("https://dev.twitter.com/issues/485");
            usertimeline.ToList().Count.Should().BeGreaterThan(0);
            usertimeline.ToList().Count.Should().Be(2);
        }

        [Fact]
        public async Task Get_User_Timeline_with_name()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\timeline\\usertimeline.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/user_timeline.json");

            var usertimeline = await session.GetUserTimeline(screenName:"NickHodgeMSFT");

            usertimeline.Should().NotBeNull();
            usertimeline.ToList()[1].Entities.Urls.Should().HaveCount(1);
            usertimeline.ToList()[1].Entities.Urls.ToList()[0].ExpandedUrl.ShouldBeEquivalentTo("https://dev.twitter.com/issues/485");
            usertimeline.ToList().Count.Should().BeGreaterThan(0);
            usertimeline.ToList().Count.Should().Be(2);
        }

        [Fact]
        public async Task Get_User_Homeline()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\timeline\\hometimeline.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/home_timeline.json");

            var hometimeline = await session.GetHomeTimeline();

            hometimeline.Should().NotBeNull();
            hometimeline.ToList().Count.Should().BeGreaterThan(0);
            hometimeline.ToList().Count.Should().Be(3);
        }

        [Fact]
        public async Task Get_Retweets_Of_Me()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\timeline\\retweetsofme.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/retweets_of_me.json");

            var retweetsofme = await session.GetRetweetsOfMe();

            retweetsofme.Should().NotBeNull();
            retweetsofme.ToList().Count.Should().BeGreaterThan(0);
            retweetsofme.ToList().Count.Should().Be(1);
            retweetsofme.ToList()[0].Text.Should().BeEquivalentTo("It's bring your migraine to work day today!");

        }


    }
}
