// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.IO;
using System.Linq;
using BoxKite.Twitter.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Tests
{
    [TestClass]
    public class DecodeTests
    {
        [TestMethod]
        public void Decode_Entity_HashTag_Separate_Tests()
        {
            var fileName = ".\\data\\entity-with-hashtag.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);
            var entity = tweet.Entities;
            var text = tweet.Text;

            Assert.IsNotNull(entity);
            Assert.IsInstanceOfType(entity,typeof(BoxKite.Twitter.Models.Entities));
            Assert.IsNotNull(entity.Hashtags);
            entity.Hashtags.Count().ShouldBeEquivalentTo(1);
            entity.Hashtags.ToList()[0].Text.ShouldBeEquivalentTo("devnestSF");
            entity.Hashtags.ToList()[0].indices.Count().ShouldBeEquivalentTo(2);
            
            // do indirect check of string for hashtag
            var zeroOffsetStart = entity.Hashtags.ToList()[0].indices.ToList()[0];
            Assert.IsInstanceOfType(zeroOffsetStart,typeof(Int32));
            var zeroOffsetEnd = entity.Hashtags.ToList()[0].indices.ToList()[1];
            Assert.IsInstanceOfType(zeroOffsetEnd, typeof(Int32));
            Assert.IsTrue(QuickSubStringHelper(text, "devnestSF",zeroOffsetStart+1 ,(zeroOffsetEnd - zeroOffsetStart -1))); // +1 -1 is to remove the #
        }

        [TestMethod]
        public void Decode_Entity_Mentions_Separate_Tests()
        {
            var fileName = ".\\data\\entity-with-mentions.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);
            var entity = tweet.Entities;
            var text = tweet.Text;

            Assert.IsNotNull(entity);
            Assert.IsInstanceOfType(entity, typeof(BoxKite.Twitter.Models.Entities));
            Assert.IsNotNull(entity.Mentions);
             entity.Mentions.Count().ShouldBeEquivalentTo(1);
            entity.Mentions.ToList()[0].Id.ShouldBeEquivalentTo(22548447);
            entity.Mentions.ToList()[0].Name.ShouldBeEquivalentTo("Arnaud Meunier");
            entity.Mentions.ToList()[0].ScreenName.ShouldBeEquivalentTo("rno");
            entity.Mentions.ToList()[0].indices.Count().ShouldBeEquivalentTo(2);

            // do indirect check of string for hashtag
            var zeroOffsetStart = entity.Mentions.ToList()[0].indices.ToList()[0];
            Assert.IsInstanceOfType(zeroOffsetStart, typeof(Int32));
            var zeroOffsetEnd = entity.Mentions.ToList()[0].indices.ToList()[1];
            Assert.IsInstanceOfType(zeroOffsetEnd, typeof(Int32));
            Assert.IsTrue(QuickSubStringHelper(text, "rno", zeroOffsetStart + 1, (zeroOffsetEnd - zeroOffsetStart - 1))); // +1 -1 is to remove the @
        }

        [TestMethod]
        public void Decode_Entity_Urls_Separate_Tests()
        {
            var fileName = ".\\data\\entity-with-urls.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);
            var entity = tweet.Entities;
            var text = tweet.Text;

            Assert.IsNotNull(entity);
            Assert.IsInstanceOfType(entity, typeof(BoxKite.Twitter.Models.Entities));
            Assert.IsNotNull(entity.Urls);
            entity.Urls.Count().ShouldBeEquivalentTo(1);
            entity.Urls.ToList()[0]._Url.ShouldBeEquivalentTo("http://t.co/0JG5Mcq");
            entity.Urls.ToList()[0].DisplayUrl.ShouldBeEquivalentTo("blog.twitter.com/2011/05/twitte…");
            entity.Urls.ToList()[0].ExpandedUrl.ShouldBeEquivalentTo("http://blog.twitter.com/2011/05/twitter-for-mac-update.html");
            entity.Urls.ToList()[0].indices.Count().ShouldBeEquivalentTo(2);

            // do indirect check of string for hashtag
            var zeroOffsetStart = entity.Urls.ToList()[0].indices.ToList()[0];
            Assert.IsInstanceOfType(zeroOffsetStart, typeof(Int32));
            var zeroOffsetEnd = entity.Urls.ToList()[0].indices.ToList()[1];
            Assert.IsInstanceOfType(zeroOffsetEnd, typeof(Int32));
            Assert.IsTrue(QuickSubStringHelper(text, "http://t.co/0JG5Mcq", zeroOffsetStart, (zeroOffsetEnd - zeroOffsetStart)));
        }

        [TestMethod]
        public void Decode_Entity_Media_Separate_Tests()
        {
            var fileName = ".\\data\\entity-with-media.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);
            var entity = tweet.Entities;
            var text = tweet.Text;

            Assert.IsNotNull(entity);
            Assert.IsInstanceOfType(entity, typeof(BoxKite.Twitter.Models.Entities));
            Assert.IsNotNull(entity.Media);
            entity.Media.Count().ShouldBeEquivalentTo(1);
            entity.Media.ToList()[0].Id.ShouldBeEquivalentTo(76360760611180544);
            entity.Media.ToList()[0].MediaUrl.ShouldBeEquivalentTo("http://p.twimg.com/AQ9JtQsCEAA7dEN.jpg");
            entity.Media.ToList()[0].MediaUrlHttps.ShouldBeEquivalentTo("https://p.twimg.com/AQ9JtQsCEAA7dEN.jpg");
            entity.Media.ToList()[0].Url.ShouldBeEquivalentTo("http://t.co/qbJx26r");
            entity.Media.ToList()[0].DisplayUrl.ShouldBeEquivalentTo("pic.twitter.com/qbJx26r");
            entity.Media.ToList()[0].ExpandedUrl.ShouldBeEquivalentTo("http://twitter.com/twitter/status/76360760606986241/photo/1");
            Assert.IsNotNull(entity.Media.ToList()[0].Sizes);
            Assert.IsNotNull(entity.Media.ToList()[0].Sizes.Large);
            Assert.IsNotNull(entity.Media.ToList()[0].Sizes.Medium);
            Assert.IsNotNull(entity.Media.ToList()[0].Sizes.Small);
            Assert.IsNotNull(entity.Media.ToList()[0].Sizes.Thumb);
            entity.Media.ToList()[0].Sizes.Large.h.ShouldBeEquivalentTo(466);
            entity.Media.ToList()[0].Sizes.Large.w.ShouldBeEquivalentTo(700);
            entity.Media.ToList()[0].Sizes.Large.resize.ShouldBeEquivalentTo("fit");
            entity.Media.ToList()[0].Type.ShouldBeEquivalentTo("photo");
            entity.Media.ToList()[0].indices.Count().ShouldBeEquivalentTo(2);

            // do indirect check of string for media
            var zeroOffsetStart = entity.Media.ToList()[0].indices.ToList()[0];
            Assert.IsInstanceOfType(zeroOffsetStart, typeof(Int32));
            var zeroOffsetEnd = entity.Media.ToList()[0].indices.ToList()[1];
            Assert.IsInstanceOfType(zeroOffsetEnd, typeof(Int32));
            Assert.IsTrue(QuickSubStringHelper(text, "http://t.co/qbJx26r", zeroOffsetStart, (zeroOffsetEnd - zeroOffsetStart)));
        }

        [TestMethod]
        public void Decode_Tweet_Deep_Tests()
        {
            var fileName = ".\\data\\tweets\\singletweet.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);

            Assert.IsNotNull(tweet);
            Assert.IsInstanceOfType(tweet, typeof (BoxKite.Twitter.Models.Tweet));

            if (tweet.RetweetCount != null)
                tweet.RetweetCount.ShouldBeEquivalentTo(0);
            tweet.Id.ShouldBeEquivalentTo(243145735212777472);
            Assert.IsNull(tweet.InReplyToId);
            Assert.IsNull(tweet.InReplyToUserId);
            Assert.IsNull(tweet.Location);
            Assert.IsNull(tweet.Place);
            tweet.Favourited.ShouldBeEquivalentTo(false);
            tweet.Source.ShouldBeEquivalentTo(
                "<a href=\"http://jason-costa.blogspot.com\" rel=\"nofollow\">My Shiny App</a>");
            tweet.Text.ShouldBeEquivalentTo("Maybe he'll finally find his keys. #peterfalk");
            tweet.Time.ToString().ShouldBeEquivalentTo("5/09/2012 12:37:15 AM +00:00");

            //check user details
            Assert.IsNotNull(tweet.User);
            Assert.IsInstanceOfType(tweet.User, typeof (BoxKite.Twitter.Models.User));
            tweet.User.Name.ShouldBeEquivalentTo("Jason Costa");
            tweet.User.Followers.ShouldBeEquivalentTo(8760);
            tweet.User.ScreenName.ShouldBeEquivalentTo("jasoncosta");
            tweet.User.IsFollowedByMe.ShouldBeEquivalentTo(false);

            //check entities
            Assert.IsNotNull(tweet.Entities);
            Assert.IsInstanceOfType(tweet.Entities, typeof (BoxKite.Twitter.Models.Entities));
            if (tweet.Entities.Hashtags != null)
            {
                Assert.IsInstanceOfType(tweet.Entities.Hashtags.ToList()[0], typeof (BoxKite.Twitter.Models.Hashtag));
            }
            if (tweet.Entities.Media != null)
                Assert.IsInstanceOfType(tweet.Entities.Media.ToList()[0], typeof (BoxKite.Twitter.Models.Media));
            if (tweet.Entities.Mentions != null)
            {
                if (tweet.Entities.Mentions.Any())
                    Assert.IsInstanceOfType(tweet.Entities.Mentions.ToList()[0], typeof (BoxKite.Twitter.Models.Mention));
            }
            if (tweet.Entities.Urls != null)
            {
                if (tweet.Entities.Urls.Any())
                    Assert.IsInstanceOfType(tweet.Entities.Urls.ToList()[0], typeof (BoxKite.Twitter.Models.Url));
            }
        }

        [TestMethod]
        public void Decode_User_Deep_Tests()
        {
            var fileName = ".\\data\\users\\show.txt";
            var json = File.ReadAllText(fileName);
            var twitteruser = JsonConvert.DeserializeObject<User>(json);

            Assert.IsNotNull(twitteruser);
            Assert.IsInstanceOfType(twitteruser, typeof(BoxKite.Twitter.Models.User));
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
            Assert.IsNull(twitteruser.Url);
        }


        private bool QuickSubStringHelper(string haystack, string needle, int zeroOffsetStart, int length)
        {
            var found = haystack.Substring(zeroOffsetStart, length);
            return (needle == found);
        }
    }
}
