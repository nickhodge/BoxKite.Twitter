// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using System.Linq;
using BoxKite.Twitter.Models.Service;


// hinted from http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/f71f6592-521f-4dee-b4c8-f7a8149ad819
namespace BoxKite.Twitter.Helpers
{
    /// <summary>
    /// Represents a strongly typed enumerable collection of Twitter Response Objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class TwitterResponseCollection<T> : TwitterControlBase, IEnumerable<T>, IList<T>
    {
         /// <summary>
        /// The array to store values.
        /// </summary>
        private List<T> twitterResponseElements;

        /// <summary>
        /// The default constructor for the collection. Initializes the collection with default capacity (4).
        /// </summary>
        public TwitterResponseCollection()
        {
            this.twitterResponseElements = new List<T>();
        }

        /// <summary>
        /// The actual number of values in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return twitterResponseElements.Count();
            }
        }

        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Adds the specified value to the collection. If the collection is full returns false; otherwise true.
        /// </summary>
        /// <param name="value">The value to add to the collection.</param>
        public bool Add(T value)
        {
            this.twitterResponseElements.Add(value);
            return true;
        }

        public void Clear()
        {
            this.twitterResponseElements.Clear();
        }

        void ICollection<T>.Add(T item)
        {
            this.twitterResponseElements.Add(item);
        }

        /// <summary>
        /// Determines whether the collection contains the specified value. Null value is considered.
        /// </summary>
        /// <param name="value">The value to find in the collection.</param>
        public bool Contains(T value)
        {
            return this.twitterResponseElements.Contains(value);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.twitterResponseElements.CopyTo(array,arrayIndex);
        }

        /// <summary>
        /// Removes the specified value from the collection. If object is present and removed returns true; otherwise false.
        /// </summary>
        /// <param name="value">The value to remove from the collection.</param>
        public bool Remove(T value)
        {
            return this.twitterResponseElements.Remove(value);
        }
        /// <summary>
        /// Returns an enumerator that iterates through the elements of the collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) twitterResponseElements).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return this.twitterResponseElements.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.twitterResponseElements.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.twitterResponseElements.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return this.twitterResponseElements[index];
            }
            set { this.twitterResponseElements[index] = value; }
        }
    }
}
