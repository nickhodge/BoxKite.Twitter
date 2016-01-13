// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests
{
    [TestClass]
    public class TimelineExtensionsTestsAppAuth
    {
        readonly TestableApplicationSession appsession = new TestableApplicationSession();

        [TestMethod]
        public async Task Get_User_Timeline_AppAuth()
        {
            // arrange
            appsession.Returns(await Json.FromFile("appauthdata\\timeline\\user_timeline.json"));
            appsession.ExpectGet("https://api.twitter.com/1.1/statuses/user_timeline.json");

            var usertimeline = await appsession.GetUserTimeline();

            Assert.IsNotNull(usertimeline);
            Assert.IsTrue(usertimeline.ToList().Count > 0);
            Assert.IsTrue(usertimeline.ToList().Count == 200);
        }



    }
}
