using NLog.Config;
using NLog.Targets;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using AB15_GUI.WPF.Models;

namespace AB15_GUI.WPF.NLog
{
    /// <summary>
    /// Writes log messages to memory for programmatic retrieval
    /// Records stored in format useful for UI binding
    /// </summary>
    [Target("MemoryRecord")]
    public sealed class LogMemoryRecordtTarget : TargetWithLayout
    {
        private const int MaxListSize = 100;
        private readonly ThreadSafeList<LoggerRecord> _logs = new ThreadSafeList<LoggerRecord>(MaxListSize);

        public LogMemoryRecordtTarget()
        {
            //
        }

        /// <summary>
        /// Gets the list of logs gathered in the <see cref="MemoryRecordTarget"/>.
        /// </summary>
        /// <remarks>
        /// Thread safety is implemented using lock
        /// </remarks>
        public IList<LoggerRecord> Logs => _logs;

        protected override void Write(LogEventInfo logEvent)
        {
            LoggerRecord logRecord = new LoggerRecord();
            logRecord.TimeStamp = logEvent.TimeStamp;
            logRecord.Index = logEvent.SequenceID;
            logRecord.Level = logEvent.Level.ToString().ToUpper();
            logRecord.Message = logEvent.Message;

            _logs.Add(logRecord);
        }

        /// <summary>
        /// Thread safe generic list implementation using lock
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class ThreadSafeList<T> : IList<T>
        {
            private readonly object _lock = new object();
            private readonly int MaxListSize;
            private readonly List<T> _list = new List<T>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="maxListSize">Maximum number of records stored in the list</param>
            public ThreadSafeList(int maxListSize = 100)
            {
                MaxListSize = maxListSize;
            }

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
                        _list[index] = value;
                    }
                }
            }

            public int Count => _list.Count;

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
                        _list.RemoveAt(0);
                    }
                    _list.Add(item);
                }
            }

            void ICollection<T>.Clear()
            {
                lock (_lock)
                {
                    _list.Clear();
                }
            }

            bool ICollection<T>.Contains(T item)
            {
                lock (_lock)
                {
                    return _list.Contains(item);
                }
            }

            void ICollection<T>.CopyTo(T[] array, int arrayIndex)
            {
                lock (_lock)
                {
                    _list.CopyTo(array, arrayIndex);
                }
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                lock (_lock)
                {
                    return _list.GetEnumerator();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                lock (_lock)
                {
                    return _list.GetEnumerator();
                }
            }

            public int IndexOf(T item)
            {
                lock (_lock)
                {
                    return _list.IndexOf(item);
                }
            }

            void IList<T>.Insert(int index, T item)
            {
                lock (_lock)
                {
                    _list.Insert(index, item);
                }
            }

            bool ICollection<T>.Remove(T item)
            {
                lock (_lock)
                {
                    return _list.Remove(item);
                }
            }

            void IList<T>.RemoveAt(int index)
            {
                lock (_lock)
                {
                    _list.RemoveAt(index);  
                }
            }
        }
    }
}
