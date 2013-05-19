using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests.Modules
{
    [TestClass]
    public class FriendsFollowersExtensionsTests
    {
        private readonly TestableSession session = new TestableSession();

        [TestMethod]
        public async Task Get_Friends_IDs()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\friendsfollowersids.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friends/ids.json");

            var friends = await session.GetFriendsIDs();

            Assert.IsNotNull(friends);
            friends.next_cursor_str.ShouldBeEquivalentTo("0");
            friends.IDs.Should().HaveCount(31);
            friends.IDs.ToList()[6].ShouldBeEquivalentTo(14488353);
        }

        [TestMethod]
        public async Task Get_Followers_IDs()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\friendsfollowersids.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/followers/ids.json");

            var followers = await session.GetFollowersIDs();

            Assert.IsNotNull(followers);
            followers.next_cursor_str.ShouldBeEquivalentTo("0");
            followers.IDs.Should().HaveCount(31);
            followers.IDs.ToList()[6].ShouldBeEquivalentTo(14488353);
        }

        [TestMethod]
        public async Task Get_Friendship_Status_lookup()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\friendshiplookup.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friendships/lookup.json");

            var ffs = await session.GetFriendships(new string [2]{"episod","twitterapi"}, new string[0]);

            Assert.IsNotNull(ffs);
            ffs.Should().HaveCount(2);
            ffs.ToList()[1].Connections.ToList()[1].ShouldBeEquivalentTo("followed_by");
        }

        [TestMethod]
        public async Task Get_Incoming_Friend_Requests()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\friendsfollowersids.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friendships/incoming.json");

            var fsr = await session.GetFriendshipRequestsIncoming();

            Assert.IsNotNull(fsr);
            fsr.next_cursor_str.ShouldBeEquivalentTo("0");
            fsr.IDs.Should().HaveCount(31);
            fsr.IDs.ToList()[6].ShouldBeEquivalentTo(14488353);
        }

        [TestMethod]
        public async Task Get_Outgoing_Friend_Requests()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\friendsfollowersids.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friendships/outgoing.json");

            var fsr = await session.GetFriendshipRequestsOutgoing();

            Assert.IsNotNull(fsr);
            fsr.next_cursor_str.ShouldBeEquivalentTo("0");
            fsr.IDs.Should().HaveCount(31);
            fsr.IDs.ToList()[6].ShouldBeEquivalentTo(14488353);
        }

        [TestMethod]
        public async Task Create_Friendship()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\createfriendshipresponse.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friendships/create.json");

            var cfsr = await session.CreateFriendship();

            Assert.IsNotNull(cfsr);
            cfsr.Name.ShouldBeEquivalentTo("Doug Williams");
        }

        [TestMethod]
        public async Task Delete_Friendship()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\createfriendshipresponse.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/post/friendships/destroy.json");

            var cfsr = await session.DeleteFriendship();

            Assert.IsNotNull(cfsr);
            cfsr.Name.ShouldBeEquivalentTo("Doug Williams");
        }

        [TestMethod]
        public async Task Change_Friendship()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\userstatus.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/friendships/update.json");

            var updatefs = await session.ChangeFriendship();

            Assert.IsNotNull(updatefs);
            updatefs.Source.CanDM.Should().BeTrue();
            updatefs.Target.FollowedBy.Should().BeTrue();
        }

        [TestMethod]
        public async Task Get_Friendship()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\userstatus.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/friendships/show.json");

            var updatefs = await session.GetFriendship();

            Assert.IsNotNull(updatefs);
            updatefs.Source.CanDM.Should().BeTrue();
            updatefs.Target.FollowedBy.Should().BeTrue();
        }

        [TestMethod]
        public async Task Get_Friends_List_Detailed_Cursored()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\userlistcursored.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friends/list.json");

            var userlistcurs = await session.GetFriendsList();

            Assert.IsNotNull(userlistcurs);
            userlistcurs.next_cursor.ShouldBeEquivalentTo(1333504313713126852);
            userlistcurs.users.Should().HaveCount(18);
            userlistcurs.users.ToList()[14].Name.ShouldBeEquivalentTo("Support");
        }

        [TestMethod]
        public async Task Get_Followers_List_Detailed_Cursored()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\userlistcursored.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/followers/list.json");

            var userlistcurs = await session.GetFollowersList();

            Assert.IsNotNull(userlistcurs);
            userlistcurs.next_cursor.ShouldBeEquivalentTo(1333504313713126852);
            userlistcurs.users.Should().HaveCount(18);
            userlistcurs.users.ToList()[14].Name.ShouldBeEquivalentTo("Support");
        }
    }
}