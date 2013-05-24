// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Reactive.Subjects;

namespace BoxKite.Twitter.Extensions
{
    public static class ReactiveExtensionsExtension
    {
        // Brendan doesnt like this, and thinks I should "kill it with fire"
        // but I am going to leave it in as it balancs the "Subscribe" force with a neat "Publish"
        // and also, I grew up in an era of Mac System 7: https://en.wikipedia.org/wiki/Publish_and_Subscribe_(Mac_OS)
        // which was awesome

        public static void Publish<T>(this Subject<T> tsSubject, T tValue)
        {
            tsSubject.OnNext(tValue);
        }
    }
}
