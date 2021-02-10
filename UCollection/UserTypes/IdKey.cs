using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCollection.UserTypes
{
    public class IdKey : IEquatable<IdKey>
    {
        public int Id { get; }

        public IdKey()
        {
            Id = new Random().Next(65534);
        }
        public IdKey(int id)
        {
            Id = id;
        }

        public bool Equals(IdKey other)
        {
            var idKey = other as IdKey;

            if (idKey == null) return false;

            return Id.Equals(idKey.Id);
        }

        public override string ToString()
        {
            return $"Id: {Id}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ Id.GetHashCode();
                return hash;
            }
        }
    }
}
