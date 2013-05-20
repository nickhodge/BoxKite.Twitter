using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests.Modules
{
    [TestClass]
    public class ApiManagemenentExtensionsTests
    {
        readonly TestableSession session = new TestableSession();

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
