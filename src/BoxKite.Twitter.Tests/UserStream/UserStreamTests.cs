// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
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
using Xunit;

namespace BoxKite.Twitter.Tests
{
    
    public class UserStreamTests : ReactiveTest
    {
        private readonly TestableUserSession session = new TestableUserSession();

        [Fact]        
        public async Task UserStream1_initialFriends()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream1initwithfriends.txt"));
            var userstreamtest1 = session.GetUserStream();
            var IsActive = true;

            userstreamtest1.Friends.Subscribe(f =>
                                              {
                                                  f.Should().NotBeNull();
                                                  IsActive = false;
                                              }
                );

            userstreamtest1.Start();

            userstreamtest1.StreamActive.Subscribe(t => { IsActive = false; });

            while (IsActive)
            {
                
            }
 
        }


        [Fact]
        public async Task UserStream1_initial_Tweets()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream1initwithfriends.txt"));
            var userstreamtest1 = session.GetUserStream();
            var IsActive = true;

            userstreamtest1.Tweets.Subscribe(t =>
                {
                t.Should().NotBeNull();
                t.Text.Should().NotBeNullOrEmpty();
                t.User.Should().NotBeNull();
                t.User.ScreenName.Should().NotBeNullOrEmpty();
                IsActive = false;
            }
            );

            userstreamtest1.Start();

            userstreamtest1.StreamActive.Subscribe(t => { IsActive = false; });

            while (IsActive)
            {

            }

        }

        [Fact]
        public async Task UserStream2_mentionofself()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream2self.txt"));
            var userstreamtest2 = session.GetUserStream();
            var IsActive = true;

            userstreamtest2.Tweets.Subscribe(t =>
                {
                    t.Should().NotBeNull();
                    t.Text.Should().NotBeNullOrEmpty();
                    t.User.Should().NotBeNull();
                    t.User.ScreenName.Should().NotBeNullOrEmpty();
                    IsActive = false;
                });

            userstreamtest2.Start();

            userstreamtest2.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [Fact]
        public async Task UserStream3_event_follow()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream3follow.txt"));
            var userstreamtest3 = session.GetUserStream();
            var IsActive = true;

            userstreamtest3.Events.Subscribe(e =>
                {
                    e.Should().NotBeNull();
                    e.EventName.ShouldBeEquivalentTo("follow");
                    IsActive = false;
                });

            userstreamtest3.Start();

            userstreamtest3.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }


        [Fact]
        public async Task UserStream4_event_list_adduser()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream4addtolistevent.txt"));
            var userstreamtest4 = session.GetUserStream();
            var IsActive = true;

            userstreamtest4.Events.Subscribe(e =>
                {
                    e.Should().NotBeNull();
                    e.EventName.ShouldBeEquivalentTo("list_member_added");
                    IsActive = false;
                });

            userstreamtest4.Start();

            userstreamtest4.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [Fact]
        public async Task UserStream5_deleteevent()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream5deleteevent.txt"));
            var userstreamtest5 = session.GetUserStream();
            var IsActive = true;
            
            userstreamtest5.DeleteEvents.Subscribe(de =>
                {
                    de.Should().NotBeNull();
                    de.DeleteEventStatus.Should().NotBeNull();
                    de.DeleteEventStatus.Id.ShouldBeEquivalentTo(1234);
                    de.DeleteEventStatus.UserId.ShouldBeEquivalentTo(3);
                    IsActive = false;
                });

            userstreamtest5.Start();

            userstreamtest5.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }


        [Fact]
        public async Task UserStream6_dm()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream6dm.txt"));
            var userstreamtest6 = session.GetUserStream();
            var IsActive = true;

            userstreamtest6.DirectMessages.Subscribe(dm =>
                {
                    dm.Should().NotBeNull();
                    dm.Text.Should().NotBeNull();
                    dm.Text.ShouldBeEquivalentTo("testiung");
                    dm.Recipient.ScreenName.ShouldBeEquivalentTo("NickHodgeMSFT");
                    dm.Sender.ScreenName.ShouldBeEquivalentTo("RealNickHodge");
                    IsActive = false;
                });

            userstreamtest6.Start();

            userstreamtest6.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [Fact]
        public async Task UserStream7_scrubgeo()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream7scrubgeo.txt"));
            var userstreamtest7 = session.GetUserStream();
            var IsActive = true;

            userstreamtest7.ScrubGeoRequests.Subscribe(sg =>
            {
                sg.Should().NotBeNull();
                sg.UpToStatusId.ShouldBeEquivalentTo(23260136625);
                IsActive = false;
            }
            );

            userstreamtest7.Start();

            userstreamtest7.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [Fact]
        public async Task UserStream8_scrubgeo()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream8limitnotice.txt"));
            var userstreamtest8 = session.GetUserStream();
            var IsActive = true;

            userstreamtest8.LimitNotices.Subscribe(ln =>
            {
                ln.Should().NotBeNull();
                ln.Track.ShouldBeEquivalentTo(1234);
                IsActive = false;
            }
            );

            userstreamtest8.Start();

            userstreamtest8.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }


        [Fact]
        public async Task UserStream9_statuswithheld()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream9status_withheld.txt"));
            var userstreamtest9 = session.GetUserStream();
            var IsActive = true;

            userstreamtest9.StatusWithheld.Subscribe(sw =>
            {
                sw.Should().NotBeNull();
                sw.Id.ShouldBeEquivalentTo(1234567890);
                sw.UserId.ShouldBeEquivalentTo(123456);
                sw.WithheldInCountries.Count().ShouldBeEquivalentTo(2);
                IsActive = false;
            }
            );

            userstreamtest9.Start();

            userstreamtest9.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [Fact]
        public async Task UserStream10_status_mention()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream10mention.txt"));
            var userstreamtest10 = session.GetUserStream();
            var IsActive = true;

            userstreamtest10.Tweets.Subscribe(tw =>
            {
                tw.Should().NotBeNull();
                tw.Text.ShouldBeEquivalentTo("@nickhodgemsft testing, please ignore");
                IsActive = false;
            }
            );

            userstreamtest10.Start();

            userstreamtest10.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [Fact]
        public async Task UserStream11_statuswithheld()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream11userwithheld.txt"));
            var userstreamtest11 = session.GetUserStream();
            var IsActive = true;

            userstreamtest11.StatusWithheld.Subscribe(uw =>
            {
                uw.Should().NotBeNull();
                uw.UserId.ShouldBeEquivalentTo(123456);
                uw.WithheldInCountries.Count().ShouldBeEquivalentTo(2);
            }
            );

            userstreamtest11.Start();

            userstreamtest11.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }


        [Fact]
        public async Task UserStream14_event_list_removeuser()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream14unlisted.txt"));
            var userstreamtest14 = session.GetUserStream();
            var IsActive = true;

            userstreamtest14.Events.Subscribe(e =>
            {
                e.Should().NotBeNull();
                e.EventName.ShouldBeEquivalentTo("list_member_removed");
                IsActive = false;
            }
            );

            userstreamtest14.Start();

            userstreamtest14.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [Fact]
        public async Task UserStream16_scrubgeo()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream16scrubgeo.txt"));
            var userstreamtest16 = session.GetUserStream();
            var IsActive = true;

            userstreamtest16.ScrubGeoRequests.Subscribe(sg =>
            {
                sg.Should().NotBeNull();
                sg.UserId.ShouldBeEquivalentTo(14090452);
                sg.UpToStatusId.ShouldBeEquivalentTo(23260136625);
                IsActive = false;
            }
            );

            userstreamtest16.Start();

            userstreamtest16.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }

        [Fact]
        public async Task UserStream17_limitnotices()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream17limit.txt"));
            var userstreamtest17 = session.GetUserStream();
            var IsActive = true;

            userstreamtest17.LimitNotices.Subscribe(ln =>
            {
                ln.Should().NotBeNull();
                ln.Track.ShouldBeEquivalentTo(1234);
                IsActive = false;
            }
            );

            userstreamtest17.Start();

            userstreamtest17.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }



        [Fact]
        public async Task UserStream18_favorite()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream18favorite.txt"));
            var userstreamtest18 = session.GetUserStream();
            var IsActive = true;

            userstreamtest18.Events.Subscribe(ev =>
            {
                ev.Should().NotBeNull();
                ev.EventName.ShouldBeEquivalentTo("favorite");
                ev.SourceUser.Should().NotBeNull();
                ev.TargetUser.Should().NotBeNull();
                var twev = (TweetStreamEvent) ev;
                twev.Should().NotBeNull();
                twev.Tweet.Should().NotBeNull();
                twev.Tweet.Id.ShouldBeEquivalentTo(427615500516880384);
                IsActive = false;
            }
                );

            userstreamtest18.Start();

            userstreamtest18.StreamActive.Subscribe(t => { IsActive = false; });
            while (IsActive)
            {
                
            }
        }


        [Fact]
        public async Task UserStream19_unfavorite()
        {
            session.Returns(await Json.FromFile("data\\userstream\\userstream19unfavorite.txt"));
            var userstreamtest19 = session.GetUserStream();
            var IsActive = true;

            userstreamtest19.Events.Subscribe(ev =>
            {
                ev.Should().NotBeNull();
                ev.EventName.ShouldBeEquivalentTo("unfavorite");
                ev.SourceUser.Should().NotBeNull();
                ev.TargetUser.Should().NotBeNull();
                var twev = (TweetStreamEvent)ev;
                twev.Should().NotBeNull();
                twev.Tweet.Should().NotBeNull();
                twev.Tweet.Id.ShouldBeEquivalentTo(427615500516880384);
                IsActive = false;
            }
                );

            userstreamtest19.Start();

            userstreamtest19.StreamActive.Subscribe(t => { IsActive = false; });

            while (IsActive)
            {
            }
        }


    }
}
