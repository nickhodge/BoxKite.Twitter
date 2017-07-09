// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class TimelineExtensionsTestsAppAuth
    {
        readonly TestableApplicationSession appsession = new TestableApplicationSession();

        [Fact]
        public async Task Get_User_Timeline_AppAuth()
        {
            // arrange
            appsession.Returns(await Json.FromFile("appauthdata\\timeline\\user_timeline.json"));
            appsession.ExpectGet("https://api.twitter.com/1.1/statuses/user_timeline.json");

            var usertimeline = await appsession.GetUserTimeline();

            usertimeline.Should().NotBeNull();
            usertimeline.ToList().Count.Should().BeGreaterThan(0);
            usertimeline.ToList().Count.Should().Be(200);
        }



    }
}
