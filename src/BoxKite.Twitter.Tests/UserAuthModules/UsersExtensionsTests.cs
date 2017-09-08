// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class UsersExtensionsTests
    {
        readonly TestableUserSession session = new TestableUserSession();

        [Fact]
        public async Task Get_Account_Settings()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\users\\accountsettings.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/account/settings.json");

            var user = await session.GetAccountSettings();

            user.Should().NotBeNull();
            user._Protected.Should().BeTrue();
            user.ScreenName.ShouldBeEquivalentTo("theSeanCook");
            user.TrendLocation.ToList()[0].name.ShouldBeEquivalentTo("Atlanta");
        }

        [Fact]
        public async Task Change_Account_Settings()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\users\\accountsettings.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/settings.json");

            var user = await session.ChangeAccountSettings();

            user.Should().NotBeNull();
            user._Protected.Should().BeTrue();
            user.ScreenName.ShouldBeEquivalentTo("theSeanCook");
            user.TrendLocation.ToList()[0].name.ShouldBeEquivalentTo("Atlanta");
        }

        [Fact]
        public async Task Change_Account_Profile()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile.json");

            var user = await session.ChangeAccountProfile();

            user.Should().NotBeNull();
            user.Description.ShouldBeEquivalentTo("Keep calm and rock on.");
        }

        [Fact]
        public async Task Change_Account_Profile_Colours()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_colors.json");

            var user = await session.ChangeAccountColours();

            user.Should().NotBeNull();
            user.Description.ShouldBeEquivalentTo("Keep calm and rock on.");
        }


        [Fact]
        public async Task Get_Profile_WhenUserSent_ReturnsOneValue()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\users\\show.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            var user = await session.GetUserProfile("shiftkey");
            
            user.Should().NotBeNull();
        }

        [Fact]
        public async Task Get_Profile_WhenUserSent_ShowExtendedResults()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\users\\show.extended.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            var user = await session.GetUserProfile("nickhodgemsft");

            user.Should().NotBeNull();
            user.ProfileBackgroundImageUrlHttps.Should().NotBeNull();
        }

        [Fact]
        public async Task Get_Profile_WhenUserSent_ReceivesNameAsParameter()
        {
            var screenName = "shiftkey";
            session.Returns(await Json.FromFile("data\\users\\show.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            // act
            await session.GetUserProfile(screenName);

            session.ReceivedParameter("screen_name", screenName).Should().BeTrue();
            session.ReceivedParameter("include_entities", true.ToString()).Should().BeTrue();
        }

        [Fact]
        public async Task Get_Profile_WhenIdSent_ReceivesNameAsParameter()
        {
            session.Returns(await Json.FromFile("data\\users\\show.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            // act
            var userdetails = await session.GetUserProfile(userId:1234);

            userdetails.Should().NotBeNull();
            userdetails.Name.ShouldBeEquivalentTo("Ryan Sarver");
        }

        [Fact]
        public async Task Get_Profile_WhenIdSent_ParsesResult()
        {
            session.Returns(await Json.FromFile("data\\users\\show.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/show.json");

            var user = await session.GetUserProfile(userId: 1234);
            user.Should().NotBeNull();
        }

        [Fact]
        public async Task Change_Profile_Image()
        {
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_image.json");

            var user = await session.ChangeAccountProfileImage("test.jpg", new byte[]{});

            user.Should().NotBeNull();
            user.UserId.ShouldBeEquivalentTo(776627022);
        }

        [Fact]
        public async Task Change_BackgroundImage()
        {
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_background_image.json");

            var user = await session.ChangeAccountBackgroundImage("test.jpg", new byte[]{});

            user.Should().NotBeNull();
            user.UserId.ShouldBeEquivalentTo(776627022);
        }

        [Fact]
        public async Task Get_Banner()
        {
            session.Returns(await Json.FromFile("data\\users\\bannerprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_banner.json");

            var banner = await session.GetProfileBanners(screenName:"boxkite");

            banner.Should().NotBeNull();
            banner.sizes.Should().NotBeNull();
            banner.sizes.Standard_1500x500.Should().NotBeNull();
            banner.sizes.Standard_1500x500.Url.Should().NotBeNull();
            banner.sizes.Standard_1500x500.Width.ShouldBeEquivalentTo(1500);
            banner.sizes.Standard_1500x500.Height.ShouldBeEquivalentTo(500);
        }

        [Fact]
        public async Task Change_Account_Colours()
        {
            session.Returns(await Json.FromFile("data\\users\\accountprofile.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/account/update_profile_colors.json");

            var user = await session.ChangeAccountColours();

            user.Should().NotBeNull();
            user.UserId.ShouldBeEquivalentTo(776627022);
        }

        [Fact]
        public async Task Get_BlockList_cursored()
        {
            session.Returns(await Json.FromFile("data\\users\\userblocklistcursored.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/blocks/list.json");

            var userc = await session.GetBlockList(0);

            userc.Should().NotBeNull();
            userc.users.Should().NotBeNull();
            userc.users.ToList().Should().HaveCount(1);
            userc.users.ToList()[0].UserId.ShouldBeEquivalentTo(509466276);
            userc.users.ToList()[0].Name.ShouldBeEquivalentTo("Javier Heady \r");
            userc.users.ToList()[0].IsFollowedByMe.Should().BeFalse();
            userc.users.ToList()[0].Friends.ShouldBeEquivalentTo(0);
        }

        [Fact]
        public async Task Create_Block_on_User()
        {
            session.Returns(await Json.FromFile("data\\users\\createblock.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/blocks/create.json");

            var user = await session.CreateUserBlock("theSeanCook",4567);

            user.Should().NotBeNull();
            user.Name.ShouldBeEquivalentTo("Sean Cook");
        }

        [Fact]
        public async Task Delete_Block_on_User()
        {
            session.Returns(await Json.FromFile("data\\users\\createblock.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/blocks/destroy.json");

            var user = await session.DeleteUserBlock("theSeanCook",56);

            user.Should().NotBeNull();
            user.Name.ShouldBeEquivalentTo("Sean Cook");
        }


        [Fact]
        public async Task Get_Users_from_List()
        {
            session.Returns(await Json.FromFile("data\\users\\userlist.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/users/lookup.json");

            var screen_names = new List<string> {"theSeanCook","shiftkey","NickHodgeMSFT"};
            var userlist = await session.GetUsersDetailsFull(screenNames: screen_names);

            userlist.Should().NotBeNull();
            userlist.Count().ShouldBeEquivalentTo(2);
            userlist.ToList()[1].Name.ShouldBeEquivalentTo("Twitter");
            userlist.ToList()[0].UserId.ShouldBeEquivalentTo(6253282);
        }

        [Fact]
        public async Task Search_Users()
        {
            session.Returns(await Json.FromFile("data\\users\\usersearch.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/search.json");

            var userlist = await session.SearchForUsers("blank");

            userlist.Should().NotBeNull();
            userlist.Count().ShouldBeEquivalentTo(3);
            userlist.ToList()[2].UserId.ShouldBeEquivalentTo(6844292);
        }

        [Fact]
        public async Task Get_Configuration()
        {
            session.Returns(await Json.FromFile("data\\users\\getconfiguration.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/help/configuration.json");

            var configuration = await session.GetConfiguration();

            configuration.Should().NotBeNull();
            configuration.MaxMediaPerUpload.ShouldBeEquivalentTo(1);
            configuration.PhotoSizeLimit.ShouldBeEquivalentTo(3145728);
            configuration.ShortUrlLengthHttps.ShouldBeEquivalentTo(23);

        }
    }
}
