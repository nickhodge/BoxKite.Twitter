// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests
{
    [TestClass]
    public class TweetExtensionsTestsAppAuth
    {
        readonly TestableUserSession session = new TestableUserSession();

        [TestMethod]
        public async Task Get_Retweets_Of_Tweet_AppAuth()
        {
            // arrange
            session.Returns(await Json.FromFile("appauthdata\\timeline\\453310114796412928.json"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/retweets/453310114796412928.json");

            var retweetslist = await session.GetRetweets(new Tweet { Id = 453310114796412928});

            Assert.IsNotNull(retweetslist);
            Assert.IsTrue(retweetslist.ToList().Count > 0);
            Assert.IsTrue(retweetslist.ToList().Count == 4);
         }

    }
}
