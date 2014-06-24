using Arango.Client;
using Arango.Client.API;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arango.Tests.SimpleQueryTests
{
    public class SimpleQueryTest : IDisposable
    {
        public SimpleQueryTest()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
        }
        
        [Test()]
        public void Should_return_simple_all()
        {
            var docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();


            var result = db.SimpleQuery.All(new SimpleQueryOperation.AllRequest
                {
                    Collection = Database.TestDocumentCollectionName
                });

            Assert.AreEqual(6, result.Count);

        }

        [Test()]
        public void Should_return_simple_by_example()
        {
            var docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();


            var result = db.SimpleQuery.ByExample(new SimpleQueryOperation.ByExampleRequest
            {
                Collection = Database.TestDocumentCollectionName,
                Example = new { bar = 12345 }
            });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(12345, result[0].Int("bar"));
            Assert.AreEqual("foo string value 1", result[0].String("foo"));
        }

        [Test()]
        public void Should_return_simple_by_example_subdocument()
        {
            var docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();


            var result = db.SimpleQuery.ByExample(new SimpleQueryOperation.ByExampleRequest
            {
                Collection = Database.TestDocumentCollectionName,
                Example = new Foo { AJ = 1 }
            });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(54321, result[0].Int("bar"));
            Assert.AreEqual("foo string value 6", result[0].String("foo"));

        }

        [Test()]
        public void Should_return_simple_first_example()
        {
            var docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();


            var result = db.SimpleQuery.FirstExample(new SimpleQueryOperation.ByExampleRequest
            {
                Collection = Database.TestDocumentCollectionName,
                Example = new { bar = 54321 }
            });

            Assert.AreEqual(54321, result.Int("bar"));

        }

        public class Foo
        {
            [JsonProperty("a.j")]
            public int AJ { get; set; }
        }

        private List<Document> CreateDummyDocuments()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();

            var docs = new List<Document>();

            // create test documents
            var doc1 = new Document()
                .String("foo", "foo string value 1")
                .Int("bar", 12345);

            var doc2 = new Document()
                .String("foo", "foo string value 2")
                .Int("bar", 54321);

            var doc3 = new Document()
                .String("foo", "foo string value 3")
                .Int("bar", 54321);

            var doc4 = new Document()
                .String("foo", "foo string value 4")
                .Int("bar", 54321);

            var doc5 = new Document()
                .String("foo", "foo string value 5")
                .Int("bar", 54321);

            var doc6 = new Document()
                .String("foo", "foo string value 6")
                .Int("bar", 54321);
            
            doc6.Add("a", new {j = 1});

            docs.Add(doc1);
            docs.Add(doc2);
            docs.Add(doc3);
            docs.Add(doc4);
            docs.Add(doc5);
            docs.Add(doc6);

            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            db.Document.Create(Database.TestDocumentCollectionName, doc3);
            db.Document.Create(Database.TestDocumentCollectionName, doc4);
            db.Document.Create(Database.TestDocumentCollectionName, doc5);
            db.Document.Create(Database.TestDocumentCollectionName, doc6);

            return docs;
        }

        public void Dispose()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
