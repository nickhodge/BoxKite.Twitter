// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.Linq;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class TweetExtensionsTestsAppAuth
    {
        readonly TestableUserSession session = new TestableUserSession();

        [Fact]
        public async Task Get_Retweets_Of_Tweet_AppAuth()
        {
            // arrange
            session.Returns(await Json.FromFile("appauthdata\\timeline\\453310114796412928.json"));
            session.ExpectGet("https://api.twitter.com/1.1/statuses/retweets/453310114796412928.json");

            var retweetslist = await session.GetRetweets(new Tweet { Id = 453310114796412928});

            retweetslist.Should().NotBeNull();
            retweetslist.ToList().Count.Should().BeGreaterThan(0);
            retweetslist.ToList().Count.Should().Be(4);
         }

    }
}
