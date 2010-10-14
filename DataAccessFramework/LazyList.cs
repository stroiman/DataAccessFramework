using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessFramework
{
    [Serializable]
    public class LazyList<T> : IList<T>
    {
        [NonSerialized]
        private readonly IEnumerable<T> _enumerable;
        private List<T> _list;

        public LazyList(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        private IList<T> List
        {
            get { return _list ?? (_list = new List<T>(_enumerable)); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            List.Add(item);
        }

        public void Clear()
        {
            List.Clear();
        }

        public bool Contains(T item)
        {
            return List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return List.Remove(item);
        }

        public int Count
        {
            get { return List.Count; }
        }
        public bool IsReadOnly
        {
            get { return List.IsReadOnly; }
        }
        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            List.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return List[index]; }
            set { List[index] = value; }
        }
    }
}
