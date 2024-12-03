using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;

namespace AB15_GUI.WPF.Models
{
        /// <summary>
        /// Thread safe generic observable list implementation using lock.
        /// Has limited number of items that can be stored (configurable). Size handling is done in circular buffer manner
        /// Restriction: does not raise PropertyChanged events for properties of list item (T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ThreadSafeObservableList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            /// <summary>
            /// Max number of items stored in the list
            /// </summary>
            private readonly int MaxListSize;

            /// <summary>
            /// Lock object for thread handling
            /// </summary>
            private readonly object _lock = new object();

            /// <summary>
            /// Underlying attribute that holds data
            /// </summary>
            /// <typeparam name="T">type of the items in list</typeparam>
            private readonly List<T> _list = new List<T>();

            /// <summary>
            /// Thread safe observable List implementation
            /// </summary>
            /// <param name="maxListSize">Maximum number of records stored in the list</param>
            public ThreadSafeObservableList(int maxListSize = 100)
            {
                 MaxListSize = maxListSize;
            }

            /// <summary>
            /// INotifyCollectionChanged event
            /// </summary>
            public event NotifyCollectionChangedEventHandler? CollectionChanged;

            /// <summary>
            /// Method to call CollectionChanged event. Notifies that collection was changed
            /// </summary>
            /// <param name="e">parameters of how collection was changed</param>
            protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                CollectionChanged?.Invoke(this, e);
            }

            /// <summary>
            /// INotifyPropertyChanged event
            /// </summary>
            public event PropertyChangedEventHandler? PropertyChanged;

            /// <summary>
            /// Method to call PropertyChanged event. Notifies that property was changed
            /// </summary>
            /// <param name="propertyName"></param>
            protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            /// <summary>
            /// Indexer of ThreadSafeObservableList
            /// </summary>
            /// <value></value>
            public T this[int index]
            {
                get
                {
                    lock(_lock)
                    {
                        return _list[index];
                    }
                }

                set 
                { 
                    lock(_lock)
                    {
                        T oldItem = _list[index];
                        _list[index] = value;
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem));
                    }
                }
            }

            /// <summary>
            /// Get number of items in the list
            /// </summary>
            public int Count => _list.Count;

            /// <summary>
            /// Get information of read status of list
            /// </summary>
            /// <returns>True if list is readonly, False otherwise</returns>
            public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

            /// <summary>
            /// Append item to the end of the list
            /// If list is already full, remove item from the beginning
            /// </summary>
            /// <param name="item">item to be added to list</param>
            public void Add(T item)
            {
                lock(_lock)
                {
                    if ((MaxListSize > 0) && (_list.Count >= MaxListSize))
                    {
                        T removedItem = _list.ElementAt(0);
                        _list.RemoveAt(0);
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, 0));
                    }
                    _list.Add(item);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                    OnPropertyChanged(nameof(Count));
                }
            }

            /// <summary>
            /// Remove all items from collection
            /// </summary>
            public void Clear()
            {
                lock (_lock)
                {
                    _list.Clear();
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    OnPropertyChanged(nameof(Count));
                }
            }

            /// <summary>
            /// Check if specified item is present in the list
            /// </summary>
            /// <param name="item"></param>
            /// <returns>True if item is present, False otherwise</returns>
            public bool Contains(T item)
            {
                lock (_lock)
                {
                    return _list.Contains(item);
                }
            }

            /// <summary>
            /// Copies the entire List to a compatible one-dimensional array, starting at the specified index of the target array. 
            /// </summary>
            /// <param name="array">destination array</param>
            /// <param name="arrayIndex">starting index of target array</param>
            public void CopyTo(T[] array, int arrayIndex)
            {
                lock (_lock)
                {
                    _list.CopyTo(array, arrayIndex);
                }
            }

            /// <summary>
            /// Get an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that iterates through the collection.</returns>
            public IEnumerator<T> GetEnumerator()
            {
                lock (_lock)
                {
                    return _list.GetEnumerator(); // TODO: potential issue in threading
                }
            }

            /// <summary>
            /// Get an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that iterates through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                lock (_lock)
                {
                    return _list.GetEnumerator(); // TODO: potential issue in threading
                }
            }

            /// <summary>
            /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire List.
            /// </summary>
            /// <param name="item">item for searching index</param>
            /// <returns>he zero-based index of the first occurrence of item within the entire List, if found; otherwise, -1.</returns>
            public int IndexOf(T item)
            {
                lock (_lock)
                {
                    return _list.IndexOf(item);
                }
            }

            /// <summary>
            /// Inserts an element into the List at the specified index.
            /// </summary>
            /// <param name="index">index where element will be inserted</param>
            /// <param name="item">item that will be inserted</param>
            public void Insert(int index, T item)
            {
                lock (_lock)
                {
                    _list.Insert(index, item);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                    OnPropertyChanged(nameof(Count));
                }
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the List.
            /// </summary>
            /// <param name="item">item that will be removed</param>
            /// <returns>True if removal was successful, False otherwise</returns>
            public bool Remove(T item)
            {
                lock (_lock)
                {
                    bool removalStatus = _list.Remove(item);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                    OnPropertyChanged(nameof(Count));
                    return removalStatus;
                }
            }

            /// <summary>
            /// Removes the element at the specified index of the List.
            /// </summary>
            /// <param name="index">index of element that will be removed</param>
            public void RemoveAt(int index)
            {
                lock (_lock)
                {
                    T removedItem = _list.ElementAt(index);
                    _list.RemoveAt(index);  
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
                    OnPropertyChanged(nameof(Count));
                }
            }
        }
}