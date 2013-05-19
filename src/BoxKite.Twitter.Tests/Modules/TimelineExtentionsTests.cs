using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests.Modules
{
    [TestClass]
    public class TimelineExtensionsTests
    {
        readonly TestableSession session = new TestableSession();

        [TestMethod]
        public async Task Get_Mentions_Timeline()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\timeline\\mentionstimeline.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/mentions_timeline.json");

            var mentions = await session.GetMentions();

            Assert.IsNotNull(mentions);
            mentions.ToList()[0].Entities.Mentions.Should().HaveCount(3);
            mentions.ToList()[0].Entities.Mentions.ToList()[1].Name.Should().BeEquivalentTo("Matt Harris");
            Assert.IsTrue(mentions.ToList().Count > 0);
            Assert.IsTrue(mentions.ToList().Count == 2);
        }

        [TestMethod]
        public async Task Get_User_Timeline()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\timeline\\usertimeline.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/user_timeline.json");

            var usertimeline = await session.GetUserTimeline();

            Assert.IsNotNull(usertimeline);
            usertimeline.ToList()[1].Entities.Urls.Should().HaveCount(1);
            usertimeline.ToList()[1].Entities.Urls.ToList()[0].ExpandedUrl.ShouldBeEquivalentTo("https://dev.twitter.com/issues/485");
            Assert.IsTrue(usertimeline.ToList().Count > 0);
            Assert.IsTrue(usertimeline.ToList().Count == 2);
        }

        [TestMethod]
        public async Task Get_User_Homeline()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\timeline\\hometimeline.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/home_timeline.json");

            var hometimeline = await session.GetHomeTimeline();

            Assert.IsNotNull(hometimeline);
            Assert.IsTrue(hometimeline.ToList().Count > 0);
            Assert.IsTrue(hometimeline.ToList().Count == 3);
        }

        [TestMethod]
        public async Task Get_Retweets_Of_Me()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\timeline\\retweetsofme.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/retweets_of_me.json");

            var retweetsofme = await session.GetRetweetsOfMe();

            Assert.IsNotNull(retweetsofme);
            Assert.IsTrue(retweetsofme.ToList().Count > 0);
            Assert.IsTrue(retweetsofme.ToList().Count == 1);
            retweetsofme.ToList()[0].Text.Should().BeEquivalentTo("It's bring your migraine to work day today!");

        }


    }
}
