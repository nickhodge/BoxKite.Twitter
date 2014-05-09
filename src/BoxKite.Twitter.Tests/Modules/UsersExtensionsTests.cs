// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests
{
    [TestClass]
    public class UsersExtensionsTests
    {
        readonly TestableSession session = new TestableSession();

        [TestMethod]
        public async Task Get_Account_Settings()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\users\\accountsettings.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/account/settings.json");

            var user = await session.GetAccountSettings();

            Assert.IsNotNull(user);
            user._Protected.Should().BeTrue();
            user.ScreenName.ShouldBeEquivalentTo("theSeanCook");
            user.TrendLocation.ToList()[0].name.ShouldBeEquivalentTo("Atlanta");
        }

        [TestMethod]
        public async Task Change_Account_Settings()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\users\\accountsettings.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/settings.json");

            var user = await session.ChangeAccountSettings();

            Assert.IsNotNull(user);
            user._Protected.Should().BeTrue();
            user.ScreenName.ShouldBeEquivalentTo("theSeanCook");
            user.TrendLocation.ToList()[0].name.ShouldBeEquivalentTo("Atlanta");
        }

        [TestMethod]
        public async Task Change_Account_Profile()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile.json");

            var user = await session.ChangeAccountProfile();

            Assert.IsNotNull(user);
            user.Description.ShouldBeEquivalentTo("Keep calm and rock on.");
        }

        [TestMethod]
        public async Task Change_Account_Profile_Colours()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_colors.json");

            var user = await session.ChangeAccountColours();

            Assert.IsNotNull(user);
            user.Description.ShouldBeEquivalentTo("Keep calm and rock on.");
        }


        [TestMethod]
        public async Task Get_Profile_WhenUserSent_ReturnsOneValue()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\users\\show.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            var user = await session.GetUserProfile("shiftkey");
            
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task Get_Profile_WhenUserSent_ReceivesNameAsParameter()
        {
            var screenName = "shiftkey";
            session.Returns(await Json.FromFile("data\\users\\show.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            // act
            await session.GetUserProfile(screenName);

            Assert.IsTrue(session.ReceivedParameter("screen_name", screenName));
            Assert.IsTrue(session.ReceivedParameter("include_entities", true.ToString()));
        }

        [TestMethod]
        public async Task Get_Profile_WhenIdSent_ReceivesNameAsParameter()
        {
            session.Returns(await Json.FromFile("data\\users\\show.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            // act
            var userdetails = await session.GetUserProfile(user_id:1234);

            Assert.IsNotNull(userdetails);
            userdetails.Name.ShouldBeEquivalentTo("Ryan Sarver");
        }

        [TestMethod]
        public async Task Get_Profile_WhenIdSent_ParsesResult()
        {
            session.Returns(await Json.FromFile("data\\users\\show.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            var user = await session.GetUserProfile(user_id: 1234);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task Change_Profile_Image()
        {
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_image.json");

            var user = await session.ChangeAccountProfileImage("test.jpg", new byte[]{});

            Assert.IsNotNull(user);
            user.UserId.ShouldBeEquivalentTo(776627022);
        }

        [TestMethod]
        public async Task Change_BackgroundImage()
        {
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_background_image.json");

            var user = await session.ChangeAccountBackgroundImage("test.jpg", new byte[]{});

            Assert.IsNotNull(user);
            user.UserId.ShouldBeEquivalentTo(776627022);
        }

        [TestMethod]
        public async Task Get_Banner()
        {
            session.Returns(await Json.FromFile("data\\users\\bannerprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_banner.json");

            var banner = await session.GetProfileBanners(screen_name:"boxkite");

            Assert.IsNotNull(banner);
            Assert.IsNotNull(banner.sizes);
            Assert.IsNotNull(banner.sizes.Standard_1500x500);
            Assert.IsNotNull(banner.sizes.Standard_1500x500.Url);
            banner.sizes.Standard_1500x500.Width.ShouldBeEquivalentTo(1500);
            banner.sizes.Standard_1500x500.Height.ShouldBeEquivalentTo(500);
        }

        [TestMethod]
        public async Task Change_Account_Colours()
        {
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_colors.json");

            var user = await session.ChangeAccountColours();

            Assert.IsNotNull(user);
            user.UserId.ShouldBeEquivalentTo(776627022);
        }

        [TestMethod]
        public async Task Get_BlockList_cursored()
        {
            session.Returns(await Json.FromFile("data\\users\\userblocklistcursored.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/blocks/list.json");

            var userc = await session.GetBlockList(0);

            Assert.IsNotNull(userc);
            userc.users.Should().NotBeNull();
            userc.users.ToList().Should().HaveCount(1);
            userc.users.ToList()[0].UserId.ShouldBeEquivalentTo(509466276);
            userc.users.ToList()[0].Name.ShouldBeEquivalentTo("Javier Heady \r");
            userc.users.ToList()[0].IsFollowedByMe.Should().BeFalse();
            userc.users.ToList()[0].Friends.ShouldBeEquivalentTo(0);
        }

        [TestMethod]
        public async Task Create_Block_on_User()
        {
            session.Returns(await Json.FromFile("data\\users\\createblock.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/blocks/create.json");

            var user = await session.CreateUserBlock("theSeanCook",4567);

            Assert.IsNotNull(user);
            user.Name.ShouldBeEquivalentTo("Sean Cook");
        }

        [TestMethod]
        public async Task Delete_Block_on_User()
        {
            session.Returns(await Json.FromFile("data\\users\\createblock.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/blocks/destroy.json");

            var user = await session.DeleteUserBlock("theSeanCook",56);

            Assert.IsNotNull(user);
            user.Name.ShouldBeEquivalentTo("Sean Cook");
        }


        [TestMethod]
        public async Task Get_Users_from_List()
        {
            session.Returns(await Json.FromFile("data\\users\\userlist.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/users/lookup.json");

            var screen_names = new List<string> {"theSeanCook","shiftkey","NickHodgeMSFT"};
            var userlist = await session.GetUsersDetailsFull(screen_names: screen_names);

            Assert.IsNotNull(userlist);
            userlist.Count().ShouldBeEquivalentTo(2);
            userlist.ToList()[1].Name.ShouldBeEquivalentTo("Twitter");
            userlist.ToList()[0].UserId.ShouldBeEquivalentTo(6253282);
        }

        [TestMethod]
        public async Task Search_Users()
        {
            session.Returns(await Json.FromFile("data\\users\\usersearch.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/search.json");

            var userlist = await session.SearchForUsers("blank");

            Assert.IsNotNull(userlist);
            userlist.Count().ShouldBeEquivalentTo(3);
            userlist.ToList()[2].UserId.ShouldBeEquivalentTo(6844292);
        }

    }
}
