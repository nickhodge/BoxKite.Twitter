using System;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Tests.Modules;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests.UserStream
{
    [TestClass]
    public class UserStreamTests : ReactiveTest
    {
        private readonly TestableSession session = new TestableSession();
  
        [TestMethod]
        public async Task UserStream1_initialFriends_And_Tweets()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream1initwithfriends.txt"));
            var userstreamtest1 = session.GetUserStream();

            userstreamtest1.Tweets.Subscribe(t =>
                                             {
                                                 Assert.IsNotNull(t);
                                                 t.Text.Should().NotBeNullOrEmpty();
                                                 Assert.IsInstanceOfType(t.User,typeof(User));
                                                 t.User.Should().NotBeNull();
                                                 t.User.ScreenName.Should().NotBeNullOrEmpty();
                                             }
            );

            userstreamtest1.Friends.Subscribe(f =>
                                              {
                                                  Assert.IsNotNull(f);
                                                  Assert.IsInstanceOfType(f, typeof (long));
                                              }
                );

            userstreamtest1.Start();

            while (userstreamtest1.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }

        [TestMethod]
        public async Task UserStream2_mentionofself()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream2self.txt"));
            var userstreamtest2 = session.GetUserStream();

            userstreamtest2.Tweets.Subscribe(t =>
                                             {
                                                 Assert.IsNotNull(t);
                                                 t.Text.Should().NotBeNullOrEmpty();
                                                 t.User.Should().NotBeNull();
                                                 t.User.ScreenName.Should().NotBeNullOrEmpty();
                                             }
                );

            userstreamtest2.Start();

            while (userstreamtest2.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }

        [TestMethod]
        public async Task UserStream4_event_list_adduser()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream4addtolistevent.txt"));
            var userstreamtest4 = session.GetUserStream();

            userstreamtest4.Events.Subscribe(e =>
            {
                Assert.IsNotNull(e);
                Assert.IsInstanceOfType(e.TargetUser, typeof(User));
                Assert.IsInstanceOfType(e.SourceUser, typeof(User));
                e.EventName.ShouldBeEquivalentTo("list_member_added");
            }
            );

            userstreamtest4.Start();

            while (userstreamtest4.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }           
        }

        [TestMethod]
        public async Task UserStream4_event_list_removeuser()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream14unlisted.txt"));
            var userstreamtest14 = session.GetUserStream();

            userstreamtest14.Events.Subscribe(e =>
            {
                Assert.IsNotNull(e);
                Assert.IsInstanceOfType(e.TargetUser, typeof(User));
                Assert.IsInstanceOfType(e.SourceUser, typeof(User));
                e.EventName.ShouldBeEquivalentTo("list_member_removed");
            }
            );

            userstreamtest14.Start();

            while (userstreamtest14.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }

    }
}
