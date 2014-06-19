// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests
{
    [TestClass]
    public class ApiManagemenentExtensionsTestsAppAuth
    {
        readonly TestableApplicationSession appsession = new TestableApplicationSession();

        [TestMethod]
        public async Task Get_Twitter_API_Limits_AppAuth()
        {
            // arrange
            appsession.Returns(await Json.FromFile("appauthdata\\rate_limit_status.json"));
            appsession.ExpectGet("https://api.twitter.com/1.1/application/rate_limit_status.json");

            var api = await appsession.GetCurrentApiStatus();

            Assert.IsNotNull(api);
            Assert.IsTrue(api.APIRateStatuses.Count == 37);
        }

    }
}
