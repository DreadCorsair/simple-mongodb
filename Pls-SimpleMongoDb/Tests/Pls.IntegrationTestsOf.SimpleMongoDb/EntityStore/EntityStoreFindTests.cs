using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pls.IntegrationTestsOf.SimpleMongoDb.TestModel;
using Pls.SimpleMongoDb;
using Pls.SimpleMongoDb.Querying;

namespace Pls.IntegrationTestsOf.SimpleMongoDb.EntityStore
{
    [TestClass]
    public class EntityStoreFindTests
        : TestBase
    {
        private const string DbName = Constants.TestDbName;

        [TestMethod]
        public void Find_UsingJsonWhereOperator_ReturnsTwoOfThree()
        {
            var documents = new[]
                            {
                                new Person {Name = "Daniel", Age = 29},
                                new Person {Name = "Adam", Age = 55},
                                new Person {Name = "Sue", Age = 55},
                            };
            TestHelper.InsertDocuments(Constants.Collections.PersonsCollectionName, documents);

            var cn = TestHelper.CreateConnection();
            using (var session = new SimoSession(cn))
            {
                var entityStore = new SimoEntityStore(session, DbName);

                var persons = entityStore.Find<Person>(@"{$where : ""this.Name == 'Daniel' || this.Name == 'Sue'""}");

                var danielAndSueFound = persons.Where(p => new[] { "Daniel", "Sue" }.Contains(p.Name)).Count() == 2;
                Assert.AreEqual(2, persons.Count);
                Assert.IsTrue(danielAndSueFound);
            }
        }

        [TestMethod]
        public void Find_UsingWhereOperator_ReturnsTwoOfThree()
        {
            var documents = new[]
                            {
                                new Person {Name = "Daniel", Age = 29},
                                new Person {Name = "Adam", Age = 55},
                                new Person {Name = "Sue", Age = 55},
                            };
            TestHelper.InsertDocuments(Constants.Collections.PersonsCollectionName, documents);

            var cn = TestHelper.CreateConnection();
            using (var session = new SimoSession(cn))
            {
                var entityStore = new SimoEntityStore(session, DbName);

                var persons = entityStore.Find<Person>(Query.New(q => q.Where(@"this.Name == 'Daniel' || this.Name == 'Sue'")));

                var danielAndSueFound = persons.Where(p => new[] { "Daniel", "Sue" }.Contains(p.Name)).Count() == 2;
                Assert.AreEqual(2, persons.Count);
                Assert.IsTrue(danielAndSueFound);
            }
        }

        [TestMethod]
        public void Find_UsingJsonInOperator_ReturnsTwoOfThree()
        {
            var documents = new[]
                            {
                                new Person {Name = "Daniel", Age = 29},
                                new Person {Name = "Adam", Age = 55},
                                new Person {Name = "Sue", Age = 55},
                            };
            TestHelper.InsertDocuments(Constants.Collections.PersonsCollectionName, documents);

            var cn = TestHelper.CreateConnection();
            using (var session = new SimoSession(cn))
            {
                var entityStore = new SimoEntityStore(session, DbName);

                var persons = entityStore.Find<Person>(@"{Name : { $in : [""Daniel"", ""Sue""] } }");

                var danielAndSueFound = persons.Where(p => new[] { "Daniel", "Sue" }.Contains(p.Name)).Count() == 2;
                Assert.AreEqual(2, persons.Count);
                Assert.IsTrue(danielAndSueFound);
            }
        }

        [TestMethod]
        public void Find_UsingInOperator_ReturnsTwoOfThree()
        {
            var documents = new[]
                            {
                                new Person {Name = "Daniel", Age = 29},
                                new Person {Name = "Adam", Age = 55},
                                new Person {Name = "Sue", Age = 55},
                            };
            TestHelper.InsertDocuments(Constants.Collections.PersonsCollectionName, documents);

            var cn = TestHelper.CreateConnection();
            using(var session = new SimoSession(cn))
            {
                var entityStore = new SimoEntityStore(session, DbName);

                var persons = entityStore.Find<Person>(Query.New(q => q["Name"].In("Daniel", "Sue")));

                var danielAndSueFound = persons.Where(p => new[] { "Daniel", "Sue" }.Contains(p.Name)).Count() == 2;
                Assert.AreEqual(2, persons.Count);
                Assert.IsTrue(danielAndSueFound);
            }
        }
    }
}