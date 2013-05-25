// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests
{
    [TestClass]
    public class FavouritesTests
    {
        readonly TestableSession session = new TestableSession();

        [TestMethod]
        public async Task Get_Favourites_For_CurrentUser_ReturnsSet()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\favorites\\favoritelist.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/favorites/list.json");

            var favourites = await session.GetFavourites();

            Assert.IsTrue(favourites.Count() > 0);
            favourites.ToList().Should().HaveCount(2);
            favourites.ToList()[0].Favourited.Should().BeTrue();
            favourites.ToList()[1].Favourited.Should().BeTrue();
        }

        [TestMethod]
        public async Task Create_Favourite_For_CurrentUser_ReturnsResult()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\favorites\\favorited.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/favorites/create.json");

            var tweet = new Tweet { Id = 1234 };

            var favourite = await session.CreateFavourite(tweet);

            Assert.IsNotNull(favourite);
            Assert.IsTrue(favourite.Favourited);
        }

        [TestMethod]
        public async Task Delete_Favourite_For_CurrentUser_ReturnsResult()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\favorites\\unfavorited.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/favorites/destroy.json");

            var tweet = new Tweet { Id = 1234 };

            var favourite = await session.DeleteFavourite(tweet);

            Assert.IsNotNull(favourite);
            Assert.IsFalse(favourite.Favourited);
        }

    }
}
