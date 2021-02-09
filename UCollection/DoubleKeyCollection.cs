using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCollection
{
    /*
     *public IEnumerator<T> GetEnumerator() 
	{
		foreach(var v in arr) yield return v;
	}
     *
     * System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() 
	{
		return GetEnumerator();
	}
     *
     * class A<T>: IEnumerable<T>
     //*/
    public class DoubleKeyCollection<TId, TName, TValue> : ICollection// : IEnumerable<KeyValuePair<TId, TName>, TValue>    IQueryable<T>
    {
        #region /// Propetries

        private readonly Dictionary<Tuple<TId, TName>, TValue> _dictionary = new Dictionary<Tuple<TId, TName>, TValue>();

        private readonly Dictionary<TId, Dictionary<TName, TValue>> _dictionary2 = new Dictionary<TId, Dictionary<TName, TValue>>();


        public Dictionary<Tuple<TId, TName>, TValue>.KeyCollection Keys
        {
            get { return _dictionary.Keys; }
        }
        public TId[] KeysId
        {
            get { return _dictionary.Keys.Select(x => x.Item1).ToArray(); }
        }
        public TName[] KeysName
        {
            get { return _dictionary.Keys.Select(x => x.Item2).ToArray(); }
        }
        public Dictionary<Tuple<TId, TName>, TValue>.ValueCollection Values
        {
            get { return _dictionary.Values; }
        }

        public void CopyTo(Array array, int index)
        {
           
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public object SyncRoot { get; }
        public bool IsSynchronized { get; }

        #endregion

        #region /// Constructor

        public DoubleKeyCollection()
        { }



        #endregion

        #region /// Events

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

            if (!Keys.Contains(new Tuple<TId, TName>(id, name)))
            {
                _dictionary.Add(new Tuple<TId, TName>(id, name), value);
            }
            else
            {
                throw new KeyAlreadyExistsException($"Key: {id} - {name} already exists");
            }
        }

        /// <summary>
        ///  Clear Collection
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
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
        public KeyValuePair<Tuple<TId, TName>, TValue>[] this[TId id]
        {
            get
            {
                try
                {
                    var result = _dictionary.Where(x => x.Key.Item1.Equals(id)).ToArray();

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
        public KeyValuePair<Tuple<TId, TName>, TValue>[] this[TName name]
        {
            get
            {
                try
                {
                    var result = _dictionary.Where(x => x.Key.Item2.Equals(name)).ToArray();

                    return result;
                }
                catch (Exception e)
                {
                    throw new KeyNotExistsException($"Requested Key {name} - does not exist", e);
                }
            }
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








        public bool Contains(TValue value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            return _dictionary.ContainsValue(value);
        }



        public void Remove(object key)
        {
            throw new NotImplementedException();
        }


        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

