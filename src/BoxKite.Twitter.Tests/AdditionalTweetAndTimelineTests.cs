// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.IO;
using System.Linq;
using BoxKite.Twitter.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace BoxKite.Twitter.Tests
{
    public class AdditionalTweetAndTimelineTests
    {
        [Fact]
        public void Parse_Tweet_with_encodings()
        {
            var fileName = ".\\data\\sampletweet-withencoding.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);

            tweet.Should().NotBeNull();
            tweet.Text.ShouldBeEquivalentTo("My take: design & development are facets of the same process. Design leads development, development informs design. #fowd");
        }

        [Fact]
        public void Parse_Tweet_with_encodings_part_deux()
        {
            var fileName = ".\\data\\sampletweet-withencoding-2.txt";
            var json = File.ReadAllText(fileName);
            var tweet = JsonConvert.DeserializeObject<Tweet>(json);

            tweet.Should().NotBeNull();
            tweet.Text.ShouldBeEquivalentTo("School -> Research Assignment -> Uni -> Assignment -> Library -> Marking. #fml");
        }

        [Fact]
        public void Parse_Twitter_timeline_with_over_200_tweets()
        {
            var fileName = ".\\data\\timeline.txt";
            var json = File.ReadAllText(fileName);
            var _tweets = JsonConvert.DeserializeObject<IEnumerable<Tweet>>(json);
            var tweets = _tweets.ToList();

            tweets.Should().NotBeNull();
            tweets.Count().Should().BeGreaterOrEqualTo(0);
        }
    }
}
