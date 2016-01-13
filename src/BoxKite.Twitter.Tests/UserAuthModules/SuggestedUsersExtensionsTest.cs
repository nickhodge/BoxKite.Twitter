// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests
{
    [TestClass]
    public class SuggestedUsersExtensionsTests
    {
        readonly TestableUserSession session = new TestableUserSession();

        [TestMethod]
        public async Task Get_Suggested_User_Lists_as_Slugs()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\suggestions\\suggestions.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/suggestions.json");

            var suggestions = await session.GetSuggestedLists();

            Assert.IsNotNull(suggestions);
            suggestions.Count().ShouldBeEquivalentTo(30);
            suggestions.ToList()[20].Slug.ShouldBeEquivalentTo("news");
         }


        [TestMethod]
        public async Task Get_Suggested_Users_foraSlug()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\suggestions\\suggestedusers.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/users/suggestions/twitter.json");

            var suggestions = await session.GetSuggestedUsers("twitter");

            Assert.IsNotNull(suggestions);
            suggestions.Size.ShouldBeEquivalentTo(20);
            suggestions.Users.Count().ShouldBeEquivalentTo(3);
            suggestions.Users.ToList()[2].Name.ShouldBeEquivalentTo("Twitter Government");
        }
    }
}
