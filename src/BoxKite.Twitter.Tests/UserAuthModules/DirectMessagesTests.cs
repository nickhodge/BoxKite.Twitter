// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class DirectMessagesTests
    {
        readonly TestableUserSession session = new TestableUserSession();

        [Fact]
        public async Task Get_DirectMessages_received()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\directmessages\\directmessageslist.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/direct_messages.json");

            var directmessages = await session.GetDirectMessages();

            directmessages.Should().NotBeNull();
            directmessages.Count().ShouldBeEquivalentTo(1);
            directmessages.ToList()[0].Id.ShouldBeEquivalentTo(240136858829479936);
            directmessages.ToList()[0].Recipient.Name.ShouldBeEquivalentTo("Mick Jagger");
            directmessages.ToList()[0].Recipient.Avatar.ShouldBeEquivalentTo("http://a0.twimg.com/profile_images/2550226257/y0ef5abcx5yrba8du0sk_normal.jpeg");
            directmessages.ToList()[0].Text.ShouldBeEquivalentTo("booyakasha");
        }

        [Fact]
        public async Task Get_DirectMessages_sent()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\directmessages\\directmessagessentlist.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/direct_messages/sent.json");

            var directmessages = await session.GetDirectMessagesSent();

            directmessages.Should().NotBeNull();
            directmessages.Count().ShouldBeEquivalentTo(1);
            directmessages.ToList()[0].Id.ShouldBeEquivalentTo(240247560269340673);
        }

        [Fact]
        public async Task Get_Single_DirectMessage()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\directmessages\\directmessagesingle.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/direct_messages/show.json");

            var directmessages = await session.GetDirectMessageSingle(240136858829479936);

            directmessages.Should().NotBeNull();
            directmessages.Id.ShouldBeEquivalentTo(240136858829479936);
            directmessages.Recipient.Name.ShouldBeEquivalentTo("Mick Jagger");
        }

        [Fact]
        public async Task Create_and_Send_DirectMessage()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\directmessages\\directmessagesend.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/direct_messages/new.json");

            var directmessage = await session.SendDirectMessage("hello, tworld. welcome to 1.1.", "theSeanCook");

            directmessage.Should().NotBeNull();
            directmessage.Text.Should().Be("hello, tworld. welcome to 1.1.");
            directmessage.Recipient.ScreenName.Should().Be("theSeanCook");
        }

        [Fact]
        public async Task Delete_Sent_DirectMessage()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\directmessages\\directmessagedelete.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/direct_messages/destroy.json");

            var directmessage = await session.DeleteDirectMessage(240251665733795841);

            directmessage.Status.Should().BeTrue();
        }
    }
}
