#BoxKite.Twitter

![boxkite logo](http://media.nickhodge.com/boxkite/twitter/github-logo-wide-2.png)

BoxKite.Twitter is a netstandard 1.4 library that provides an interface to the Twitter API 1.1, licensed [MS-PL](http://opensource.org/licenses/MS-PL).

Supporting all platforms that compatible with netstandard 1.4; it uses modern .NET development mechanisms `async/await` and [Reactive Extensions](http://reactivex.io/).

Questions? You can find me on Twitter, of course! [@RealNickHodge](https://twitter.com/RealNickHodge)

@ghuntley [has a great BoxKite.Twitter examples here](https://github.com/ghuntley/BoxKite.Twitter.Examples)


## Version News
Version 3.0.0 Changes:
* Converted to netstandard library. BoxKite supports all platforms that compatible with netstandard 1.4.
* Min version of the Reactive Extensions is now v3.1.0 which is netstandard compatible.
* When contributing or compiling from source, the minimum version of Visual Studio is now 2017.
* Android namepace is now `BoxKite.Twitter` was `BoxKite.Twitter.Android`

Version 2.4.0 Changes:
* Add support for [>140 character Direct Message sending](https://twittercommunity.com/t/removing-the-140-character-limit-from-direct-messages/41348). BoxKite.Twitter does not truncate messages, but the Direct Message API required passing a parameter indicating support for >140 characters. This is now implemented.

Version 2.3.0 Changes:
* Change Desktop .NET to 4.6
* Remove Windows 8/Windows Phone 8 targets (please use Portable or older versions of BoxKite.Twitter)
* Addition of Universal Windows Platform supporting both Windows 10 and Windows Mobile 10

Version 2.2.x Changes:
* Thanks to @vikingcode, new extended_entities are added to the Twitter object. This is in preparation for [Twitter's addition of video to their supported media types](https://twittercommunity.com/t/twitter-video-support-in-rest-and-streaming-api/31258) As this is yet to be widely used, expect some fixes!
* Added various other edge/new APIs (GetConfiguration) with appropriate tests

Version 2.1 Changes:
* Many areas of code cleanup, with some property/model name changes
* If SearchStream/UserStream disconnects; will try a reconnect (automatically)

Version 2.0.x-pre Implements/Changes:
* Both Twitter's [Application Authentication](https://dev.twitter.com/docs/auth/application-only-auth) and [User Authentication](https://dev.twitter.com/docs/auth/obtaining-access-tokens) supported.
* REST APIs mapped to Application/User Authentication as based on Twitter documentation
* iOS and Android build (via Mono/Xamarin)
* `TwitterConnection` method naming changes (verb/noun, more sensible)
* a couple of new Twitter REST API endpoints added eg: [status/retweeters](https://dev.twitter.com/docs/api/1.1/get/statuses/retweeters/ids)
* Removal of Json `<dynamic>` use (had only been used in three places in 1.x series)
* Based on [Paul Bett's recommendation to remove Message Buses from reactive style apps](http://log.paulbetts.org/messagebus-and-why-you-shouldnt-use-it/), I surgically removed Brendan's Reactive.EventAggregator. Sorry, dude
* .NET API naming conventions (I strongly recommend you watch [Designing Wonderful .NET APIs](http://vimeo.com/97501377) from NDC 2014)
* various small bugs crushed

Plans for 2.3 Release
* cleanup with `<Func>` work in Backfills
* better state machine for connect/disconnect eg: in mobile environments
* change to BoxKiteTwitter exception handling from "inline" (what a stupid idea that was) errors

Version 1.5.x Implements:
* start of [Application-only Authentication via OAuth2](https://dev.twitter.com/docs/auth/application-only-auth)
* this is going to require some rework in the API to permit App-level API access (pull user timelines, friends of account, list resources, search in tweets etc) without a user login.

Version 1.4.x Implements:
* Getting/Setting Profile Banners (added to twitter API 21st April 2014)
```csharp
await session.GetProfileBanners(screen_name:"realnickhodge");
await session.ChangeProfileBanner("boxkite1500x500", fs);
```
* User now contains Profile Banner URL, if turned on by the user (thanks Paul!)

Version 1.3.x Fixes/Implements:
* Universal App support
* Roll in Reactive.EventAggregator into BoxKite.Twitter itself because @shiftkey

Version 1.2.x Fixes/Implements:
* As per Twitter [List IDs moving to 64 bits in early 2014](https://blog.twitter.com/2013/list-ids-become-64-bit-integers-early-2014)
* TwitterConnection is the central "entry point" to BoxKite.Twitter
* VS2013 build
* IStreamEvent work to abstract messages from the UserStream
* various other fixes

1.0.6 (and 1.0.5) Fixes/Implements:
* Adding support for latest Portable HTTPClient
* Adding more Rx in many places to make life easier
* Fixed bug where gzip compression on the HTTP Stream (for userstreams) didn't flush tweets from the server-side.
* various small bugs found in tweet backfills, found when testing with a fresh account

1.0.4 Fixes/Implements:
* As per Twitter: [User IDs moving to 64 bits later in 2013](https://dev.twitter.com/blog/test-accounts-user-ids-greater-32-bits) 
* Note: Using .NET long to hold user_id
* Note: this impacts any downstream client expecting plain old 32 bit ints
* FriendlyDateTimeString now says "Just Now" rather than "in 13 seconds" when user's device clock has drifted forward from correct internet time.

## Twitter API Coverage

At the present time, BoxKite.Twitter supports API version 1.1 of the following:

* [Twitter 1.1 REST API](https://dev.twitter.com/docs/api/1.1)
* [User and Application authentication](https://dev.twitter.com/docs/auth/application-only-auth)
* [User streams](https://dev.twitter.com/docs/streaming-apis/streams/user)
* [Search streams](https://dev.twitter.com/docs/api/1.1/post/statuses/filter)

##NuGet Package

[BoxKite.Twitter listing on NuGet](https://nuget.org/packages/BoxKite.Twitter)

```
Install-Package BoxKite.Twitter
```

##So What Does the Code look like?

[Pop into the wiki](https://github.com/nickhodge/BoxKite.Twitter/wiki) to see code examples


## Wiki documentation

[Documentation is online and provided in the Wiki](https://github.com/nickhodge/BoxKite.Twitter/wiki)

## Extra Info and Documentation

* More indepth [developer details are documented here](https://github.com/nickhodge/BoxKite.Twitter/wiki/devdetails)
* There are a few, [small missing pieces documented here](https://github.com/nickhodge/BoxKite.Twitter/wiki/todos)
* 53 ["Live Fire" tests are explained here](https://github.com/nickhodge/BoxKite.Twitter/wiki/livefire)
* [Sample, Simple WPF/XAML 4.5 Client App](https://github.com/nickhodge/BoxKite.Twitter/wiki/samplewpf)
* Note: Sample apps in various stages of incompleteness; will be in a separate repo

##To Build:
* Visual Studio 2015 Update 1 with latest NuGet supporting PCLs (Portable Class Libraries) and Universal Windows Platform
* Solution is Visual Studio 2015 Update 1, but code should work and build with previous versions of Visual Studio
* Visual Studio 2015 Community Versions should also be OK (but I haven't tested this)
* Uses Xamarin Studio 5.9.4 (or later) for Android and iOS builds. Disable these projects if you don't have Xamarin installed

##Where you can help out
* Modeling the API to make sense for more developers than me
* Additional harness for Reactive hook up
* Stronger "this works on non-Microsoft .NET platforms" live testing and experience
* Always more Documentation, of course

##API Coverage, With Tests & Intellisense Comments
[From Twitter REST API v1.1](https://dev.twitter.com/docs/api/1.1)

* Timelines
* Tweets 
* [status/update_with_media](https://dev.twitter.com/docs/api/1.1/post/statuses/update_with_media)
*  Search
*  Saved Searches
*  User Streaming: [GET userstreams](https://dev.twitter.com/docs/streaming-apis/streams/user) 
*  Search via Streaming [POST status/filter](https://dev.twitter.com/docs/api/1.1/post/statuses/filter)
*  Direct Messages
*  Friends and Followers
*  Users
*  Suggested Users
*  Favorites
*  Lists 
*  Places and Geo
*  Trends
*  Spam Reporting
*  oauth
*  API rate limits

##Tests:
* 124 test methods with average of 4 assertion tests each
* Both Twitter API 1.1 and Userstream Unit Tests
* 53 "Live Fire Tests" (Live integration tests) Included as a separate project, it is engaged via the Console; uses a real Twitter Account to exercise the API. Configurable Test series, individual tests that can be run.
* Test source is JSON from [dev.twitter.com](https://dev.twitter.com/) and where innaccurate on the site, taken from live data or corrected to match live data.
* Error response type testing (eg: 429 rate_limits) & generic decode tests included


## Dependencies

* Without [Newtonsoft Json.NET](http://json.net) this library would not exist (MIT License)
* Microsoft's Open sourced .NET [Reactive Extensions](https://rx.codeplex.com/) (Apache License 2.0)
* [Portable version of the HTTP Client Libraries, 2.2.x](http://blogs.msdn.com/b/bclteam/p/httpclient.aspx) from the Microsoft ASP.NET WebAPI team [HTTP Client Libraries](http://nuget.org/packages/Microsoft.Net.Http/)  (sourced from NuGet, currently Licensed under [DotNetBetaUnsupported](http://go.microsoft.com/fwlink/?LinkID=279007) license)

### (Dependency when running BoxKite.Twitter.Tests only)

* Great little collection of extension methods that make writing UnitTests easier [FluentAssertions](http://fluentassertions.codeplex.com/) (MS-PL) 


##Copyright, License Information

[BoxKite.Twitter](https://github.com/shiftkey/BoxKite.Twitter) was started by Brendan Forster ([https://github.com/shiftkey/](https://github.com/shiftkey/)) as a part of a larger Twitter project now on permanent hiatus.

BoxKite.Twitter is Licensed under: 
[MS-PL](http://opensource.org/licenses/MS-PL)

Copyright: 
[Nick Hodge](https://github.com/nickhodge/) and [Brendan Forster](https://github.com/shiftkey/) 2012-2016

BoxKite Logo is Copyright 2012 [Nick Hodge](https://github.com/nickhodge/)

## Why BoxKite?

Everyone likes flying kites, right? The original inventor of the [box kite](http://en.wikipedia.org/wiki/Box_kite) was [Lawrence Hargrave](http://en.wikipedia.org/wiki/Lawrence_Hargrave). In 1893. And he was an Australian. Sadly missing from our newer plastic yet colourful currency, Lawrence was one of those guys who helped manned flight become a reality.

And keeping with the Twitter bird theme, flying and airborne I created a fork of the MahTweets twitter client in late 2011 and named it [MahTweets Lawrence Hargrave](https://github.com/nickhodge/MahTweets.LawrenceHargrave).

In mid 2012, Brendan needed a code name for a new Twitter "thing" he was working on. Keeping in this flighty theme, I named his app "BoxKite". For posterity's sake, I have absconded with Brendan's code, and my contributed codename (and a nifty icon) and have dubbed this project BoxKite.

