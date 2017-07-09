// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System;
using System.IO;
using System.Linq;
using BoxKite.Twitter.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class DecodeTests
    {

        [Fact]
        public void Decode_Deep_ReTweet()
        {
            // arrange
            var fileName = @".\data\retweet.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);

            tweet.Should().NotBeNull();
            tweet.RetweetedStatus.Should().NotBeNull();
            tweet.RetweetedStatus.User.Should().NotBeNull();
        }

        [Fact]
        public void Decode_Entity_HashTag_Separate_Tests()
        {
            var fileName = ".\\data\\entity-with-hashtag.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);
            var entity = tweet.Entities;
            var text = tweet.Text;

            entity.Should().NotBeNull();
            entity.Hashtags.Should().NotBeNull();
            entity.Hashtags.Count().ShouldBeEquivalentTo(1);
            entity.Hashtags.ToList()[0].Text.ShouldBeEquivalentTo("devnestSF");
            entity.Hashtags.ToList()[0].indices.Count().ShouldBeEquivalentTo(2);
            
            // do indirect check of string for hashtag
            var zeroOffsetStart = entity.Hashtags.ToList()[0].indices.ToList()[0];
            var zeroOffsetEnd = entity.Hashtags.ToList()[0].indices.ToList()[1];

            // +1 -1 is to remove the #
            QuickSubStringHelper(text, "devnestSF", zeroOffsetStart + 1, (zeroOffsetEnd - zeroOffsetStart - 1)).Should()
                .BeTrue();
        }

        [Fact]
        public void Decode_Entity_Mentions_Separate_Tests()
        {
            var fileName = ".\\data\\entity-with-mentions.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);
            var entity = tweet.Entities;
            var text = tweet.Text;

            entity.Should().NotBeNull();
            entity.Mentions.Should().NotBeNull();
            entity.Mentions.Count().ShouldBeEquivalentTo(1);
            entity.Mentions.ToList()[0].Id.ShouldBeEquivalentTo(22548447);
            entity.Mentions.ToList()[0].Name.ShouldBeEquivalentTo("Arnaud Meunier");
            entity.Mentions.ToList()[0].ScreenName.ShouldBeEquivalentTo("rno");
            entity.Mentions.ToList()[0].indices.Count().ShouldBeEquivalentTo(2);

            // do indirect check of string for hashtag
            var zeroOffsetStart = entity.Mentions.ToList()[0].indices.ToList()[0];
            var zeroOffsetEnd = entity.Mentions.ToList()[0].indices.ToList()[1];
            // +1 -1 is to remove the @
            QuickSubStringHelper(text, "rno", zeroOffsetStart + 1, (zeroOffsetEnd - zeroOffsetStart - 1)).Should()
                .BeTrue();
        }

        [Fact]
        public void Decode_Entity_Urls_Separate_Tests()
        {
            var fileName = ".\\data\\entity-with-urls.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);
            var entity = tweet.Entities;
            var text = tweet.Text;

            entity.Should().NotBeNull();
            entity.Urls.Should().NotBeNull();
            entity.Urls.Count().ShouldBeEquivalentTo(1);
            entity.Urls.ToList()[0]._Url.ShouldBeEquivalentTo("http://t.co/0JG5Mcq");
            entity.Urls.ToList()[0].DisplayUrl.ShouldBeEquivalentTo("blog.twitter.com/2011/05/twitte…");
            entity.Urls.ToList()[0].ExpandedUrl.ShouldBeEquivalentTo("http://blog.twitter.com/2011/05/twitter-for-mac-update.html");
            entity.Urls.ToList()[0].indices.Count().ShouldBeEquivalentTo(2);

            // do indirect check of string for hashtag
            var zeroOffsetStart = entity.Urls.ToList()[0].indices.ToList()[0];
            var zeroOffsetEnd = entity.Urls.ToList()[0].indices.ToList()[1];
            QuickSubStringHelper(text, "http://t.co/0JG5Mcq", zeroOffsetStart, (zeroOffsetEnd - zeroOffsetStart))
                .Should().BeTrue();
        }

        [Fact]
        public void Decode_Entity_Media_Separate_Tests()
        {
            var fileName = ".\\data\\entity-with-media.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);
            var entity = tweet.Entities;
            var text = tweet.Text;

            entity.Media.Should().NotBeNull();
            entity.Media.Count().ShouldBeEquivalentTo(1);
            entity.Media.ToList()[0].Id.ShouldBeEquivalentTo(76360760611180544);
            entity.Media.ToList()[0].MediaUrl.ShouldBeEquivalentTo("http://p.twimg.com/AQ9JtQsCEAA7dEN.jpg");
            entity.Media.ToList()[0].MediaUrlHttps.ShouldBeEquivalentTo("https://p.twimg.com/AQ9JtQsCEAA7dEN.jpg");
            entity.Media.ToList()[0].Url.ShouldBeEquivalentTo("http://t.co/qbJx26r");
            entity.Media.ToList()[0].DisplayUrl.ShouldBeEquivalentTo("pic.twitter.com/qbJx26r");
            entity.Media.ToList()[0].ExpandedUrl.ShouldBeEquivalentTo("http://twitter.com/twitter/status/76360760606986241/photo/1");
            entity.Media.ToList()[0].Sizes.Should().NotBeNull();
            entity.Media.ToList()[0].Sizes.Large.Should().NotBeNull();
            entity.Media.ToList()[0].Sizes.Medium.Should().NotBeNull();
            entity.Media.ToList()[0].Sizes.Small.Should().NotBeNull();
            entity.Media.ToList()[0].Sizes.Thumb.Should().NotBeNull();
            entity.Media.ToList()[0].Sizes.Large.Height.ShouldBeEquivalentTo(466);
            entity.Media.ToList()[0].Sizes.Large.Width.ShouldBeEquivalentTo(700);
            entity.Media.ToList()[0].Sizes.Large.Resize.ShouldBeEquivalentTo("fit");
            entity.Media.ToList()[0].Type.ShouldBeEquivalentTo("photo");
            entity.Media.ToList()[0].indices.Count().ShouldBeEquivalentTo(2);

            // do indirect check of string for media
            var zeroOffsetStart = entity.Media.ToList()[0].indices.ToList()[0];
            var zeroOffsetEnd = entity.Media.ToList()[0].indices.ToList()[1];
            QuickSubStringHelper(text, "http://t.co/qbJx26r", zeroOffsetStart, (zeroOffsetEnd - zeroOffsetStart))
                .Should().BeTrue();
        }

        [Fact]
        public void Decode_Tweet_Deep_Tests()
        {
            var fileName = ".\\data\\tweets\\singletweet.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);

            tweet.Should().NotBeNull();

            if (tweet.RetweetCount != null)
                tweet.RetweetCount.ShouldBeEquivalentTo(0);
            tweet.Id.ShouldBeEquivalentTo(243145735212777472);

            tweet.InReplyToId.Should().NotBeNull();
            tweet.InReplyToUserId.Should().NotBeNull();
            tweet.Location.Should().NotBeNull();
            tweet.Place.Should().NotBeNull();
            tweet.RetweetedStatus.Should().NotBeNull();
            tweet.Favourited.ShouldBeEquivalentTo(false);
            tweet.FavoriteCount.ShouldBeEquivalentTo(10);
            tweet.Truncated.ShouldBeEquivalentTo(true);
            tweet.Source.ShouldBeEquivalentTo(
                "<a href=\"http://jason-costa.blogspot.com\" rel=\"nofollow\">My Shiny App</a>");
            tweet.Text.ShouldBeEquivalentTo("Maybe he'll finally find his keys. #peterfalk");
            tweet.Time.ToString().ShouldBeEquivalentTo("5/09/2012 12:37:15 AM +00:00");

            //check user details
            tweet.User.Should().NotBeNull();
            tweet.User.Name.ShouldBeEquivalentTo("Jason Costa");
            tweet.User.Followers.ShouldBeEquivalentTo(8760);
            tweet.User.ScreenName.ShouldBeEquivalentTo("jasoncosta");
            tweet.User.IsFollowedByMe.ShouldBeEquivalentTo(false);

            //check entities
            tweet.Entities.Should().NotBeNull();
        }

        [Fact]
        public void Decode_User_Deep_Tests()
        {
            var fileName = ".\\data\\users\\show.txt";
            var json = File.ReadAllText(fileName);
            var twitteruser = JsonConvert.DeserializeObject<User>(json);

            twitteruser.Should().NotBeNull();
            twitteruser.Followers.ShouldBeEquivalentTo(276334);
            twitteruser.Friends.ShouldBeEquivalentTo(1780);
            twitteruser.IsFollowedByMe.ShouldBeEquivalentTo(false);
            twitteruser.IsProtected.ShouldBeEquivalentTo(false);
            twitteruser.Name.ShouldBeEquivalentTo("Ryan Sarver");
            twitteruser.ScreenName.ShouldBeEquivalentTo("rsarver");
            twitteruser.UserId.ShouldBeEquivalentTo(795649);
            twitteruser.TimeZone.ShouldBeEquivalentTo("Pacific Time (US & Canada)");
            twitteruser.ContributorsEnabled.ShouldBeEquivalentTo(true);
            twitteruser.CreatedAt.Ticks.ShouldBeEquivalentTo(633081099550000000);
            twitteruser.DefaultProfile.ShouldBeEquivalentTo(false);
            twitteruser.Description.ShouldBeEquivalentTo("Director, Platform at Twitter. Detroit and Boston export. Foodie and over-the-hill hockey player. @devon's lesser half");
            twitteruser.DefaultProfileImage.ShouldBeEquivalentTo(false);
            twitteruser.FavouritesCount.ShouldBeEquivalentTo(3162);
            twitteruser.FollowRequestSent.ShouldBeEquivalentTo(false);
            twitteruser.Followers.ShouldBeEquivalentTo(276334);
            twitteruser.Friends.ShouldBeEquivalentTo(1780);
            twitteruser.GeoEnabled.ShouldBeEquivalentTo(true);
            twitteruser.IsTranslator.ShouldBeEquivalentTo(false);
            twitteruser.Language.ShouldBeEquivalentTo("en");
            twitteruser.ListedCount.ShouldBeEquivalentTo(1586);
            twitteruser.Location.ShouldBeEquivalentTo("San Francisco, CA");
            twitteruser.Notifications.ShouldBeEquivalentTo(false);
            twitteruser.ProfileBackgroundColour.ShouldBeEquivalentTo("45ADA8");
            twitteruser.ProfileBackgroundTile.ShouldBeEquivalentTo(true);
            twitteruser.Avatar.ShouldBeEquivalentTo("http://a0.twimg.com/profile_images/1777569006/image1327396628_normal.png");
            twitteruser.ProfileLinkColour.ShouldBeEquivalentTo("547980");
            twitteruser.ProfileSidebarBorderColour.ShouldBeEquivalentTo("547980");
            twitteruser.ShowAllInlineMedia.ShouldBeEquivalentTo(true);
            twitteruser.StatusesCount.ShouldBeEquivalentTo(13728);
            twitteruser.UTCOffset.ShouldBeEquivalentTo(-28800);
            twitteruser.Url.Should().NotBeNull();
        }


        private bool QuickSubStringHelper(string haystack, string needle, int zeroOffsetStart, int length)
        {
            var found = haystack.Substring(zeroOffsetStart, length);
            return (needle == found);
        }
    }
}
