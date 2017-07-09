// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class SearchExtensionsTests
    {
        readonly TestableUserSession session = new TestableUserSession();

        [Fact]
        public async Task Search_Query_received()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\searchresponse.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/search/tweets.json");

            var searchresults = await session.SearchFor("testonly", SearchResultType.Mixed);

            searchresults.Should().NotBeNull();
            searchresults.search_metadata.query.ShouldBeEquivalentTo("%23freebandnames");
            searchresults.Tweets.Should().HaveCount(4);
        }

        [Fact]
        public async Task Search_Query_Geocoded()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\searchresponse.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/search/tweets.json");

            var searchresults = await session.SearchFor("testonly", SearchResultType.Mixed, 37.781157, -122.398720, 1);

            searchresults.Should().NotBeNull();
            searchresults.search_metadata.query.ShouldBeEquivalentTo("%23freebandnames");
            searchresults.Tweets.Should().HaveCount(4);
        }

        [Fact]
        public async Task Get_Saved_Search_Queries()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\savedsearcheslist.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/saved_searches/list.json");

            var searchresults = await session.GetSavedSearches();

            searchresults.Should().NotBeNull();
            searchresults.Should().HaveCount(2);
            searchresults.ToList()[0].Name.ShouldBeEquivalentTo("@twitterapi");
            searchresults.ToList()[1].Id.ShouldBeEquivalentTo(9569730);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-AU");
            searchresults.ToList()[1].DateCreated.ToString().ShouldBeEquivalentTo("15/06/2010 9:38:04 AM +00:00");
        }

        [Fact]
        public async Task Get_A_Saved_Search()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\savedsearch.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/saved_searches/show/62353170.json");

            var searchresults = await session.GetSaveASearch("62353170");

            searchresults.Should().NotBeNull();
            searchresults.Query.ShouldBeEquivalentTo("@anywhere");
        }

        [Fact]
        public async Task Delete_A_Saved_Search()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\savedsearch.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/saved_searches/destroy/62353170.json");

            var searchresults = await session.DeleteSavedSearch("62353170");

            searchresults.Should().NotBeNull();
            searchresults.Query.ShouldBeEquivalentTo("@anywhere");
        }

        [Fact]
        public async Task Create_Save_A_Saved_Search_by_Saving()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\search\\savedsearch.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/saved_searches/create.json");

            var searchresults = await session.CreateSaveSearch("@anywhere");

            searchresults.Should().NotBeNull();
            searchresults.Query.ShouldBeEquivalentTo("@anywhere");
        }
    }
}
