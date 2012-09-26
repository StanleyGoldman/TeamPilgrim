using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common
{
    public class FactoryDictionary<T> : IDictionary<string, T>
    {
        private readonly Func<string, T> _factory;
        private readonly Dictionary<string, T> _dictionary;

        public FactoryDictionary(Func<string, T> factory, Dictionary<string, T> dictionary)
        {
            _factory = factory;
            _dictionary = dictionary;
        }

        public FactoryDictionary(Func<string, T> factory, int count)
            : this(factory, new Dictionary<string, T>(count))
        {

        }

        public FactoryDictionary(Func<string, T> factory)
            : this(factory, new Dictionary<string, T>())
        {

        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, T> item)
        {
            _dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            _dictionary.ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            if (_dictionary.ContainsKey(item.Key))
            {
                if (_dictionary[item.Key].Equals(item.Value))
                {
                    _dictionary.Remove(item.Key);
                    return true;
                }
            }

            return false;
        }

        public int Count { get { return _dictionary.Count; } }

        public bool IsReadOnly { get; private set; }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(string key, T value)
        {
            _dictionary.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out T value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public T this[string key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<T> Values
        {
            get { return _dictionary.Values; }
        }
    }
}