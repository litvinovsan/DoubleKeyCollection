using System;

namespace UCollection.UserTypes
{
    public class Person : IEquatable<Person>
    {
        public int Id { get; }
        public string Notes { get; }

        public Person()
        {
            Id = IdCnt++;
            Notes = Guid.NewGuid().ToString();
        }

        public Person(int id, string notes)
        {
            Id = id;
            Notes = notes;
        }

        public static int IdCnt;

        public bool Equals(Person other)
        {
            var person = other as Person;

            if (person == null) return false;

            return Id.Equals(other.Id) && Notes.Equals(other.Notes);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ Id.GetHashCode();
                hash = hash * 16777619 ^ Notes.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"Person info: Id-{Id}, Notes-{Notes}";
        }
    }
}
