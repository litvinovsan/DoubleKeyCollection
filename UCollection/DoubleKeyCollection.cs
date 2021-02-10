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
        where TId : IEquatable<TId> where TName : IEquatable<TName>
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
                var tuple = new Tuple<TId, TName>(id, name);

                _dictionary.Add(tuple, value);
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

            if (!Contains(id) || !Contains(name)) return false;

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
        public bool TryGetValue(TId id, TName name, ref TValue value)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return _dictionary.TryGetValue(new Tuple<TId, TName>(id, name), out value);
        }

        /// <summary>
        /// Returns True in case of success getting value by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TId id, ref TValue value)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (!Contains(id)) return false;
            try
            {
                value = this[id].First().Item2;
            }
            catch (Exception e)
            {
                throw new ValueNotExistsException($"Value could not be retrieved", e);
            }

            return true;
        }

        /// <summary>
        /// Returns True in case of success getting value by Name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TName name, ref TValue value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!Contains(name)) return false;
            try
            {
                value = this[name].First().Item2;
            }
            catch (Exception e)
            {
                throw new ValueNotExistsException($"Value could not be retrieved", e);
            }

            return true;
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

            return _dictionary.Count(x => x.Key.Item1.Equals(id) && x.Key.Item2.Equals(name)) > 0;
        }

        /// <summary>
        /// Returns True if the collection contains Id key
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns></returns>
        public bool Contains(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return _dictionary.Keys.FirstOrDefault(x => x.Item1.Equals(id)) != null;
        }

        /// <summary>
        /// Returns True if the collection contains Name key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(TName name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return _dictionary.Keys.FirstOrDefault(x => x.Item2.Equals(name)) != null;
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

        #region Helper Struct
        public struct KeyStruct : IEquatable<KeyStruct>
        {
            public TId Id { get; }
            public TName Name { get; }

            public KeyStruct(TId id, TName name)
            {
                Id = id;
                Name = name;
            }


            //public bool Equals(KeyStruct other)
            //{
            //    bool result = false;
            //    if (other is KeyStruct)
            //    {

            //    }

            //     result = EqualityComparer<TId>.Default.Equals(Id, other.Id)
            //                 && EqualityComparer<TName>.Default.Equals(Name, other.Name);


            //    return result;
            //}

            //public override bool Equals(object other)
            //{
            //    return other is KeyStruct && Equals((KeyStruct)other);
            //}

            //public override int GetHashCode()
            //{
            //    var result = 17;
            //    unchecked
            //    {
            //        //result = 31 * result + EqualityComparer<TId>.Default.GetHashCode(Id);
            //        //result = 31 * result + EqualityComparer<TName>.Default.GetHashCode(Name);
            //        //return result;
            //        return Id.GetHashCode() ^ Name.GetHashCode();
            //    }
            //}

            public bool Equals(KeyStruct other)
            {
                return EqualityComparer<TId>.Default.Equals(Id, other.Id) && EqualityComparer<TName>.Default.Equals(Name, other.Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is KeyStruct other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (EqualityComparer<TId>.Default.GetHashCode(Id) * 397) ^ EqualityComparer<TName>.Default.GetHashCode(Name);
                }
            }

            public static bool operator ==(KeyStruct left, KeyStruct right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(KeyStruct left, KeyStruct right)
            {
                return !left.Equals(right);
            }
        }
        #endregion
    }
}

