// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests
{
    [TestClass]
    public class ApiManagemenentExtensionsTests
    {
        readonly TestableUserSession session = new TestableUserSession();

        [TestMethod]
        public async Task Get_Twitter_API_Limits()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\apiratelimit.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/application/rate_limit_status.json");

            var api = await session.GetCurrentAPIStatus();

            Assert.IsNotNull(api);
            Assert.IsTrue(api.ApiRateStatuses.Count == 61);
        }

    }
}
