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
    public class DoubleKeyCollection<TId, TName, TValue> //:ICollection<>// : IEnumerable<KeyValuePair<TId, TName>, TValue>
    {
        #region /// Propetries

       
        // KeyStruct
        private readonly Dictionary<KeyStruct, TValue> _dictionaryStruct = new Dictionary<KeyStruct, TValue>();
        public Dictionary<KeyStruct, TValue>.KeyCollection KeysStruct
        {
            get { return _dictionaryStruct.Keys; }
        }
        public TId[] KeysStructId
        {
            get { return _dictionaryStruct.Keys.Select(x => x.Id).ToArray(); }
        }
        public TName[] KeysStructName
        {
            get { return _dictionaryStruct.Keys.Select(x => x.Name).ToArray(); }
        }
        public Dictionary<KeyStruct, TValue>.ValueCollection ValuesStruct
        {
            get { return _dictionaryStruct.Values; }
        }
        public int CountStruct
        {
            get { return _dictionaryStruct.Count; }
        }

        // ValueTuple
        private readonly Dictionary<Tuple<TId, TName>, TValue> _dictionaryTuple = new Dictionary<Tuple<TId, TName>, TValue>();
        public Dictionary<Tuple<TId, TName>, TValue>.KeyCollection KeysTuple
        {
            get { return _dictionaryTuple.Keys; }
        }
        public TId[] KeysTupleId
        {
            get { return _dictionaryTuple.Keys.Select(x => x.Item1).ToArray(); }
        }
        public TName[] KeysTupleName
        {
            get { return _dictionaryTuple.Keys.Select(x => x.Item2).ToArray(); }
        }
        public Dictionary<Tuple<TId, TName>, TValue>.ValueCollection ValuesTuple
        {
            get { return _dictionaryTuple.Values; }
        }
        public int Count
        {
            get { return _dictionaryTuple.Count; }
        }

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

            if (!KeysTuple.Contains(new Tuple<TId, TName>(id, name)))
            {
                _dictionaryStruct.Add(new KeyStruct(id, name), value);
                _dictionaryTuple.Add(new Tuple<TId, TName>(id, name),  value);
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
            _dictionaryStruct.Clear();
            _dictionaryTuple.Clear();
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







        public bool ContainsKey(object key)
        {
            throw new NotImplementedException();
        }

        public bool ContainsValue(TValue value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            //return _dictionaryStruct.ContainsValue(value);
            return _dictionaryTuple.ContainsValue(value);
        }



        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public TValue this[TId id, TName name]
        {
            get
            {
                try
                {
                    // var result = _dictionary[new KeyValuePair<TId, TName>(id, name)];
                   // var result = _dictionaryStruct[new KeyStruct(id, name)];
                         var result = _dictionaryTuple[new Tuple<TId, TName>(id, name)];

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
                     _dictionaryStruct[new KeyStruct(id, name)] = value;
                }
                catch (Exception e)
                {
                    throw new KeyNotExistsException($"Requested pair {id} - {name} does not exist", e);
                }
            }
        }
        public KeyValuePair<Tuple<TId, TName>, TValue>[] this[TId id]
        {
            get
            {
                try
                {
                    //var result = _dictionaryStruct.Where(x => x.Key.Id.Equals(id)).ToArray();
                    var result = _dictionaryTuple.Where(x => x.Key.Item1.Equals(id)).ToArray();

                    return result;
                }
                catch (Exception e)
                {
                    throw new KeyNotExistsException($"Requested Key {id} - does not exist", e);
                }
            }
        }

        public KeyValuePair<Tuple<TId, TName>, TValue>[] this[TName name]
        {
            get
            {
                try
                {
                    
                   // var result = _dictionaryStruct.Where(x => x.Key.Name.Equals(name)).ToArray();
                    var result = _dictionaryTuple.Where(x => x.Key.Item2.Equals(name)).ToArray();

                    return result;
                }
                catch (Exception e)
                {
                    throw new KeyNotExistsException($"Requested Key {name} - does not exist", e);
                }
            }
        }

        public struct KeyStruct : IEquatable<KeyStruct>
        {
            public readonly TId Id;
            public readonly TName Name;

            public KeyStruct(TId id, TName name)
            {
                Id = id;
                Name = name;
            }

            public bool Equals(KeyStruct other)
            {
                 var result = EqualityComparer<TId>.Default.Equals(Id, other.Id)
                             && EqualityComparer<TName>.Default.Equals(Name, other.Name);

               
                return result;
            }

            public override bool Equals(object other)
            {
                return other is KeyStruct && Equals((KeyStruct)other);
            }

            public override int GetHashCode()
            {
                var result = 17;
                unchecked
                {
                    //result = 31 * result + EqualityComparer<TId>.Default.GetHashCode(Id);
                    //result = 31 * result + EqualityComparer<TName>.Default.GetHashCode(Name);
                    //return result;
                    return Id.GetHashCode() ^ Name.GetHashCode();
                }
            }

        }
    }
}
