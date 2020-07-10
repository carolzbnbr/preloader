using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace XamarinPreLoaderSample
{
    public class SmartObservableCollection<T> : ObservableCollection<T>
    {
        protected readonly object locker = new object();

        /// <summary>
        /// This private variable holds the flag to
        /// turn on and off the collection changed notification.
        /// </summary>
        private bool suspendCollectionChangeNotification;

        /// <summary>
        /// Initializes a new instance of the FastObservableCollection class.
        /// </summary>
        public SmartObservableCollection()
            : base()
        {
            suspendCollectionChangeNotification = false;
        }


        public SmartObservableCollection(IEnumerable<T> items) : this()
        {
            ReplaceRange(items);
        }

        /// <summary>
        /// This event is overriden CollectionChanged event of the observable collection.
        /// </summary>
        public override event NotifyCollectionChangedEventHandler CollectionChanged;


        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
        {
            if (notificationMode != NotifyCollectionChangedAction.Add && notificationMode != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("Mode must be either Add or Reset for AddRange.", nameof(notificationMode));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            CheckReentrancy();

            lock (locker)
            {
                if (notificationMode == NotifyCollectionChangedAction.Reset)
                {
                    foreach (var i in collection)
                        Items.Add(i);

                    OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                    return;
                }

                int startIndex = Count;
                var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);
                foreach (var i in changedItems)
                    Items.Add(i);

                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems, startIndex));
            }


        }



        /// <summary> 
        /// Clears the current collection and replaces it with the specified item. 
        /// </summary> 
        public void Replace(T item) => ReplaceRange(new T[] { item });

        /// <summary> 
        /// Clears the current collection and replaces it with the specified collection. 
        /// </summary> 
        public void ReplaceRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            lock (locker)
            {
                Items.Clear();
                AddRange(collection, NotifyCollectionChangedAction.Reset);
            }

        }

        ///// <summary>
        ///// This method adds the given generic list of items
        ///// as a range into current collection by casting them as type T.
        ///// It then notifies once after all items are added.
        ///// </summary>
        ///// <param name="items">The source collection.</param>
        //public void AddRange(IEnumerable<T> items)
        //{
        //    lock (locker)
        //    {
        //        SuspendCollectionChangeNotification();
        //        foreach (var i in items)
        //        {
        //            InsertItem(Count, i);
        //        }
        //        NotifyChanges();
        //    }
        //}

        public void InsertRange(IEnumerable<T> items, int startIndex)
        {
            if (items == null)
            {
                return;
            }
            lock (locker)
            {
                SuspendCollectionChangeNotification();
                foreach (var i in items)
                {
                    if (!ReferenceEquals(i, null))
                    {
                        Insert(startIndex, i);
                    }
                }
                NotifyChanges();
            }
        }

        public new void Clear()
        {
            lock (locker)
            {
                base.Clear();
            }
        }


        /// <summary>
        /// Raises collection change event.
        /// </summary>
        public void NotifyChanges()
        {
            ResumeCollectionChangeNotification();
            var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(arg);
        }

        /// <summary>
        /// This method removes the given generic list of items as a range
        /// into current collection by casting them as type T.
        /// It then notifies once after all items are removed.
        /// </summary>
        /// <param name="items">The source collection.</param>
        public void RemoveItems(IList<T> items)
        {
            lock (locker)
            {
                SuspendCollectionChangeNotification();
                foreach (var i in items)
                {
                    Remove(i);
                }
                NotifyChanges();
            }
        }


        /// <summary>
        /// Resumes collection changed notification.
        /// </summary>
        public void ResumeCollectionChangeNotification()
        {
            suspendCollectionChangeNotification = false;
        }

        /// <summary>
        /// Suspends collection changed notification.
        /// </summary>
        public void SuspendCollectionChangeNotification()
        {
            suspendCollectionChangeNotification = true;
        }

        /// <summary>
        /// This collection changed event performs thread safe event raising.
        /// </summary>
        /// <param name="e">The event argument.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Recommended is to avoid reentry 
            // in collection changed event while collection
            // is getting changed on other thread.
            using (BlockReentrancy())
            {
                if (!suspendCollectionChangeNotification)
                {

                    CollectionChanged?.Invoke(this, e);
                }
            }
        }


        /// <summary> 
        /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). NOTE: with notificationMode = Remove, removed items starting index is not set because items are not guaranteed to be consecutive.
        /// </summary> 
        public void RemoveRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Reset)
        {
            if (notificationMode != NotifyCollectionChangedAction.Remove && notificationMode != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("Mode must be either Remove or Reset for RemoveRange.", nameof(notificationMode));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            CheckReentrancy();

            if (notificationMode == NotifyCollectionChangedAction.Reset)
            {

                foreach (var i in collection)
                    Items.Remove(i);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                return;
            }

            var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);
            for (int i = 0; i < changedItems.Count; i++)
            {
                if (!Items.Remove(changedItems[i]))
                {
                    changedItems.RemoveAt(i); //Can't use a foreach because changedItems is intended to be (carefully) modified
                    i--;
                }
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItems, -1));
        }
    }
}
