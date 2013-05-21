using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Tests.Modules;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests.UserStream
{
    [TestClass]
    public class UserStreamTest : ReactiveTest
    {
        private readonly TestableSession session = new TestableSession();
  
        [TestMethod]
        public async Task Userstream_initialtweets()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream1.txt"));
            var userstreamtest1 = session.GetUserStream();

            userstreamtest1.Tweets.Subscribe(t =>
                                             {
                                                 Assert.IsNotNull(t);
                                                 t.Text.Should().NotBeNullOrEmpty();
                                                 t.User.Should().NotBeNull();
                                                 t.User.ScreenName.Should().NotBeNullOrEmpty();
                                             }
            );

            userstreamtest1.Start();

            while (userstreamtest1.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }
    }
}
