using System.Reactive.Subjects;

namespace BoxKite.Twitter.Extensions
{
    public static class ReactiveExtensionsExtension
    {
        public static void Publish<T>(this Subject<T> tsSubject, T tValue)
        {
            tsSubject.OnNext(tValue);
        }
    }
}
