// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests.Modules
{
    [TestClass]
    public class DirectMessagesTests
    {
        readonly TestableSession session = new TestableSession();

        [TestMethod]
        public async Task Get_DirectMessages_received()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\directmessages\\directmessageslist.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/direct_messages.json");

            var directmessages = await session.GetDirectMessages();

            Assert.IsNotNull(directmessages);
            directmessages.Count().ShouldBeEquivalentTo(1);
            directmessages.ToList()[0].Id.ShouldBeEquivalentTo(240136858829479936);
            directmessages.ToList()[0].Recipient.Name.ShouldBeEquivalentTo("Mick Jagger");
            directmessages.ToList()[0].Recipient.Avatar.ShouldBeEquivalentTo("http://a0.twimg.com/profile_background_images/644522235/cdjlccey99gy36j3em67.jpeg");
            directmessages.ToList()[0].Text.ShouldBeEquivalentTo("booyakasha");
        }

        [TestMethod]
        public async Task Get_DirectMessages_sent()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\directmessages\\directmessagessentlist.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/direct_messages/sent.json");

            var directmessages = await session.GetDirectMessagesSent();

            Assert.IsNotNull(directmessages);
            directmessages.Count().ShouldBeEquivalentTo(1);
            directmessages.ToList()[0].Id.ShouldBeEquivalentTo(240247560269340673);
        }

        [TestMethod]
        public async Task Get_Single_DirectMessage()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\directmessages\\directmessagesingle.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/direct_messages/show.json");

            var directmessages = await session.GetDirectMessageSingle(240136858829479936);

            Assert.IsNotNull(directmessages);
            directmessages.Id.ShouldBeEquivalentTo(240136858829479936);
            directmessages.Recipient.Name.ShouldBeEquivalentTo("Mick Jagger");
        }

        [TestMethod]
        public async Task Create_and_Send_DirectMessage()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\directmessages\\directmessagesend.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/direct_messages/new.json");

            var directmessage = await session.SendDirectMessage("hello, tworld. welcome to 1.1.", "theSeanCook");

            Assert.IsNotNull(directmessage);
            Assert.IsTrue(directmessage.Text == "hello, tworld. welcome to 1.1.");
            Assert.IsTrue(directmessage.Recipient.ScreenName == "theSeanCook");
        }

        [TestMethod]
        public async Task Delete_Sent_DirectMessage()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\directmessages\\directmessagedelete.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/direct_messages/destroy.json");

            var directmessage = await session.DeleteDirectMessage(240251665733795841);

            Assert.IsTrue(directmessage.Status);
        }
    }
}
