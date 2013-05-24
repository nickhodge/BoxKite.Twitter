// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests.Modules
{
    [TestClass]
    public class SearchExtensionsTests
    {
        readonly TestableSession session = new TestableSession();

        [TestMethod]
        public async Task Search_Query_received()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\searchresponse.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/search/tweets.json");

            var searchresults = await session.SearchFor("testonly", SearchResultType.Mixed);

            Assert.IsNotNull(searchresults);
            searchresults.search_metadata.query.ShouldBeEquivalentTo("%23freebandnames");
            searchresults.Tweets.Should().HaveCount(4);
        }

        [TestMethod]
        public async Task Search_Query_Geocoded()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\searchresponse.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/search/tweets.json");

            var searchresults = await session.SearchFor("testonly", SearchResultType.Mixed, 37.781157, -122.398720, 1);

            Assert.IsNotNull(searchresults);
            searchresults.search_metadata.query.ShouldBeEquivalentTo("%23freebandnames");
            searchresults.Tweets.Should().HaveCount(4);
        }

        [TestMethod]
        public async Task Get_Saved_Search_Queries()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\savedsearcheslist.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/saved_searches/list.json");

            var searchresults = await session.GetSavedSearches();

            Assert.IsNotNull(searchresults);
            searchresults.Should().HaveCount(2);
            searchresults.ToList()[0].Name.ShouldBeEquivalentTo("@twitterapi");
            searchresults.ToList()[1].Id.ShouldBeEquivalentTo(9569730);
            searchresults.ToList()[1].DateCreated.ToString().ShouldBeEquivalentTo("15/06/2010 9:38:04 AM +00:00");
        }

        [TestMethod]
        public async Task Get_A_Saved_Search()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\savedsearch.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/saved_searches/show/62353170.json");

            var searchresults = await session.GetSaveASearch("62353170");

            Assert.IsNotNull(searchresults);
            searchresults.Query.ShouldBeEquivalentTo("@anywhere");
        }

        [TestMethod]
        public async Task Delete_A_Saved_Search()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\savedsearch.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/saved_searches/destroy/62353170.json");

            var searchresults = await session.DeleteSavedSearch("62353170");

            Assert.IsNotNull(searchresults);
            searchresults.Query.ShouldBeEquivalentTo("@anywhere");
        }

        [TestMethod]
        public async Task Create_Save_A_Saved_Search_by_Saving()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\savedsearch.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/saved_searches/create.json");

            var searchresults = await session.CreateSaveSearch("@anywhere");

            Assert.IsNotNull(searchresults);
            searchresults.Query.ShouldBeEquivalentTo("@anywhere");
        }
    }
}
