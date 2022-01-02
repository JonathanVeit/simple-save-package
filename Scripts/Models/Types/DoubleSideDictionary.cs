using System;
using System.Collections.Generic;

namespace SimpleSave.Models
{
    /// <summary>
    /// Dictionary which works in both ways.
    /// </summary>
    /// <typeparam name="TKey1">First key type.</typeparam>
    /// <typeparam name="TKey2">Second key type.</typeparam>
    internal class DoubleSideDictionary<TKey1, TKey2>
    {
        private readonly Dictionary<TKey1, TKey2> _forward;
        private readonly Dictionary<TKey2, TKey1> _backward;

        public DoubleSideDictionary()
        {
            _forward = new Dictionary<TKey1, TKey2>();
            _backward = new Dictionary<TKey2, TKey1>();
        }

        public void Add(TKey1 key1, TKey2 key2)
        {
            if (_forward.ContainsKey(key1) ||
                _backward.ContainsKey(key2))
            {
                throw new ArgumentException($"They keys {key1} and {key2} have already been added!");
            }

            _forward.Add(key1, key2);
            _backward.Add(key2, key1);
        }

        public void Remove(TKey1 key1)
        {
            if (!_forward.ContainsKey(key1))
            {
                return;
            }

            _backward.Remove(_forward[key1]);
            _forward.Remove(key1);
        }
        public void Remove(TKey2 key2)
        {
            if (!_backward.ContainsKey(key2))
            {
                return;
            }

            _forward.Remove(_backward[key2]);
            _backward.Remove(key2);
        }

        public TKey2 this[TKey1 key1]
        {
            get
            {
                if (!_forward.ContainsKey(key1))
                {
                    throw new ArgumentException();
                }

                return _forward[key1];
            }
            set
            {
                _forward[key1] = value;
                _backward.Remove(value);
                _backward.Add(value, key1);
            }
        }
        public TKey1 this[TKey2 key2]
        {
            get
            {
                if (!_backward.ContainsKey(key2))
                {
                    throw new ArgumentException();
                }

                return _backward[key2];
            }
            set
            {
                _backward[key2] = value;
                _forward.Remove(value);
                _forward.Add(value, key2);
            }
        }


        public bool TryGetValue(TKey1 key1, out TKey2 key2)
        {
            return _forward.TryGetValue(key1, out key2);
        }
        public bool TryGetValue(TKey2 key2, out TKey1 key1)
        {
            return _backward.TryGetValue(key2, out key1);
        }

        public void Clear()
        {
            _forward.Clear();
            _backward.Clear();
        }
    }
}