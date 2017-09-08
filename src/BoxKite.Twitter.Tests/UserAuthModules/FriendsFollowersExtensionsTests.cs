﻿// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class FriendsFollowersExtensionsTests
    {
        private readonly TestableUserSession session = new TestableUserSession();

        [Fact]
        public async Task Get_Friends_IDs()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\friendsfollowersids.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friends/ids.json");

            var friends = await session.GetFriendsIDs("testing",324512345);

            friends.Should().NotBeNull();
            friends.next_cursor.ShouldBeEquivalentTo(0);
            friends.UserIDs.Should().HaveCount(31);
            friends.UserIDs.ToList()[6].ShouldBeEquivalentTo(14488353);
        }

        [Fact]
        public async Task Get_Followers_IDs()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\friendsfollowersids.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/followers/ids.json");

            var followers = await session.GetFollowersIDs("targetscreenname",3435);

            followers.Should().NotBeNull();
            followers.next_cursor.ShouldBeEquivalentTo(0);
            followers.UserIDs.Should().HaveCount(31);
            followers.UserIDs.ToList()[6].ShouldBeEquivalentTo(14488353);
        }

        [Fact]
        public async Task Get_Friendship_Status_lookup()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\friendshiplookup.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friendships/lookup.json");

            var ffs = await session.GetFriendships(new string [2]{"episod","twitterapi"});

            ffs.Should().NotBeNull();
            ffs.Should().HaveCount(2);
            ffs.ToList()[1].Connections.ToList()[1].ShouldBeEquivalentTo("followed_by");
        }

        [Fact]
        public async Task Get_Incoming_Friend_Requests()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\friendsfollowersids.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friendships/incoming.json");

            var fsr = await session.GetFriendshipRequestsIncoming();

            fsr.Should().NotBeNull();
            fsr.next_cursor.ShouldBeEquivalentTo(0);
            fsr.UserIDs.Should().HaveCount(31);
            fsr.UserIDs.ToList()[6].ShouldBeEquivalentTo(14488353);
        }

        [Fact]
        public async Task Get_Outgoing_Friend_Requests()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\friendsfollowersids.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friendships/outgoing.json");

            var fsr = await session.GetFriendshipRequestsOutgoing();

            fsr.Should().NotBeNull();
            fsr.next_cursor.ShouldBeEquivalentTo(0);
            fsr.UserIDs.Should().HaveCount(31);
            fsr.UserIDs.ToList()[6].ShouldBeEquivalentTo(14488353);
        }

        [Fact]
        public async Task Create_Friendship()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\createfriendshipresponse.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friendships/create.json");

            var cfsr = await session.CreateFriendship("testscreenname",345);

            cfsr.Should().NotBeNull();
            cfsr.Name.ShouldBeEquivalentTo("Doug Williams");
        }

        [Fact]
        public async Task Delete_Friendship()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\createfriendshipresponse.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/post/friendships/destroy.json");

            var cfsr = await session.DeleteFriendship("testscreenname", 345);

            cfsr.Should().NotBeNull();
            cfsr.Name.ShouldBeEquivalentTo("Doug Williams");
        }

        [Fact]
        public async Task Change_Friendship()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\userstatus.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/friendships/update.json");

            var updatefs = await session.ChangeFriendship("testscreenname", 345);

            updatefs.Should().NotBeNull();
            updatefs.Relationship.Source.CanDM.Should().BeTrue();
            updatefs.Relationship.Target.FollowedBy.Should().BeTrue();
        }

        [Fact]
        public async Task Get_Friendship()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\userstatus.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/friendships/show.json");

            var updatefs = await session.GetFriendship("sourcescreenname", "targetscreenname", 342, 6536);

            updatefs.Should().NotBeNull();
            updatefs.Relationship.Source.CanDM.Should().BeTrue();
            updatefs.Relationship.Target.FollowedBy.Should().BeTrue();
        }

        [Fact]
        public async Task Get_Friends_List_Detailed_Cursored()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\userlistcursored.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/friends/list.json");

            var userlistcurs = await session.GetFriendsList("retst",2345);

            userlistcurs.Should().NotBeNull();
            userlistcurs.next_cursor.ShouldBeEquivalentTo(1333504313713126852);
            userlistcurs.users.Should().HaveCount(18);
            userlistcurs.users.ToList()[14].Name.ShouldBeEquivalentTo("Support");
        }

        [Fact]
        public async Task Get_Followers_List_Detailed_Cursored()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\friendsfollowers\\userlistcursored.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/followers/list.json");

            var userlistcurs = await session.GetFollowersList("testing",343);

            userlistcurs.Should().NotBeNull();
            userlistcurs.next_cursor.ShouldBeEquivalentTo(1333504313713126852);
            userlistcurs.users.Should().HaveCount(18);
            userlistcurs.users.ToList()[14].Name.ShouldBeEquivalentTo("Support");
        }
    }
}