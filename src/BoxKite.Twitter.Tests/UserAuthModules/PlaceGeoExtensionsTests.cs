﻿// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class PlaceGeoExtensionsTests
    {
        readonly TestableUserSession session = new TestableUserSession();

        [Fact]
        public async Task Get_Place_Info()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\placegeo\\placeinfo.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/geo/id/df51dec6f4ee2b2c.json");

            var placegeo = await session.GetPlaceInfo("df51dec6f4ee2b2c");

            placegeo.Should().NotBeNull();
            placegeo.FullName.ShouldBeEquivalentTo("Presidio, San Francisco");
            placegeo.BoundingBox.GeoCoordinates[0][0][0].ShouldBeEquivalentTo(-122.48530488);
        }

        [Fact]
        public async Task Get_Places_From_Current_Location()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\placegeo\\reversegeocoderesponse.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/geo/reverse_geocode.json");

            var placegeolist = await session.GetPlaceIDFromGeocode();

            placegeolist.Should().NotBeNull();
            placegeolist.query.type.ShouldBeEquivalentTo("reverse_geocode");
            placegeolist.query._params.granularity.ShouldBeEquivalentTo("neighborhood");
            placegeolist.result.places.Count().ShouldBeEquivalentTo(4);
            placegeolist.result.places.ToList()[0].Id.ShouldBeEquivalentTo("cf7afb4ee6011bca");
        }


        [Fact]
        public async Task Get_Places_for_Tweetage()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\placegeo\\geosearchresponse.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/geo/search.json");

            var placegeolist = await session.GetPlaceIDFromInfo();

            placegeolist.Should().NotBeNull();
            placegeolist.query.type.ShouldBeEquivalentTo("search");
            placegeolist.result.places.Count().ShouldBeEquivalentTo(5);
            placegeolist.result.places.ToList()[0].Id.ShouldBeEquivalentTo("3e8542a1e9f82870");
        }

        [Fact]
        public async Task Get_Similar_Places()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\placegeo\\similarplaces.txt"));
            session.ExpectGet("https://api.twitter.com/1.1/geo/similar_places.json");

            var placegeolist = await session.GetPlaceSimilarName();

            placegeolist.Should().NotBeNull();
            placegeolist.query.type.ShouldBeEquivalentTo("similar_places");
            placegeolist.result.places.Count().ShouldBeEquivalentTo(1);
            placegeolist.result.token.ShouldBeEquivalentTo("19153cc4df966b1787165f4620baa6a0");
            placegeolist.result.places.ToList()[0].Id.ShouldBeEquivalentTo("3bdf30ed8b201f31");
        }


        [Fact]
        public async Task Create_Place()
        {
            // arrange
            session.Returns(await Json.FromFile("data\\placegeo\\addplaceresponse.txt"));
            session.ExpectPost("https://api.twitter.com/1.1/geo/create.json");

            var placeadded = await session.CreatePlace("yo",0.0,0.0,"city","far43");

            placeadded.Should().NotBeNull();
            placeadded.id.ShouldBeEquivalentTo("6b9811c8d9de10b9");
        }

    }
}
