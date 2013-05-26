using System;
using System.Collections.ObjectModel;

namespace BoxKite.Twitter.Models
{
    public class TweetsContainer : KeyedCollection<long,Tweet>
    {
        public void SubscribeTo(IObservable<Tweet> tweetstream)
        {
            tweetstream.Subscribe(AddIfUnique);
        }

        public void AddIfUnique(Tweet t)
        {
            if (!this.Contains(t.Id))
                Add(t);
        }

        protected override long GetKeyForItem(Tweet item)
        {
            return item.Id;
        }
    }
}
