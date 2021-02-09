using UCollection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace UCollection.Tests
{
    [TestClass()]
    public class DoubleKeyCollectionTests
    {
        #region /// Initialization

        private class Person
        {
            public int Id { get; set; }
            public string Name { get; }

            public Person()
            {
                Id = IdCnt++;
                Name = Guid.NewGuid().ToString();
            }

            public Person(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public static int IdCnt = 0;
        }

        private DoubleKeyCollection<int, string, Person> _testCollection;

        private Person _person1;
        private Person _person2;
        private Person _person3;

        [TestInitialize]
        public void Setup()
        {
            _testCollection = new DoubleKeyCollection<int, string, Person>();

            _person1 = new Person();
            _person2 = new Person();
            _person3 = new Person();

            _testCollection.Add(_person1.Id, _person1.Name, _person1);
            _testCollection.Add(_person2.Id, _person2.Name, _person2);
            _testCollection.Add(_person3.Id, _person3.Name, _person3);
        }

        [TestCleanup]
        public void Clean()
        {
            _testCollection.Clear();
            Person.IdCnt = 0;
        }

        [TestMethod()]
        public void ClearTest()
        {
            var expected = 0;

            //Action
            var p = new Person();
            _testCollection.Add(p.Id, p.Name, p);
            _testCollection.Clear();
            var actual = _testCollection.Count;

            //Assert
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region /// Add and Remove Tests

        #region /// "Add" Method
        [TestMethod()]
        [Timeout(2000)]
        public void AddTest()
        {
            // arrange
            DoubleKeyCollection<int, int, int> collection = new DoubleKeyCollection<int, int, int>();
            int numCnt = 100;

            // act
            for (int i = 0; i < numCnt; i++)
            {
                collection.Add(i, i, i);
            }

            // assert
            Assert.AreEqual(collection.Count, numCnt);
        }

        [TestMethod()]
        [ExpectedException(typeof(DoubleKeyCollection<int, string, DateTime>.KeyAlreadyExistsException))]
        public void Add_DuplicateTest()
        {
            // arrange
            DoubleKeyCollection<int, string, DateTime> collection = new DoubleKeyCollection<int, string, DateTime>();

            int testId = 1;
            string testName = Guid.NewGuid().ToString();
            var val = DateTime.Now;

            // act
            collection.Add(testId, testName, val);
            collection.Add(testId, testName, val);
            // assert
            Assert.AreEqual(collection.Values.ToList()[0], val);
        }

        [TestMethod()]
        public void Add_ArgNullTest()
        {
            // arrange
            DoubleKeyCollection<int, string, Person> collection = new DoubleKeyCollection<int, string, Person>();

            // assert
            Assert.ThrowsException<ArgumentNullException>(() => collection.Add(1, null, null));
        }

        #endregion

        [TestMethod()]
        public void Same_Id_Test()
        {
            //Arrange
            var id = _person1.Id;
            var p1 = new Person() { Id = id };
            var p2 = new Person() { Id = id };
            var p3 = new Person() { Id = id };

            //Action
            _testCollection.Add(p1.Id, p1.Name, p1);
            _testCollection.Add(p2.Id, p2.Name, p2);
            _testCollection.Add(p3.Id, p3.Name, p3);

            var expectedCnt = _testCollection.Keys.Where(x => x.Item1.Equals(id)).Select(y => y).ToArray().Length;
            var result = _testCollection[id].Count();
            //Assert
            Assert.AreEqual(expectedCnt, result);
        }


        [TestMethod()]
        public void RemoveTest()
        {
            // Arrange 
            var expectedId = _person2.Id;
            var expectedName = _person2.Name;

            // Action
            var isContains = _testCollection.Contains(expectedId, expectedName);
            Assert.IsTrue(isContains);

            var result = _testCollection.Remove(expectedId, expectedName);
            Assert.IsTrue(result);

            isContains = _testCollection.Contains(expectedId, expectedName);
            Assert.IsFalse(isContains);
        }

        [TestMethod()]
        public void Remove_No_Key_Test()
        {
            // Arrange 
            var expectedId = _person2.Id;
            var expectedName = "dummy";

            // Action
            var isContains = _testCollection.Contains(expectedId, expectedName);
            Assert.IsFalse(isContains);

            var result = _testCollection.Remove(expectedId, expectedName);
            Assert.IsFalse(result);

            isContains = _testCollection.Contains(expectedId, expectedName);
            Assert.IsFalse(isContains);
        }
        #endregion

        #region /// Get Values Tests

        [TestMethod()]
        public void Foreach_Test()
        {
            //Arrange
            List<int> idListExpected = new List<int>() { _person1.Id, _person2.Id, _person3.Id };
            List<string> nameListExpected = new List<string>() { _person1.Name, _person2.Name, _person3.Name };

            //Action
            List<int> idListResult = new List<int>();
            List<string> nameListResult = new List<string>();
            foreach (var entry in _testCollection)
            {
                idListResult.Add(entry.Id);
                nameListResult.Add(entry.Name);
            }
            //Assert
            Assert.IsTrue(idListExpected.Count == idListResult.Count);
            Assert.IsTrue(nameListExpected.Count == nameListResult.Count);
            Assert.IsTrue(idListExpected[1] == idListResult[1]);
            Assert.IsTrue(nameListExpected[0] == nameListResult[0]);
        }

        [TestMethod()]
        public void TryGetValueTest()
        {
            var person = new Person();
            var result = _testCollection.TryGetValue(_person1.Id, _person1.Name, out person);
            Assert.IsTrue(result);
            Assert.AreEqual(person, _person1);
        }

        [TestMethod()]
        public void TryGetValue_No_entry_Test()
        {
            var person = new Person();
            var result = _testCollection.TryGetValue(person.Id, person.Name, out person);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void Get_Value_by_Full_Key_Test()
        {
            //Arrange
            var id = _person1.Id;
            var name = _person1.Name;

            //Action
            var result = _testCollection[id, name];
            //Assert
            Assert.AreEqual(_person1, result);
        }

        [TestMethod()]
        public void Key_NotFoundException_Test()
        {
            //Arrange
            var id = _person1.Id;

            //Assert
            Assert.ThrowsException<DoubleKeyCollection<int, string, Person>.KeyNotExistsException>(() => _testCollection[id, "Dummy"]);
        }

        [TestMethod()]
        public void Identical_Types_In_Key_Test()
        {
            DoubleKeyCollection<int, int, Person> dcCollection = new DoubleKeyCollection<int, int, Person>();

            dcCollection.Add(75, 75, new Person());
            dcCollection.Add(17, 21, new Person());
            dcCollection.Add(11, 2, new Person());
            dcCollection.Add(75, 4, new Person());

            var resultId = dcCollection[id: 75];
            Assert.AreEqual(2, resultId.Count());
            var resultName = dcCollection[name: 75];
            Assert.AreEqual(1, resultName.Count());
        }

        [TestMethod()]
        public void ContainsValueTest()
        {
            // Arrange
            var expectedPerson = new Person();
            var notExpectedPerson = new Person();
            _testCollection.Add(expectedPerson.Id, expectedPerson.Name, expectedPerson);

            // Action
            var result = _testCollection.Contains(expectedPerson);
            var resultNotExpected = _testCollection.Contains(notExpectedPerson);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(resultNotExpected);
        }

        [TestMethod()]
        public void Contains_Id_Name_Test()
        {
            // Arrange
            var testPerson = new Person();
            _testCollection.Add(testPerson.Id, testPerson.Name, testPerson);

            // Actions
            var actual = _testCollection.Contains(testPerson.Id, testPerson.Name);
            _testCollection.Contains(testPerson);

            // Assert
            Assert.IsTrue(_testCollection.Contains(testPerson));
            Assert.IsTrue(actual);
        }

        [TestMethod()]
        public void Contains_Id_Test()
        {
            // Arrange 
            var expId = _person1.Id;

            // Action
            var result = _testCollection.Contains(expId);
            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void Contains_Name_Test()
        {
            // Arrange 
            var expName = _person1.Name;

            // Action
            var result = _testCollection.Contains(expName);
            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(_testCollection.Contains("Dummy"));
        }

        #endregion

        #region /// Speed Tests
        [TestMethod()]
        public void SpeedTest_Get_by_Id_Name_Large_Dict_Test()
        {
            //Arrange
            Stopwatch stopWatch = new Stopwatch();

            for (int i = 0; i < 100000; i++)
            {
                var person = new Person();
                _testCollection.Add(person.Id, person.Name, person);
            }

            var expectedPerson = new Person();
            var id = expectedPerson.Id;
            var name = expectedPerson.Name;

            _testCollection.Add(expectedPerson.Id, expectedPerson.Name, expectedPerson);

            for (int i = 0; i < 100000; i++)
            {
                var person = new Person();
                _testCollection.Add(person.Id, person.Name, person);
            }

            //Action
            stopWatch.Start();
            var result = _testCollection[id, name];
            stopWatch.Stop();

            var unused = stopWatch.Elapsed.TotalMilliseconds; // Key Value TotalMilliseconds = 0.6987
                                                              // Key Struct time=1.0098  
                                                              // Key Struct std   1.192
                                                              // ValueTuple 0.98
                                                              // Tuple 0.46
                                                              //Assert
            Assert.AreEqual(expectedPerson, result);
        }

        [TestMethod()]
        public void SpeedTest_Get_Values_by_Id_Large_Dict_Test()
        {
            //Arrange
            Stopwatch stopWatch = new Stopwatch();

            // Init random entries
            for (int i = 0; i < 100000; i++)
            {
                var person = new Person();
                _testCollection.Add(person.Id, person.Name, person);
            }

            var expectedPerson = new Person();
            var expectedPersonId = expectedPerson.Id;

            // Init Expected values
            for (int i = 0; i < 15; i++)
            {
                var p = new Person();
                _testCollection.Add(expectedPersonId, p.Name, p);
            }

            // Init random entries
            for (int i = 0; i < 100000; i++)
            {
                var person = new Person();
                _testCollection.Add(person.Id, person.Name, person);
            }

            // Init Expected values
            for (int i = 0; i < 15; i++)
            {
                var p = new Person();
                _testCollection.Add(expectedPersonId, p.Name, p);
            }

            //Action
            stopWatch.Start();
            var result = _testCollection[expectedPersonId];
            var actualdLength = result.Count();

            stopWatch.Stop();

            var unused = stopWatch.Elapsed.TotalMilliseconds; // Key Value         92.66
                                                              // Key Struct time   84.4   
                                                              // Key Struct std    20.3
                                                              // ValueTuple 98.9
                                                              // Tuple      33.8
                                                              //Assert
            Assert.AreEqual(30, actualdLength);
        }

        [TestMethod()]
        public void SpeedTest_Get_Values_by_Name_Large_Dict_Test()
        {
            //Arrange
            Stopwatch stopWatch = new Stopwatch();

            // Init random entries
            for (int i = 0; i < 100000; i++)
            {
                var person = new Person();
                _testCollection.Add(person.Id, person.Name, person);
            }

            var expectedPerson = new Person();
            var expectedName = expectedPerson.Name;

            // Init Expected values
            for (int i = 0; i < 15; i++)
            {
                var p = new Person();
                _testCollection.Add(p.Id, expectedName, p);
            }

            // Init random entries
            for (int i = 0; i < 100000; i++)
            {
                var person = new Person();
                _testCollection.Add(person.Id, person.Name, person);
            }

            // Init Expected values
            for (int i = 0; i < 15; i++)
            {
                var p = new Person();
                _testCollection.Add(p.Id, expectedName, p);
            }

            //Action
            stopWatch.Start();
            var result = _testCollection[expectedName];
            var actualdLength = result.Count();

            stopWatch.Stop();

            var unused = stopWatch.Elapsed.TotalMilliseconds; // Key Value 21.032 
                                                              // Key Struct 27.6
                                                              // KeyStruct std 17
                                                              // ValueTuple 33
                                                              // Tuple 13
                                                              //Assert
            Assert.AreEqual(30, actualdLength);
        }

        #region ReferenceType Id and Name
        class TestKeyName
        {
            private string Name { get; set; }

            public TestKeyName()
            {
                Name = Guid.NewGuid().ToString();
            }
        }

        class TestKeyId
        {
            public int Id { get; set; }

            public TestKeyId()
            {
                Id = new Random().Next(65534);
            }
        }

        [TestMethod()]
        public void SpeedTest_Reference_Type_Key_Get_Value_Test()
        {
            //Arrange
            Stopwatch stopWatch = new Stopwatch();
            var collection = new DoubleKeyCollection<TestKeyId, TestKeyName, Person>();

            // Init random entries
            for (int i = 0; i < 100000; i++)
            {
                var person = new Person();
                collection.Add(new TestKeyId(), new TestKeyName(), person);
            }

            var expectedPerson = new Person();
            var expectedName = new TestKeyName();

            // Init Expected values
            for (int i = 0; i < 15; i++)
            {
                var p = new Person();
                collection.Add(new TestKeyId(), expectedName, p);
            }

            // Init random entries
            for (int i = 0; i < 100000; i++)
            {
                var person = new Person();
                collection.Add(new TestKeyId(), new TestKeyName(), person);
            }

            // Init Expected values
            for (int i = 0; i < 15; i++)
            {
                var p = new Person();
                collection.Add(new TestKeyId(), expectedName, p);
            }

            //Action
            stopWatch.Start();
            var result = collection[expectedName];
            var actualdLength = result.Count();

            stopWatch.Stop();

            var time = stopWatch.Elapsed.TotalMilliseconds; // Key Value  
                                                            // Key Struct     32.53
                                                            // Key Struct std 26.0053   
                                                            // ValueTuple 24.7
                                                            // Tuple 14
                                                            //Assert
            Assert.AreEqual(30, actualdLength);
        }
        #endregion

        #endregion
    }
}