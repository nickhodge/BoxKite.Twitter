using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter.Modules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests.Modules
{
    [TestClass]
    public class ErrorTests
    {
        readonly TestableSession errorsession = new TestableSession();

        [TestMethod]
        public async Task Get_Rate_LimitExceeded_Error()
        {
            // Ref: https://dev.twitter.com/docs/rate-limiting/1.1
            // arrange
            errorsession.simulatingError = true;
            errorsession.httpStatusCode = HttpStatusCode.Gone ; // Twitter : Rate Limit exceeded, RFC6585 Too Many Requests
            errorsession.Returns(await Json.FromFile("data\\errors\\ratelimitexceedederror.txt"));
            errorsession.ExpectGet("https://api.twitter.com/1.1/favorites/list.json");

            var favourites = await errorsession.GetFavourites();

            Assert.IsNotNull(favourites);
            favourites.ToList()[0].twitterFaulted.Should().BeTrue();
            favourites.ToList()[0].TwitterControlMessage.Should().NotBeNull();
            favourites.ToList()[0].TwitterControlMessage.http_status_code.ShouldBeEquivalentTo(410); // Note: testing 410 as 429 is not an enum

        }

    }
}
