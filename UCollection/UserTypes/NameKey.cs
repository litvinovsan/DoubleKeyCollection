using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCollection.UserTypes
{
    public class NameKey : IEquatable<NameKey>
    {
        public string Name { get; }

        public NameKey()
        {
            Name = Guid.NewGuid().ToString();
        }
        public NameKey(string name)
        {
            Name = name;
        }

        public bool Equals(NameKey other)
        {
            var idName = other as NameKey;

            if (idName == null) return false;

            return Name.Equals(idName.Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)1166167426;
                hash = hash * 16777379 ^ Name.GetHashCode();
                return hash;
            }

        }
        public override string ToString()
        {
            return $"Name: {Name}";
        }
    }
}
