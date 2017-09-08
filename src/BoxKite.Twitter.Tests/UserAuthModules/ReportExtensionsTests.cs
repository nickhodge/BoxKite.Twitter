// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class ReportExtensionsTest
    {
        private readonly TestableUserSession session = new TestableUserSession();

        [Fact]
        public async Task Get_Trends_For_Place()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\report\\spamuserreport.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/users/report_spam.json");

            var spamreport = await session.ReportUserForSpam("screenname");

            spamreport.Should().NotBeNull();
            spamreport.Name.ShouldBeEquivalentTo("Matt Harris");
        }


    }
}