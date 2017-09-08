// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class ApiManagemenentExtensionsTestsAppAuth
    {
        readonly TestableApplicationSession appsession = new TestableApplicationSession();

        [Fact]
        public async Task Get_Twitter_API_Limits_AppAuth()
        {
            // arrange
            appsession.Returns(await Json.FromFile("appauthdata\\rate_limit_status.json"));
            appsession.ExpectGet("https://api.twitter.com/1.1/application/rate_limit_status.json");

            var api = await appsession.GetCurrentApiStatus();

            api.Should().NotBeNull();
            api.APIRateStatuses.Count.Should().Be(37);
        }

    }
}
