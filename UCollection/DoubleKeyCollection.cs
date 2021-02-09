using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UCollection
{
    public class DoubleKeyCollection<TId, TName, TValue> : IEnumerable<DoubleKeyCollection<TId, TName, TValue>.KeyValueStruct>
    {
        #region /// Propetries

        private readonly Dictionary<Tuple<TId, TName>, TValue> _dictionary = new Dictionary<Tuple<TId, TName>, TValue>();
        public Dictionary<Tuple<TId, TName>, TValue>.KeyCollection Keys
        {
            get { return _dictionary.Keys; }
        }
        public Dictionary<Tuple<TId, TName>, TValue>.ValueCollection Values
        {
            get { return _dictionary.Values; }
        }

        /// <summary>
        ///  Collection of Id's
        /// </summary>
        public IEnumerable<TId> KeysId
        {
            get { return _dictionary.Keys.Select(x => x.Item1); }
        }

        /// <summary>
        ///  Collection of Names's
        /// </summary>
        public IEnumerable<TName> KeysName
        {
            get { return _dictionary.Keys.Select(x => x.Item2); }
        }

        ///  Total number of entries in collection
        public int Count
        {
            get { return _dictionary.Count; }
        }

        #endregion

        #region /// Methods

        /// <summary>
        /// Add Value with complex Id-Name key to collection.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(TId id, TName name, TValue value)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var keyToAdd = new Tuple<TId, TName>(id, name);

            if (Keys.Contains(keyToAdd)) throw new KeyAlreadyExistsException($"Key: {id} - {name} already exists"); ;

            try
            {
                _dictionary.Add(new Tuple<TId, TName>(id, name), value);
            }
            catch (Exception)
            {
                throw new KeyAlreadyExistsException($"Key: {id} - {name} already exists");
            }
        }

        /// <summary>
        /// Remove Value by Id and Name keys
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public bool Remove(TId id, TName name)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var result = false;

            if (!KeysId.Contains(id) || !KeysName.Contains(name)) return false;

            try
            {
                var key = new Tuple<TId, TName>(id, name);
                if (Keys.Contains(key))
                {
                    result = _dictionary.Remove(key);
                }
            }
            catch (Exception e)
            {
                throw new DoubleKeyCollectionException("Remove operation Error. " + e.Message);
            }

            return result;
        }


        /// <summary>
        ///  Clear Collection
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// Returns True in case of success getting value by Id and Name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TId id, TName name, out TValue value)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return _dictionary.TryGetValue(new Tuple<TId, TName>(id, name), out value);
        }

        /// <summary>
        /// Get Value by both keys
        /// </summary>
        /// <param name="id">First key</param>
        /// <param name="name">second key</param>
        /// <returns></returns>
        public TValue this[TId id, TName name]
        {
            get
            {
                if (id == null) throw new ArgumentNullException(nameof(id));
                if (name == null) throw new ArgumentNullException(nameof(name));

                try
                {
                    var result = _dictionary[new Tuple<TId, TName>(id, name)];

                    return result;
                }
                catch (Exception e)
                {
                    throw new KeyNotExistsException($"Requested pair {id} - {name} does not exist", e);
                }
            }
            set
            {
                if (id == null) throw new ArgumentNullException(nameof(id));
                if (name == null) throw new ArgumentNullException(nameof(name));
                try
                {
                    _dictionary[new Tuple<TId, TName>(id, name)] = value;
                }
                catch (Exception e)
                {
                    throw new KeyNotExistsException($"Requested pair {id} - {name} does not exist", e);
                }
            }
        }

        /// <summary>
        /// Get collection of entries where the first key is equal 'id'
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<TName, TValue>> this[TId id]
        {
            get
            {
                if (id == null) throw new ArgumentNullException(nameof(id));
                try
                {
                    var result = _dictionary.Where(x => x.Key.Item1.Equals(id)).Select((z) => new Tuple<TName, TValue>(z.Key.Item2, z.Value));
                    return result;
                }
                catch (Exception e)
                {
                    throw new KeyNotExistsException($"Requested Key {id} - does not exist", e);
                }
            }
        }

        /// <summary>
        /// Get collection of entries where the second key is equal 'name'
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<TId, TValue>> this[TName name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                try
                {
                    var result = _dictionary.Where(x => x.Key.Item2.Equals(name)).Select((z) => new Tuple<TId, TValue>(z.Key.Item1, z.Value));

                    return result;
                }
                catch (Exception e)
                {
                    throw new KeyNotExistsException($"Requested Key {name} - does not exist", e);
                }
            }
        }

        /// <summary>
        /// Returns True if the collection contains "value"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(TValue value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            return _dictionary.ContainsValue(value);
        }

        /// <summary>
        /// Returns True if the collection contains uniq combination of Id and Name
        /// </summary>
        /// <param name="id">Key</param>
        /// <param name="name">Key</param>
        /// <returns></returns>
        public bool Contains(TId id, TName name)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return _dictionary.ContainsKey(new Tuple<TId, TName>(id, name));
        }

        /// <summary>
        /// Returns True if the collection contains Id key
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns></returns>
        public bool Contains(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return KeysId.Contains(id);
        }

        /// <summary>
        /// Returns True if the collection contains Name key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(TName name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return KeysName.Contains(name);
        }
        #endregion

        #region /// Exceptions 

        [Serializable]
        public class KeyNotExistsException : Exception
        {
            public KeyNotExistsException() { }
            public KeyNotExistsException(string message) : base(message) { }
            public KeyNotExistsException(string message, Exception inner) : base(message, inner) { }
            protected KeyNotExistsException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        [Serializable]
        public class DoubleKeyCollectionException : Exception
        {
            public DoubleKeyCollectionException() { }
            public DoubleKeyCollectionException(string message) : base(message) { }
            public DoubleKeyCollectionException(string message, Exception inner) : base(message, inner) { }
            protected DoubleKeyCollectionException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        [Serializable]
        public class KeyAlreadyExistsException : Exception
        {
            public KeyAlreadyExistsException() { }
            public KeyAlreadyExistsException(string message) : base(message) { }
            public KeyAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
            protected KeyAlreadyExistsException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        [Serializable]
        public class ValueNotExistsException : Exception
        {
            public ValueNotExistsException() { }
            public ValueNotExistsException(string message) : base(message) { }
            public ValueNotExistsException(string message, Exception inner) : base(message, inner) { }
            protected ValueNotExistsException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
        #endregion

        #region /// Implementation of interfaces
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator<KeyValueStruct> IEnumerable<KeyValueStruct>.GetEnumerator()
        {
            foreach (var item in _dictionary)
            {
                yield return
                    new KeyValueStruct()
                    {
                        Id = item.Key.Item1,
                        Name = item.Key.Item2,
                        Value = item.Value
                    };
            }
        }

        public struct KeyValueStruct
        {
            public TId Id;
            public TName Name;
            public TValue Value;
        }

        #endregion
    }
}

