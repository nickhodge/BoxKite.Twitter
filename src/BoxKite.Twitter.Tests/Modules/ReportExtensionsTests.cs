// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests.Modules
{
    [TestClass]
    public class ReportExtensionsTest
    {
        private readonly TestableSession session = new TestableSession();

        [TestMethod]
        public async Task Get_Trends_For_Place()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\report\\spamuserreport.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/users/report_spam.json");

            var spamreport = await session.ReportUserForSpam("screenname");

            Assert.IsNotNull(spamreport);
            spamreport.Name.ShouldBeEquivalentTo("Matt Harris");
        }


    }
}