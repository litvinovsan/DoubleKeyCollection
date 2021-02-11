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
            var collection = new MultiCollection<IdKey, NameKey, Person>
                {
                    {new IdKey(4), new NameKey("Tom"), new Person(4,"some info")},
                    {new IdKey(1), new NameKey("Alex"), new Person()},
                    {new IdKey(2), new NameKey("Alex"), new Person()},
                    {new IdKey(3), new NameKey("Hans"), new Person()},
                    {new IdKey(3), new NameKey("Jerry"), new Person()},
                    {new IdKey(9), new NameKey("Spike"), new Person()},
                    {new IdKey(4), new NameKey("Ford"), new Person()}

                };


            // Перебор колекции 
            foreach (var item in collection)
            {
                Console.WriteLine($"{item.Id}, {item.Name},  {item.Value}");
            }

            Console.WriteLine($"\n\rКоличество записей: {collection.Count}");

            // Метод Contains для Id
            var expectedId = new IdKey(4);
            var bResult = collection.Contains(expectedId);
            Console.WriteLine($"\n\rСодержит '{expectedId}' ? => '{bResult}'\n\r");

            // Метод Contains для Name
            var expectedName = new NameKey("Jerry");
            bResult = collection.Contains(expectedName);
            Console.WriteLine($"\n\nСодержит '{expectedName}' ? => {bResult}\n\r");

            // Метод Contains для Value
            var expectedValue = new Person(4, "ABC");
            bResult = collection.Contains(expectedValue);
            Console.WriteLine($"Содержит '{expectedValue}' значение? => {bResult}\n\r");

            // Индексатор  Id - Name
            var myId = new IdKey(4);
            var myName = new NameKey("Tom");
            var value = collection[myId, myName];
            Console.WriteLine($"\n\rПолучение значения через индексатор ['{myId}','{myName}'] =>  {value}\n\r");

            // Индексатор Name
            Console.WriteLine($"\n\rСписок записей c полем '{myName}': ");
            var entrs = collection[myName];
            foreach (var entr in entrs)
            {
                Console.WriteLine($"'{entr.Key.Id }' - '{entr.Value}'");
            }

            // Индексатор Id
            Console.WriteLine($"\n\rСписок записей у которых есть '{myId}':");
            var etriesById = collection[myId];
            foreach (var entr in etriesById)
            {
                Console.WriteLine($"'{entr.Key.Name}'  '{entr.Key.Id}'");
            }

            // Попытка получения значения по ключу
            Person p = new Person();
            bResult = collection.TryGetValue(myId, myName, ref p);
            Console.WriteLine($"\n\rПолучить значение по двум ключам '{myId.Id} {myName.Name}': => {p} => {bResult}\n\r");

            // Попытка получения значения только по Id.  
            var myId2 = new IdKey(9);
            var myName2 = new NameKey("Enik");

            collection.Add(myId2, myName2, new Person(myId2.Id, myName2.Name));

            bResult = collection.TryGetValue(myId2, ref p);
            Console.WriteLine($"Получить значение {myId2}: => {p} => {bResult}");

            Console.WriteLine(" ");
            Console.ReadLine();
        }
    }
}
