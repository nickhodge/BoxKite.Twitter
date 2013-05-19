using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Configuration;
using System.Reactive.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using BoxKite.Twitter.Models.Service;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests.Modules
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

            var user = await session.GetProfile("shiftkey");
            
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task Get_Profile_WhenUserSent_ReceivesNameAsParameter()
        {
            var screenName = "shiftkey";
            session.Returns(await Json.FromFile("data\\users\\show.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            // act
            await session.GetProfile(screenName);

            Assert.IsTrue(session.ReceivedParameter("screen_name", screenName));
            Assert.IsTrue(session.ReceivedParameter("include_entities", "true"));
        }

        [TestMethod]
        public async Task Get_Profile_WhenIdSent_ReceivesNameAsParameter()
        {
            session.Returns(await Json.FromFile("data\\users\\show.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            // act
            var userdetails = await session.GetProfile(1234.ToString());

            Assert.IsNotNull(userdetails);
            userdetails.Name.ShouldBeEquivalentTo("Ryan Sarver");
        }

        [TestMethod]
        public async Task Get_Profile_WhenIdSent_ParsesResult()
        {
            session.Returns(await Json.FromFile("data\\users\\show.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            var user = await session.GetProfile(1234.ToString());

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task Change_Profile_Image()
        {
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_image.json");

            var user = await session.ChangeAccountProfileImage("test.jpg", new byte[]{});

            Assert.IsNotNull(user);
            user.Id.ShouldBeEquivalentTo(776627022);
        }

        [TestMethod]
        public async Task Change_BackgroundImage()
        {
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_background_image.json");

            var user = await session.ChangeAccountBackgroundImage("test.jpg", new byte[]{});

            Assert.IsNotNull(user);
            user.Id.ShouldBeEquivalentTo(776627022);
        }

        [TestMethod]
        public async Task Change_Account_Colours()
        {
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_colors.json");

            var user = await session.ChangeAccountColours();

            Assert.IsNotNull(user);
            user.Id.ShouldBeEquivalentTo(776627022);
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

            var user = await session.CreateBlock("theSeanCook");

            Assert.IsNotNull(user);
            user.Name.ShouldBeEquivalentTo("Sean Cook");
        }

        [TestMethod]
        public async Task Delete_Block_on_User()
        {
            session.Returns(await Json.FromFile("data\\users\\createblock.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/blocks/destroy.json");

            var user = await session.DeleteBlock("theSeanCook");

            Assert.IsNotNull(user);
            user.Name.ShouldBeEquivalentTo("Sean Cook");
        }


        [TestMethod]
        public async Task Get_Users_from_List()
        {
            session.Returns(await Json.FromFile("data\\users\\userlist.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/users/lookup.json");

            var screen_names = new List<string> {"theSeanCook","shiftkey","NickHodgeMSFT"};
            var ids = new List<int>();
            var userlist = await session.GetFullUserDetails(screen_names, ids);

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
