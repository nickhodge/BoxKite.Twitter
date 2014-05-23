// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests.Modules
{
    [TestClass]
    public class ListExtensionsTests
    {
        private readonly TestableUserSession session = new TestableUserSession();

        [TestMethod]
        public async Task Get_Lists()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\getuserlists.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/lists/list.json");

            var lists = await session.GetLists(353445, "Test Screen Name");

            Assert.IsNotNull(lists);
            lists.Count().ShouldBeEquivalentTo(2);
            lists.ToList()[1].Id.ShouldBeEquivalentTo(2031945);
            lists.ToList()[0].FullName.ShouldBeEquivalentTo("@twitterapi/meetup-20100301");
            lists.ToList()[1].User.Name.ShouldBeEquivalentTo("Twitter API");
            lists.ToList()[0].MemberCount.ShouldBeEquivalentTo(116);
        }

        [TestMethod]
        public async Task Get_List_Timeline()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\listtimeline.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/lists/statuses.json");

            var listtimeline = await session.GetListTimeline(23,"slughere");

            Assert.IsNotNull(listtimeline);
            listtimeline.Count().ShouldBeEquivalentTo(1);
            listtimeline.ToList()[0].Id.ShouldBeEquivalentTo(245160944223793152);
            listtimeline.ToList()[0].User.Name.ShouldBeEquivalentTo("Toronto FC");
            listtimeline.ToList()[0].Entities.Hashtags.Count().ShouldBeEquivalentTo(2);
            listtimeline.ToList()[0].Entities.Hashtags.ToList()[1].Text.ShouldBeEquivalentTo("MLS");
            listtimeline.ToList()[0].Entities.Urls.Count().ShouldBeEquivalentTo(1);
            listtimeline.ToList()[0].Entities.Urls.ToList()[0]._Url.ShouldBeEquivalentTo("http://t.co/W2tON3OK");
            listtimeline.ToList()[0].Entities.Mentions.Count().ShouldBeEquivalentTo(1);
            listtimeline.ToList()[0].Entities.Mentions.ToList()[0].ScreenName.ShouldBeEquivalentTo("TeamUpFdn");
        }

        [TestMethod]
        public async Task Delete_User_From_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\listtimeline.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/members/destroy"); // WHERE IS JSON? Documentation is weird, man

            var removal = await session.DeleteUserFromList();

            Assert.IsTrue(removal.Status);
        }

        [TestMethod]
        public async Task Delete_From_Users_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\listtimeline.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/subscribers/destroy.json");

            var removal = await session.DeleteFromUsersList(23, "slughere");

            Assert.IsTrue(removal.Status);
        }

        [TestMethod]
        public async Task Create_User_Entry_In_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\listtimeline.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/members/create_all.json");

            var adds = await session.AddUsersToList(23, "slug here", new List<string> { "fred" }, new List<long> { 66, 4503597479886593});

            Assert.IsTrue(adds.Status);
        }


        [TestMethod]
        public async Task Delete_Users_From_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\listtimeline.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/members/destroy_all.json");

            var adds = await session.DeleteUsersFromList(23, "slug here", new List<string> { "fred" }, new List<long> { 66, 4503597479886593 });

            Assert.IsTrue(adds.Status);
        }

        [TestMethod]
        public async Task Create_A_User_In_My_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\listtimeline.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/members/create.json");

            var adds = await session.AddUserToMyList(23);

            Assert.IsTrue(adds.Status);
        }

        [TestMethod]
        public async Task Change_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\listtimeline.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/update.json");

            var changes = await session.ChangeList(23, "slug here", "fred", "public", "description");

            Assert.IsTrue(changes.Status);
        }

        [TestMethod]
        public async Task Get_User_Membership()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\userinlistscursored.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/lists/memberships.json");

            var removal = await session.GetListMembershipForUser(23);

            Assert.IsNotNull(removal);
            removal.next_cursor.ShouldBeEquivalentTo(1373407125131783107);
            removal.lists.Count().ShouldBeEquivalentTo(2);
            removal.lists.ToList()[1].Id.ShouldBeEquivalentTo(49270287);
            removal.lists.ToList()[0].MemberCount.ShouldBeEquivalentTo(46);
            removal.lists.ToList()[1].Name.ShouldBeEquivalentTo("vanessa williams");
            removal.lists.ToList()[0].User.Name.ShouldBeEquivalentTo("Chris Greco");
            removal.lists.ToList()[0].User.Avatar.ShouldBeEquivalentTo("http://a1.twimg.com/profile_images/1331628329/chris_2_normal.jpg");
        }


        [TestMethod]
        public async Task Get_List_Subscribers()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\listsubscribers.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/lists/subscribers.json");

            var subs = await session.GetListSubscribers(23, "slug goes here");

            Assert.IsNotNull(subs);
            subs.next_cursor.ShouldBeEquivalentTo(0);
            subs.users.Count().ShouldBeEquivalentTo(1);
            subs.users.ToList()[0].Name.ShouldBeEquivalentTo("NickHodge");
        }


        [TestMethod]
        public async Task Create_A_Subscribe_To_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\subscribetolistresponse.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/subscribers/create.json");

            var subs = await session.SubscribeToUsersList(23,"slug here");

            Assert.IsNotNull(subs);
            subs.Id.ShouldBeEquivalentTo(574);
            subs.User.Name.ShouldBeEquivalentTo("Twitter");
        }

        [TestMethod]
        public async Task Create_New_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\addlist.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/create.json");

            var newadd = await session.CreateList("newlist", "public");

            Assert.IsNotNull(newadd);
            newadd.Id.ShouldBeEquivalentTo(58300198);
            newadd.Mode.ShouldBeEquivalentTo("public");
            newadd.Slug.ShouldBeEquivalentTo("goonies");
            newadd.Following.ShouldBeEquivalentTo(false);
            newadd.SubscriberCount.ShouldBeEquivalentTo(0);
        }

        [TestMethod]
        public async Task Get_Single_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\getsinglelist.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/show.json");

            var singlelist = await session.GetList(23,"newlist");

            Assert.IsNotNull(singlelist);
            singlelist.FullName.ShouldBeEquivalentTo("@twitter/team");
            singlelist.Following.ShouldBeEquivalentTo(false);
            singlelist.Mode.ShouldBeEquivalentTo("public");
            singlelist.Uri.ShouldBeEquivalentTo("/twitter/team");
        }

        [TestMethod]
        public async Task Get_Subscriptions_Cursored()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\twitterlistcursored.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/subscriptions.json");

            var twlc = await session.GetMySubscriptions();

            Assert.IsNotNull(twlc);
            twlc.next_cursor.ShouldBeEquivalentTo(1364811190668631091);
            twlc.twitterlists.Count().ShouldBeEquivalentTo(5);
            twlc.twitterlists.ToList()[3].Id.ShouldBeEquivalentTo(49286494);
        }

        [TestMethod]
        public async Task Get_Lists_Owned()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\listownerships.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/lists/ownerships.json");

            var twlc = await session.GetListOwned();

            Assert.IsNotNull(twlc);
            twlc.next_cursor.ShouldBeEquivalentTo(46541288);
            twlc.twitterlists.Count().ShouldBeEquivalentTo(2);
            twlc.twitterlists.ToList()[1].Name.ShouldBeEquivalentTo("D9");
        }

        [TestMethod]
        public async Task Get_Is_Subscribed_To_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\issubscribedtolist.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/subscribers/show.json");

            var subs = await session.IsSubscribedToList(23, "slug here");

            Assert.IsNotNull(subs);
            subs.UserId.ShouldBeEquivalentTo(819797);
        }

        [TestMethod]
        public async Task Get_Is_User_On_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\isuseronlist.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/lists/members/show.json");

            var subs = await session.IsUserOnList(23, "slug here");

            Assert.IsNotNull(subs);
            subs.UserId.ShouldBeEquivalentTo(657693);
        }

        [TestMethod]
        public async Task Get_Members_On_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\membersoflistcursored.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/lists/members.json");

            var members = await session.GetMembersOnList(23, "slug here");

            Assert.IsNotNull(members);
            members.users.Count().ShouldBeEquivalentTo(1);
        }

        [TestMethod]
        public async Task Delete_List()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\lists\\deletelist.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/lists/destroy.json");

            var dellist = await session.DeleteList(23,"slughere");

            Assert.IsNotNull(dellist);
            dellist.Description.ShouldBeEquivalentTo("For life");
            dellist.User.Name.ShouldBeEquivalentTo("Arne Roomann-Kurrik");
        }


    }
}