// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class ApiManagemenentExtensionsTests
    {
        readonly TestableUserSession session = new TestableUserSession();

        [Fact]
        public async Task Get_Twitter_API_Limits()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\apiratelimit.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/application/rate_limit_status.json");

            var api = await session.GetCurrentApiStatus();

            api.Should().NotBeNull();
            api.APIRateStatuses.Count.Should().Be(61);
        }

    }
}
