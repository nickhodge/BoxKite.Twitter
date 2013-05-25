#BoxKite.Twitter

![logo](https://raw.github.com/nickhodge/BoxKite.Twitter/master/artwork/Boxkite-Logo-github.png "Logo")

BoxKite.Twitter is a .NET Portable Class Library that provides an interface to Twitter, licensed MS-PL.

Supporting Windows 8, Windows Phone 8 and .NET 4.5

Questions? You can find me on Twitter, of course! [@NickHodgeMSFT](https://twitter.com/NickHodgeMSFT)

## Twitter API Coverage

At the present time, BoxKite.Twitter supports API version 1.1 of the following:

* [Twitter 1.1 REST API](https://dev.twitter.com/docs/api/1.1)
* [User streams](https://dev.twitter.com/docs/streaming-apis/streams/user)
* [Search streams](https://dev.twitter.com/docs/api/1.1/post/statuses/filter)

##NuGet Package

```
Install-Package BoxKite.Twitter -Pre
```

## Wiki documentation

[Documentation is online and provided here](https://github.com/nickhodge/BoxKite.Twitter/wiki)

##To Build:
* VS2012 with latest NuGet supporting PCLs (Portable Class Libraries)

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
* 113 test methods with average of 4 assertion tests each
* Both Twitter API 1.1 and Userstream Unit Tests
* 45 "Live Fire Tests". Included as a separate project, it is engaged via the Console; uses a real Twitter Account to exercise the API. Configurable Test series, individual tests that can be run.
* Test source is JSON from [dev.twitter.com](https://dev.twitter.com/) and where innaccurate on the site, taken from live data or corrected to match live data.
* Error response type testing (eg: 429 rate_limits) & generic decode tests included


## Dependencies

* Without [Newtonsoft Json.NET](http://json.net) this library would not exist (MIT License)
* Microsoft's Open sourced .NET [Reactive Extensions](https://rx.codeplex.com/) (Apache License 2.0)
* [Portable version of the HTTP Client Libraries](http://blogs.msdn.com/b/bclteam/p/httpclient.aspx) from the Microsoft ASP.NET WebAPI team [HTTP Client Libraries](http://nuget.org/packages/Microsoft.Net.Http/2.1.6-rc)  (sourced from NuGet, currently Licensed under [DotNetBetaUnsupported](http://go.microsoft.com/fwlink/?LinkID=279007) license)

### (Dependency when running BoxKite.Twitter.Tests only)

* Great little collection of extension methods that make writing UnitTests easier [FluentAssertions](http://fluentassertions.codeplex.com/) (MS-PL) 


##Copyright, License Information

[BoxKite.Twitter](https://github.com/shiftkey/BoxKite.Twitter) was started by Brendan Forster ([https://github.com/shiftkey/](https://github.com/shiftkey/)) as a part of a larger Twitter project now on permanent hiatus.

BoxKite.Twitter is Licensed under: 
[MS-PL] (http://opensource.org/licenses/MS-PL)

Copyright: 
[Nick Hodge](https://github.com/nickhodge/) and [Brendan Forster](https://github.com/shiftkey/) 2012-2013

## Why BoxKite?

![boxkite](https://raw.github.com/nickhodge/BoxKite.Twitter/master/src/BoxKite.Twitter/Assets/logo-wide.png "Logo")

Everyone likes flying kites, right? The original inventor of the [box kite](http://en.wikipedia.org/wiki/Box_kite) was [Lawrence Hargrave](http://en.wikipedia.org/wiki/Lawrence_Hargrave). In 1893. And he was an Australian. Sadly missing from our newer plastic yet colourful currency, Lawrence was one of those guys who helped manned flight become a reality.

And keeping with the Twitter bird theme, flying and airborne I created a fork of the MahTweets twitter client in late 2011 and named it [MahTweets Lawrence Hargrave](https://github.com/nickhodge/MahTweets.LawrenceHargrave).

In mid 2012, Brendan needed a code name for a new Twitter "thing" he was working on. Keeping in this flighty theme, I named his app "BoxKite". For posterity's sake, I have absconded with Brendan's code, and my contributed codename (and a nifty icon) and have dubbed this project BoxKite.