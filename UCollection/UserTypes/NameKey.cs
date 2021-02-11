using System;

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
            return other is NameKey idName && Name.Equals(idName.Name);
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
