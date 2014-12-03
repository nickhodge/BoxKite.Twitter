// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Linq;
using System.Reactive.Linq;
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
        private readonly TestableUserSession session = new TestableUserSession();

        [TestMethod]        
        public async Task UserStream1_initialFriends()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream1initwithfriends.txt"));
            var userstreamtest1 = session.GetUserStream();
            var IsActive = true;

            userstreamtest1.Friends.Subscribe(f =>
                                              {
                                                  Assert.IsNotNull(f);
                                                  Assert.IsInstanceOfType(f.ToList()[0], typeof (long));
                                                  IsActive = false;
                                              }
                );

            userstreamtest1.Start();

            userstreamtest1.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });

            while (IsActive)
            {
                
            }
 
        }


        [TestMethod]
        public async Task UserStream1_initial_Tweets()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream1initwithfriends.txt"));
            var userstreamtest1 = session.GetUserStream();
            var IsActive = true;

            userstreamtest1.Tweets.Subscribe(t =>
            {
                Assert.IsNotNull(t);
                t.Text.Should().NotBeNullOrEmpty();
                Assert.IsInstanceOfType(t.User, typeof(User));
                t.User.Should().NotBeNull();
                t.User.ScreenName.Should().NotBeNullOrEmpty();
                IsActive = false;
            }
            );

            userstreamtest1.Start();

            userstreamtest1.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });

            while (IsActive)
            {

            }

        }

        [TestMethod]
        public async Task UserStream2_mentionofself()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream2self.txt"));
            var userstreamtest2 = session.GetUserStream();
            var IsActive = true;

            userstreamtest2.Tweets.Subscribe(t =>
                                             {
                                                 Assert.IsNotNull(t);
                                                 t.Text.Should().NotBeNullOrEmpty();
                                                 t.User.Should().NotBeNull();
                                                 t.User.ScreenName.Should().NotBeNullOrEmpty();
                                                 IsActive = false;
                                             }
                );

            userstreamtest2.Start();

            userstreamtest2.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [TestMethod]
        public async Task UserStream3_event_follow()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream3follow.txt"));
            var userstreamtest3 = session.GetUserStream();
            var IsActive = true;

            userstreamtest3.Events.Subscribe(e =>
            {
                Assert.IsNotNull(e);
                Assert.IsInstanceOfType(e.TargetUser, typeof(User));
                Assert.IsInstanceOfType(e.SourceUser, typeof(User));
                e.EventName.ShouldBeEquivalentTo("follow");
                IsActive = false;
            }
            );

            userstreamtest3.Start();

            userstreamtest3.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }


        [TestMethod]
        public async Task UserStream4_event_list_adduser()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream4addtolistevent.txt"));
            var userstreamtest4 = session.GetUserStream();
            var IsActive = true;

            userstreamtest4.Events.Subscribe(e =>
            {
                Assert.IsNotNull(e);
                Assert.IsInstanceOfType(e.TargetUser, typeof(User));
                Assert.IsInstanceOfType(e.SourceUser, typeof(User));
                e.EventName.ShouldBeEquivalentTo("list_member_added");
                IsActive = false;
            }
            );

            userstreamtest4.Start();

            userstreamtest4.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [TestMethod]
        public async Task UserStream5_deleteevent()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream5deleteevent.txt"));
            var userstreamtest5 = session.GetUserStream();
            var IsActive = true;
            
            userstreamtest5.DeleteEvents.Subscribe(de =>
            {
                Assert.IsNotNull(de);
                Assert.IsNotNull(de.DeleteEventStatus);
                de.DeleteEventStatus.Id.ShouldBeEquivalentTo(1234);
                de.DeleteEventStatus.UserId.ShouldBeEquivalentTo(3);
                IsActive = false;
            }
            );

            userstreamtest5.Start();

            userstreamtest5.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }


        [TestMethod]
        public async Task UserStream6_dm()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream6dm.txt"));
            var userstreamtest6 = session.GetUserStream();
            var IsActive = true;

            userstreamtest6.DirectMessages.Subscribe(dm =>
            {
                Assert.IsNotNull(dm);
                Assert.IsNotNull(dm.Text);
                dm.Text.ShouldBeEquivalentTo("testiung");
                Assert.IsInstanceOfType(dm.Recipient, typeof(User));
                Assert.IsInstanceOfType(dm.Sender, typeof(User));
                dm.Recipient.ScreenName.ShouldBeEquivalentTo("NickHodgeMSFT");
                dm.Sender.ScreenName.ShouldBeEquivalentTo("RealNickHodge");
                IsActive = false;
            }
            );

            userstreamtest6.Start();

            userstreamtest6.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [TestMethod]
        public async Task UserStream7_scrubgeo()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream7scrubgeo.txt"));
            var userstreamtest7 = session.GetUserStream();
            var IsActive = true;

            userstreamtest7.ScrubGeoRequests.Subscribe(sg =>
            {
                Assert.IsNotNull(sg);
                sg.UpToStatusId.ShouldBeEquivalentTo(23260136625);
                IsActive = false;
            }
            );

            userstreamtest7.Start();

            userstreamtest7.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [TestMethod]
        public async Task UserStream8_scrubgeo()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream8limitnotice.txt"));
            var userstreamtest8 = session.GetUserStream();
            var IsActive = true;

            userstreamtest8.LimitNotices.Subscribe(ln =>
            {
                Assert.IsNotNull(ln);
                ln.Track.ShouldBeEquivalentTo(1234);
                IsActive = false;
            }
            );

            userstreamtest8.Start();

            userstreamtest8.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }


        [TestMethod]
        public async Task UserStream9_statuswithheld()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream9status_withheld.txt"));
            var userstreamtest9 = session.GetUserStream();
            var IsActive = true;

            userstreamtest9.StatusWithheld.Subscribe(sw =>
            {
                Assert.IsNotNull(sw);
                sw.Id.ShouldBeEquivalentTo(1234567890);
                sw.UserId.ShouldBeEquivalentTo(123456);
                sw.WithheldInCountries.Count().ShouldBeEquivalentTo(2);
                IsActive = false;
            }
            );

            userstreamtest9.Start();

            userstreamtest9.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [TestMethod]
        public async Task UserStream10_status_mention()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream10mention.txt"));
            var userstreamtest10 = session.GetUserStream();
            var IsActive = true;

            userstreamtest10.Tweets.Subscribe(tw =>
            {
                Assert.IsNotNull(tw);
                tw.Text.ShouldBeEquivalentTo("@nickhodgemsft testing, please ignore");
                Assert.IsInstanceOfType(tw.User, typeof(User));
                IsActive = false;
            }
            );

            userstreamtest10.Start();

            userstreamtest10.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [TestMethod]
        public async Task UserStream11_statuswithheld()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream11userwithheld.txt"));
            var userstreamtest11 = session.GetUserStream();
            var IsActive = true;

            userstreamtest11.StatusWithheld.Subscribe(uw =>
            {
                Assert.IsNotNull(uw);
                uw.UserId.ShouldBeEquivalentTo(123456);
                uw.WithheldInCountries.Count().ShouldBeEquivalentTo(2);
                IsActive = false;
            }
            );

            userstreamtest11.Start();

            userstreamtest11.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }


        [TestMethod]
        public async Task UserStream14_event_list_removeuser()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream14unlisted.txt"));
            var userstreamtest14 = session.GetUserStream();
            var IsActive = true;

            userstreamtest14.Events.Subscribe(e =>
            {
                Assert.IsNotNull(e);
                Assert.IsInstanceOfType(e.TargetUser, typeof(User));
                Assert.IsInstanceOfType(e.SourceUser, typeof(User));
                e.EventName.ShouldBeEquivalentTo("list_member_removed");
                IsActive = false;
            }
            );

            userstreamtest14.Start();

            userstreamtest14.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [TestMethod]
        public async Task UserStream16_scrubgeo()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream16scrubgeo.txt"));
            var userstreamtest16 = session.GetUserStream();
            var IsActive = true;

            userstreamtest16.ScrubGeoRequests.Subscribe(sg =>
            {
                Assert.IsNotNull(sg);
                sg.UserId.ShouldBeEquivalentTo(14090452);
                sg.UpToStatusId.ShouldBeEquivalentTo(23260136625);
                IsActive = false;
            }
            );

            userstreamtest16.Start();

            userstreamtest16.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [TestMethod]
        public async Task UserStream17_limitnotices()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream17limit.txt"));
            var userstreamtest17 = session.GetUserStream();
            var IsActive = true;

            userstreamtest17.LimitNotices.Subscribe(ln =>
            {
                Assert.IsNotNull(ln);
                ln.Track.ShouldBeEquivalentTo(1234);
                IsActive = false;
            }
            );

            userstreamtest17.Start();

            userstreamtest17.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }



        [TestMethod]
        public async Task UserStream18_favorite()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream18favorite.txt"));
            var userstreamtest18 = session.GetUserStream();
            var IsActive = true;

            userstreamtest18.Events.Subscribe(ev =>
            {
                Assert.IsNotNull(ev);
                ev.EventName.ShouldBeEquivalentTo("favorite");
                Assert.IsNotNull(ev.SourceUser);
                Assert.IsNotNull(ev.TargetUser);
                var twev = (TweetStreamEvent) ev;
                Assert.IsNotNull(twev);
                Assert.IsNotNull(twev.tweet);
                twev.tweet.Id.ShouldBeEquivalentTo(427615500516880384);
                IsActive = false;
            }
                );

            userstreamtest18.Start();

            userstreamtest18.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }


        [TestMethod]
        public async Task UserStream19_unfavorite()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream19unfavorite.txt"));
            var userstreamtest19 = session.GetUserStream();
            var IsActive = true;

            userstreamtest19.Events.Subscribe(ev =>
            {
                Assert.IsNotNull(ev);
                ev.EventName.ShouldBeEquivalentTo("unfavorite");
                Assert.IsNotNull(ev.SourceUser);
                Assert.IsNotNull(ev.TargetUser);
                var twev = (TweetStreamEvent)ev;
                Assert.IsNotNull(twev);
                Assert.IsNotNull(twev.tweet);
                twev.tweet.Id.ShouldBeEquivalentTo(427615500516880384);
                IsActive = false;
            }
                );

            userstreamtest19.Start();

            userstreamtest19.UserStreamActive.Where(status => status.Equals(false)).Subscribe(t => { IsActive = false; });

            while (IsActive)
            {
            }
        }


    }
}
