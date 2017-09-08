﻿/// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class TrendsExtensionsTests
    {
        private readonly TestableUserSession session = new TestableUserSession();

        [Fact]
        public async Task Get_Trends_For_Place()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\trends\\trendsforplace.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/trends/place.json");

            var placetrends = await session.GetTrendsForPlace(1);

            placetrends.Should().NotBeNull();
            placetrends.Count().ShouldBeEquivalentTo(1);
            placetrends.ToList()[0].locations.Count().ShouldBeEquivalentTo(1);
            placetrends.ToList()[0].locations.ToList()[0].name.ShouldBeEquivalentTo("Worldwide");
        }


        [Fact]
        public async Task Get_Trend_Locations_Available()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\trends\\possibletrendplaces.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/trends/available.json");

            var placetrends = await session.GetTrendsAvailableLocations();

            placetrends.Should().NotBeNull();
            placetrends.Count().ShouldBeEquivalentTo(5);
            placetrends.ToList()[3].WOEID.ShouldBeEquivalentTo(2477058);
        }

        [Fact]
        public async Task Get_Trends_By_LatLong()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\trends\\trendsbygeo.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/trends/closest.json");

            var placetrends = await session.GetTrendsByLocation(0.0,0.0);

            placetrends.Should().NotBeNull();
            placetrends.Count().ShouldBeEquivalentTo(1);
            placetrends.ToList()[0].Name.ShouldBeEquivalentTo("Australia");
        }

    }
}