using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UCollection
{
    public class MultiCollection<TId, TName, TValue> : IEnumerable<MultiCollection<TId, TName, TValue>.KeyValueStruct>
        where TId : IEquatable<TId> where TName : IEquatable<TName>
    {
        #region /// Propetries

        // Main collections
        private readonly Dictionary<KeyStruct, TValue> _dictionary = new Dictionary<KeyStruct, TValue>();
        private readonly Dictionary<TId, List<TName>> _dIdName = new Dictionary<TId, List<TName>>();
        private readonly Dictionary<TName, List<TId>> _dNameId = new Dictionary<TName, List<TId>>();

        public Dictionary<KeyStruct, TValue>.KeyCollection Keys
        {
            get { return _dictionary.Keys; }
        }
        public Dictionary<KeyStruct, TValue>.ValueCollection Values
        {
            get { return _dictionary.Values; }
        }
        public int Count
        {
            get { return _dictionary.Count; }
        }

        private readonly object _locker = new object();
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

            var keyToAdd = new KeyStruct(id, name);

            if (Keys.Contains(keyToAdd))
            {
                throw new KeyAlreadyExistsException($"Key: {id} - {name} already exists");
            }

            try
            {
                lock (_locker)
                {
                    _dictionary.Add(keyToAdd, value);
                    SaveKeyPairHelper(id, name);
                }
            }
            catch (Exception)
            {
                throw new CollectionException($"Key: {id} - {name} adding error");
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
                var key = new KeyStruct(id, name);

                lock (_locker)
                {
                    if (Keys.Contains(key))
                    {
                        result = _dictionary.Remove(key);
                        RemoveKeyPairHelper(id, name);
                    }
                }
            }
            catch (Exception e)
            {
                throw new CollectionException("Remove operation Error. " + e.Message);
            }

            return result;
        }

        /// <summary>
        ///  Clear Collection
        /// </summary>
        public void Clear()
        {
            lock (_locker)
            {
                _dictionary.Clear();
                _dNameId.Clear();
                _dIdName.Clear();
            }
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

            return _dictionary.TryGetValue(new KeyStruct(id, name), out value);
        }

        /// <summary>
        /// Returns True in case of success getting value by Id.
        /// Attention! Returns the first value found with the specified Id.
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
                value = this[id].First().Value;
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
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TName name, ref TValue value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!Contains(name)) return false;
            try
            {
                value = this[name].First().Value;
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
                    lock (_locker)
                    {
                        var result = _dictionary[new KeyStruct(id, name)];
                        return result;
                    }
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
                    _dictionary[new KeyStruct(id, name)] = value;
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
        public IEnumerable<KeyValuePair<KeyStruct, TValue>> this[TId id]
        {
            get
            {
                if (id == null) throw new ArgumentNullException(nameof(id));
                try
                {
                    var isSuccess = _dIdName.TryGetValue(id, out var namesList);

                    if (!isSuccess || namesList.Count == 0) throw new KeyNotExistsException($"Requested Key {id} - does not exist");

                    IEnumerable<KeyValuePair<KeyStruct, TValue>> resultEnumerable;

                    if (namesList.Count == 1)
                    {
                        var strKey = new KeyStruct(id, namesList[0]);
                        resultEnumerable = _dictionary.Where(x => x.Key.Equals(strKey));
                    }
                    else // NamesList contains more than 1 entries for uniq id.     1-"A"  1-"B" 1-"C"
                    {
                        var keyStructs = new KeyStruct[namesList.Count];

                        for (int i = 0; i < namesList.Count; i++)
                        {
                            keyStructs[i] = new KeyStruct(id, namesList[i]);
                        }

                        resultEnumerable = keyStructs.Select(x => new KeyValuePair<KeyStruct, TValue>(x, _dictionary[x]));
                    }

                    return resultEnumerable;
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
        public IEnumerable<KeyValuePair<KeyStruct, TValue>> this[TName name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                try
                {
                    var isSuccess = _dNameId.TryGetValue(name, out var idList);

                    if (!isSuccess || idList.Count == 0) throw new KeyNotExistsException($"Requested Key {name} - does not exist");

                    IEnumerable<KeyValuePair<KeyStruct, TValue>> resultEnumerable;

                    if (idList.Count == 1)
                    {
                        var strKey = new KeyStruct(idList[0], name);


                        resultEnumerable = _dictionary.Where(x => x.Key.Equals(strKey));
                    }
                    else // IdList contains more than 1 entries for uniq name.      "A" - 1; "A" - 2; "A" - 3
                    {
                        var keyStructs = new KeyStruct[idList.Count];

                        for (int i = 0; i < idList.Count; i++)
                        {
                            keyStructs[i] = new KeyStruct(idList[i], name);
                        }

                        resultEnumerable = keyStructs.Select(x => new KeyValuePair<KeyStruct, TValue>(x, _dictionary[x]));
                    }

                    return resultEnumerable;
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

            lock (_locker)
            {
                return _dictionary.ContainsValue(value);
            }
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

            lock (_locker)
            {
                return _dictionary.ContainsKey(new KeyStruct(id, name));
            }
        }

        /// <summary>
        /// Returns True if the collection contains Id key
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns></returns>
        public bool Contains(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            lock (_locker)
            {
                return _dIdName.ContainsKey(id);
            }
        }

        /// <summary>
        /// Returns True if the collection contains Name key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(TName name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            lock (_locker)
            {
                return _dNameId.ContainsKey(name);
            }
        }


        #region /// Helper Methods

        /// <summary>
        /// Store Keys to helper collections to get Values faster
        /// </summary>
        /// <param name="idKey"></param>
        /// <param name="nameKey"></param>
        private void SaveKeyPairHelper(TId idKey, TName nameKey)
        {
            // Id -List<Name> Collection
            if (_dIdName.ContainsKey(idKey))
            {
                _dIdName[idKey].Add(nameKey);
            }
            else
            {
                _dIdName.Add(idKey, new List<TName>() { nameKey });
            }

            // Name -List<Id> Collection

            if (_dNameId.ContainsKey(nameKey))
            {
                _dNameId[nameKey].Add(idKey);
            }
            else
            {
                _dNameId.Add(nameKey, new List<TId>() { idKey });
            }
        }

        /// <summary>
        /// Remove Keys to helper collections to get Values faster
        /// </summary>
        /// <param name="idKey"></param>
        /// <param name="nameKey"></param>
        private void RemoveKeyPairHelper(TId idKey, TName nameKey)
        {
            // Id -List<Name> Collection
            if (_dIdName.ContainsKey(idKey))
            {
                if (_dIdName[idKey].Count > 1)
                {
                    _dIdName[idKey].Remove(nameKey);
                }
                else
                {
                    _dIdName.Remove(idKey);
                }
            }
            // Name -List<Id> Collection

            if (_dNameId.ContainsKey(nameKey))
            {
                if (_dNameId[nameKey].Count > 1)
                {
                    _dNameId[nameKey].Remove(idKey);
                }
                else
                {
                    _dNameId.Remove(nameKey);
                }
            }
        }

        #endregion

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
        public class CollectionException : Exception
        {
            public CollectionException() { }
            public CollectionException(string message) : base(message) { }
            public CollectionException(string message, Exception inner) : base(message, inner) { }
            protected CollectionException(
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
                        Id = item.Key.Id,
                        Name = item.Key.Name,
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

        #region /// Key Struct
        public struct KeyStruct : IEquatable<KeyStruct>
        {
            public TId Id { get; }
            public TName Name { get; }

            public KeyStruct(TId id, TName name)
            {
                Id = id;
                Name = name;
            }
            public KeyStruct(KeyStruct kStr)
            {
                Id = kStr.Id;
                Name = kStr.Name;
            }

            public override bool Equals(object obj)
            {
                return obj is KeyStruct && Equals((KeyStruct)obj);
            }

            public bool Equals(KeyStruct other)
            {
                return EqualityComparer<TId>.Default.Equals(Id, other.Id) &&
                       EqualityComparer<TName>.Default.Equals(Name, other.Name);
            }

            public override int GetHashCode()
            {
                var hashCode = -1919740922;
                hashCode = hashCode * -1521134295 + EqualityComparer<TId>.Default.GetHashCode(Id);
                hashCode = hashCode * -1521134295 + EqualityComparer<TName>.Default.GetHashCode(Name);
                return hashCode;
            }
        }
        #endregion
    }
}

