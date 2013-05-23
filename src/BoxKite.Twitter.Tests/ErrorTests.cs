using System.Net;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Modules;
using BoxKite.Twitter.Tests.Modules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests
{
    [TestClass]
    public class ErrorTests
    {
        readonly TestableSession errorsession = new TestableSession();

        [TestMethod]
        public async Task Get_Rate_LimitExceeded_Error_OnCollection()
        {
            // Ref: https://dev.twitter.com/docs/rate-limiting/1.1
            // arrange
            errorsession.simulatingError = true;
            errorsession.httpStatusCode = HttpStatusCode.Gone ; // Twitter : Rate Limit exceeded, RFC6585 Too Many Requests
            errorsession.Returns(await Json.FromFile("data\\errors\\ratelimitexceedederror.txt"));
            errorsession.ExpectGet("https://api.twitter.com/1.1/favorites/list.json");

            var favourites = await errorsession.GetFavourites();

            Assert.IsNotNull(favourites);
            favourites.twitterFaulted.Should().BeTrue();
            favourites.twitterControlMessage.Should().NotBeNull();
            favourites.twitterControlMessage.http_status_code.ShouldBeEquivalentTo(410); // Note: testing 410 as 429 is not an enum
        }


        [TestMethod]
        public async Task Get_Rate_LimitExceeded_Error_OnSingle()
        {
            // Ref: https://dev.twitter.com/docs/rate-limiting/1.1
            // arrange
            errorsession.simulatingError = true;
            errorsession.httpStatusCode = HttpStatusCode.Gone; // Twitter : Rate Limit exceeded, RFC6585 Too Many Requests
            errorsession.Returns(await Json.FromFile("data\\errors\\ratelimitexceedederror.txt"));
            errorsession.ExpectGet("/1.1/favorites/create.json");
            var twt = new Tweet();
            var favourites = await errorsession.CreateFavourite(twt);

            Assert.IsNotNull(favourites);
            favourites.twitterFaulted.Should().BeTrue();
            favourites.twitterControlMessage.Should().NotBeNull();
            favourites.twitterControlMessage.http_status_code.ShouldBeEquivalentTo(410); // Note: testing 410 as 429 is not an enum
        }


        [TestMethod]
        public async Task Get_Lists_Parameter_EitherOr_Error()
        {
            errorsession.simulatingError = true;
            errorsession.httpStatusCode = HttpStatusCode.PreconditionFailed; // Twitter : Rate Limit exceeded, RFC6585 Too Many Requests
            errorsession.Returns(await Json.FromFile("data\\lists\\getuserlists.txt"));
            errorsession.ExpectGet("/1.1/lists/list.json");
            var listserror1 = await errorsession.GetLists();

            Assert.IsNotNull(listserror1);
            listserror1.twitterFaulted.Should().BeTrue();
            listserror1.twitterControlMessage.Should().NotBeNull();
            listserror1.twitterControlMessage.twitter_error_message.ShouldBeEquivalentTo("Parameter Error: Either screen_name or user_id required");
        }


    }
}
