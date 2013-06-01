// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Twitter.Tests
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
                                                  Assert.IsInstanceOfType(f.ToList()[0], typeof (long));
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
        public async Task UserStream3_event_follow()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream3follow.txt"));
            var userstreamtest3 = session.GetUserStream();

            userstreamtest3.Events.Subscribe(e =>
            {
                Assert.IsNotNull(e);
                Assert.IsInstanceOfType(e.TargetUser, typeof(User));
                Assert.IsInstanceOfType(e.SourceUser, typeof(User));
                e.EventName.ShouldBeEquivalentTo("follow");
            }
            );

            userstreamtest3.Start();

            while (userstreamtest3.IsActive)
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
        public async Task UserStream5_deleteevent()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream5deleteevent.txt"));
            var userstreamtest5 = session.GetUserStream();
            
            userstreamtest5.DeleteEvents.Subscribe(de =>
            {
                Assert.IsNotNull(de);
                Assert.IsNotNull(de.DeleteEventStatus);
                de.DeleteEventStatus.Id.ShouldBeEquivalentTo(1234);
                de.DeleteEventStatus.UserId.ShouldBeEquivalentTo(3);
            }
            );

            userstreamtest5.Start();

            while (userstreamtest5.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }


        [TestMethod]
        public async Task UserStream6_dm()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream6dm.txt"));
            var userstreamtest6 = session.GetUserStream();

            userstreamtest6.DirectMessages.Subscribe(dm =>
            {
                Assert.IsNotNull(dm);
                Assert.IsNotNull(dm.Text);
                dm.Text.ShouldBeEquivalentTo("testiung");
                Assert.IsInstanceOfType(dm.Recipient, typeof(User));
                Assert.IsInstanceOfType(dm.Sender, typeof(User));
                dm.Recipient.ScreenName.ShouldBeEquivalentTo("NickHodgeMSFT");
                dm.Sender.ScreenName.ShouldBeEquivalentTo("RealNickHodge");
            }
            );

            userstreamtest6.Start();

            while (userstreamtest6.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }

        [TestMethod]
        public async Task UserStream7_scrubgeo()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream7scrubgeo.txt"));
            var userstreamtest7 = session.GetUserStream();

            userstreamtest7.ScrubGeoRequests.Subscribe(sg =>
            {
                Assert.IsNotNull(sg);
                sg.UpToStatusId.ShouldBeEquivalentTo(23260136625);
            }
            );

            userstreamtest7.Start();

            while (userstreamtest7.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }

        [TestMethod]
        public async Task UserStream8_scrubgeo()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream8limitnotice.txt"));
            var userstreamtest8 = session.GetUserStream();

            userstreamtest8.LimitNotices.Subscribe(ln =>
            {
                Assert.IsNotNull(ln);
                ln.Track.ShouldBeEquivalentTo(1234);
            }
            );

            userstreamtest8.Start();

            while (userstreamtest8.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }


        [TestMethod]
        public async Task UserStream9_statuswithheld()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream9status_withheld.txt"));
            var userstreamtest9 = session.GetUserStream();

            userstreamtest9.StatusWithheld.Subscribe(sw =>
            {
                Assert.IsNotNull(sw);
                sw.Id.ShouldBeEquivalentTo(1234567890);
                sw.UserId.ShouldBeEquivalentTo(123456);
                sw.WithheldInCountries.Count().ShouldBeEquivalentTo(2);
            }
            );

            userstreamtest9.Start();

            while (userstreamtest9.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }

        [TestMethod]
        public async Task UserStream10_status_mention()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream10mention.txt"));
            var userstreamtest10 = session.GetUserStream();

            userstreamtest10.Tweets.Subscribe(tw =>
            {
                Assert.IsNotNull(tw);
                tw.Text.ShouldBeEquivalentTo("@nickhodgemsft testing, please ignore");
                Assert.IsInstanceOfType(tw.User, typeof(User));
            }
            );

            userstreamtest10.Start();

            while (userstreamtest10.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }

        [TestMethod]
        public async Task UserStream11_statuswithheld()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream11userwithheld.txt"));
            var userstreamtest11 = session.GetUserStream();

            userstreamtest11.StatusWithheld.Subscribe(uw =>
            {
                Assert.IsNotNull(uw);
                uw.UserId.ShouldBeEquivalentTo(123456);
                uw.WithheldInCountries.Count().ShouldBeEquivalentTo(2);
            }
            );

            userstreamtest11.Start();

            while (userstreamtest11.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }


        [TestMethod]
        public async Task UserStream14_event_list_removeuser()
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

        [TestMethod]
        public async Task UserStream16_scrubgeo()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream16scrubgeo.txt"));
            var userstreamtest16 = session.GetUserStream();

            userstreamtest16.ScrubGeoRequests.Subscribe(sg =>
            {
                Assert.IsNotNull(sg);
                sg.UserId.ShouldBeEquivalentTo(14090452);
                sg.UpToStatusId.ShouldBeEquivalentTo(23260136625);
            }
            );

            userstreamtest16.Start();

            while (userstreamtest16.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }

        [TestMethod]
        public async Task UserStream17_limitnotices()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream17limit.txt"));
            var userstreamtest17 = session.GetUserStream();

            userstreamtest17.LimitNotices.Subscribe(ln =>
            {
                Assert.IsNotNull(ln);
                ln.Track.ShouldBeEquivalentTo(1234);
            }
            );

            userstreamtest17.Start();

            while (userstreamtest17.IsActive)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }
    }
}
