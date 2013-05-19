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
            api.help.helpprivacy.limit.ShouldBeEquivalentTo(15);
            api.search.searchtweets.limit.ShouldBeEquivalentTo(180);
            api.statuses.statusesretweetsid.remaining.ShouldBeEquivalentTo(15);
            api.users.usersshow.reset.ShouldBeEquivalentTo(1346439527);
            api.statuses.statuseshome_timeline.ResetTime.ShouldBeEquivalentTo(new DateTime(634820723270000000));
        }

    }
}
