using System;
using UCollection.UserTypes;

namespace UCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n\rПример использования коллекции \r\n ");
            // Start init.
            var collection = new DoubleKeyCollection<IdKey, NameKey, Person>
                {
                    {new IdKey(4), new NameKey("Tom"), new Person(4,"some info")},
                    {new IdKey(1), new NameKey("Alex"), new Person()},
                    {new IdKey(2), new NameKey("Alex"), new Person()},
                    {new IdKey(3), new NameKey("Hans"), new Person()},
                    {new IdKey(3), new NameKey("Jerry"), new Person()},
                    {new IdKey(9), new NameKey("Spike"), new Person()},
                    {new IdKey(4), new NameKey("Ford"), new Person()}

                };

            // Добавление записи

            // Перебор колекции 
            foreach (var item in collection)
            {
                Console.WriteLine($"Id:  {item.Id}, Name: {item.Name},  Value:{item.Value}");
            }

            Console.WriteLine($"\n\rКоличество записей: {collection.Count}");

            // Содержит Ключ 1?
            var expectedId = new IdKey(4);
            var bResult = collection.Contains(expectedId);
            Console.WriteLine($"Содержит {expectedId}? {bResult}\n\r");

            // Содержит Ключ 2?
            var expectedName = new NameKey("Jerry");
            bResult = collection.Contains(expectedName);
            Console.WriteLine($"Содержит {expectedName}? {bResult}\n\r");

            // Содержит Value?
            var expectedValue = new Person(4, "some info");
            bResult = collection.Contains(expectedValue);
            Console.WriteLine($"Содержит {expectedValue}? {bResult}\n\r");

            // Получение значения по уникальной комбинации Id - Name
            var myId = new IdKey(4);
            var myName = new NameKey("Tom");
            var value = collection[myId, myName];
            Console.WriteLine($"Получение значения через индексатор для {myId} {myName}:  {value}\n\r");

            // Получение списка записей Id-Value у которых совпадает Name
            Console.WriteLine($"\n\rСписок записей Id-Value у которых совпадает поле {myName}:");
            var etriesByName = collection[myName];
            foreach (var entr in etriesByName)
            {
                Console.WriteLine($"{entr.Item1} - {entr.Item2}");
            }

            // Получение списка записей Name-Value у которых совпадает Id
            Console.WriteLine($"\n\rСписок записей Name-Value у которых совпадает {myId}:");
            var etriesById = collection[myId];
            foreach (var entr in etriesById)
            {
                Console.WriteLine($"{entr.Item1} - {entr.Item2}");
            }

            // Попытка получения значения по id-name
            Person p = new Person();
            bResult = collection.TryGetValue(myId, myName, ref p);
            Console.WriteLine($"Получить значение по id-name: {p} {bResult}");

            // Попытка получения значения только по Id. В этом случае Id должен быть уникальным
            var myId2 = new IdKey(9);
            var myName2 = new NameKey("Enik");

            collection.Add(myId, myName, new Person(myId2.Id, myName2.Name));

            bResult = collection.TryGetValue(myId, ref p);
            Console.WriteLine($"Получить значение по id-name: {p} {bResult}");

            Console.WriteLine(" ");
            Console.ReadLine();
        }
    }
}
