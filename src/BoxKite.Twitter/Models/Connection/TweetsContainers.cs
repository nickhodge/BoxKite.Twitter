// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BoxKite.Twitter.Models
{
    /*public class TweetsContainer: KeyedCollection<long,Tweet>
    {
        public void SubscribeTo(IObservable<Tweet> stream)
        {
            stream.Subscribe(AddIfUnique);
        }

        public void AddIfUnique(Tweet t)
        {
            if (!this.Contains(t))
                Add(t);
        }

        protected override long GetKeyForItem(Tweet item)
        {
            return item.Id;
        }
    }

    public class DirectMessagesContainer : KeyedCollection<long, DirectMessage>
    {
        public void SubscribeTo(IObservable<DirectMessage> stream)
        {
            stream.Subscribe(AddIfUnique);
        }

        public void AddIfUnique(DirectMessage dm)
        {
            if (!this.Contains(dm))
                Add(dm);
        }

        protected override long GetKeyForItem(DirectMessage item)
        {
            return item.Id;
        }
    }

    public class UsersContainer : KeyedCollection<int, User>
    {
        public void SubscribeTo(IObservable<User> stream)
        {
            stream.Subscribe(AddIfUnique);
        }

        public void AddIfUnique(User u)
        {
            if (!this.Contains(u))
                Add(u);
        }

        protected override int GetKeyForItem(User item)
        {
            return item.UserId;
        }
    }
     */

    public class TweetsContainer : SortedSet<long>
    {
        public void SubscribeTo(IObservable<Tweet> stream)
        {
            stream.Subscribe(AddIfUnique);
        }

        public void AddIfUnique(Tweet t)
        {
            if (!this.Contains(t.Id))
                Add(t.Id);
        }
    }

}
