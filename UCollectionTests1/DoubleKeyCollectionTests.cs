using UCollection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace UCollection.Tests
{
    [TestClass()]
    public class DoubleKeyCollectionTests
    {
        #region /// Initialization
        public class Person
        {
            public int Id { get; set; }
            public string Name { get; }
            private DateTime BirthDay { get; }

            public Person()
            {
                Id = IdCnt++;
                Name = Guid.NewGuid().ToString();
                BirthDay = DateTime.Now;
            }

            public Person(int id, string name)
            {
                Id = id;
                Name = name;
                BirthDay = DateTime.UtcNow;
            }

            public static int IdCnt = 0;
        }

        public DoubleKeyCollection<int, string, Person> TestCollection;

        public Person Person1;
        public Person Person2;
        public Person Person3;

        [TestInitialize]
        public void Setup()
        {
            TestCollection = new DoubleKeyCollection<int, string, Person>();

            Person1 = new Person();
            Person2 = new Person();
            Person3 = new Person();

            TestCollection.Add(Person1.Id, Person1.Name, Person1);
            TestCollection.Add(Person2.Id, Person2.Name, Person2);
            TestCollection.Add(Person3.Id, Person3.Name, Person3);
        }

        [TestCleanup]
        public void Clean()
        {
            TestCollection.Clear();
            Person.IdCnt = 0;
        }


        [TestMethod()]
        public void ClearTest()
        {
            var expected = 0;

            //Action
            var p = new Person();
            TestCollection.Add(p.Id, p.Name, p);
            TestCollection.Clear();
            var actual = TestCollection.Count;

            //Assert
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region /// Add Values Tests

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
            var Val = DateTime.Now;

            // act
            collection.Add(testId, testName, Val);
            collection.Add(testId, testName, Val);
            // assert
            Assert.AreEqual(collection.Values.ToList()[0], Val);
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
            var id = Person1.Id;
            var p1 = new Person() { Id = id };
            var p2 = new Person() { Id = id };
            var p3 = new Person() { Id = id };

            //Action
            TestCollection.Add(p1.Id, p1.Name, p1);
            TestCollection.Add(p2.Id, p2.Name, p2);
            TestCollection.Add(p3.Id, p3.Name, p3);

            var expectedCnt = TestCollection.Keys.Where(x => x.Item1 .Equals(id)).Select(y => y).ToArray().Length;
            var result = TestCollection[id].Length;
            //Assert
            Assert.AreEqual(expectedCnt, result);
        }

        #endregion

        #region /// Get Values Tests

        [TestMethod()]
        public void Get_Value_by_Full_Key_Test()
        {
            //Arrange
            var id = Person1.Id;
            var name = Person1.Name;

            //Action
            var result = TestCollection[id, name];
            //Assert
            Assert.AreEqual(Person1, result);
        }

        [TestMethod()]
        public void Key_NotFoundException_Test()
        {
            //Arrange
            var id = Person1.Id;

            //Assert
            Assert.ThrowsException<DoubleKeyCollection<int, string, Person>.KeyNotExistsException>(() => TestCollection[id, "Dummy"]);
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
            Assert.AreEqual(2, resultId.Length);
            var resultName = dcCollection[name: 75];
            Assert.AreEqual(1, resultName.Length);
        }

        [TestMethod()]
        public void ContainsValueTest()
        {
            // Arrange
            var expectedPerson = new Person();
            var notExpectedPerson = new Person();
            TestCollection.Add(expectedPerson.Id, expectedPerson.Name, expectedPerson);

            // Action
            var result = TestCollection.Contains(expectedPerson);
            var resultNotExpected = TestCollection.Contains(notExpectedPerson);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(resultNotExpected);
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
                TestCollection.Add(person.Id, person.Name, person);
            }

            var expectedPerson = new Person();
            var id = expectedPerson.Id;
            var name = expectedPerson.Name;

            TestCollection.Add(expectedPerson.Id, expectedPerson.Name, expectedPerson);

            for (int i = 0; i < 100000; i++)
            {
                var person = new Person();
                TestCollection.Add(person.Id, person.Name, person);
            }

            //Action
            stopWatch.Start();
            var result = TestCollection[id, name];
            stopWatch.Stop();

            var time = stopWatch.Elapsed.TotalMilliseconds; // Key Value TotalMilliseconds = 0.6987
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
                TestCollection.Add(person.Id, person.Name, person);
            }

            var expectedPerson = new Person();
            var expectedPersonId = expectedPerson.Id;

            // Init Expected values
            for (int i = 0; i < 15; i++)
            {
                var p = new Person();
                TestCollection.Add(expectedPersonId, p.Name, p);
            }

            // Init random entries
            for (int i = 0; i < 100000; i++)
            {
                var person = new Person();
                TestCollection.Add(person.Id, person.Name, person);
            }

            // Init Expected values
            for (int i = 0; i < 15; i++)
            {
                var p = new Person();
                TestCollection.Add(expectedPersonId, p.Name, p);
            }

            //Action
            stopWatch.Start();
            var result = TestCollection[expectedPersonId];
            var actualdLength = result.Length;

            stopWatch.Stop();

            var time = stopWatch.Elapsed.TotalMilliseconds; // Key Value         92.66
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
                TestCollection.Add(person.Id, person.Name, person);
            }

            var expectedPerson = new Person();
            var expectedName = expectedPerson.Name;

            // Init Expected values
            for (int i = 0; i < 15; i++)
            {
                var p = new Person();
                TestCollection.Add(p.Id, expectedName, p);
            }

            // Init random entries
            for (int i = 0; i < 100000; i++)
            {
                var person = new Person();
                TestCollection.Add(person.Id, person.Name, person);
            }

            // Init Expected values
            for (int i = 0; i < 15; i++)
            {
                var p = new Person();
                TestCollection.Add(p.Id, expectedName, p);
            }

            //Action
            stopWatch.Start();
            var result = TestCollection[expectedName];
            var actualdLength = result.Length;

            stopWatch.Stop();

            var time = stopWatch.Elapsed.TotalMilliseconds; // Key Value 21.032 
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
            public string Name { get; set; }

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
            var actualdLength = result.Length;

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




        [TestMethod()]
        public void RemoveTest()
        {
            Assert.Fail();
        }
       
      
    }
}