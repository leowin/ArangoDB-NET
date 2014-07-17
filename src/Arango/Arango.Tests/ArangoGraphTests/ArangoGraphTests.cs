using System;
using System.Collections.Generic;
using System.Linq;
using Arango.Client.Protocol;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.ArangoGraphTests
{
    [TestFixture()] 
    public class ArangoGraphTests : IDisposable
    {
        ArangoDatabase db;
        
        [SetUp()]
        public void Init()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            db = Database.GetTestDatabase();
        }
        
        [TearDown()]
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
        
        
        [Test()]
        public void Should_get_all_graphs()
        {
            List<String> vertexCollections = new List<String>();
            vertexCollections.Add("vc1");
            vertexCollections.Add("vc2");
            
            List<ArangoGraphEdgeDefinition> eds = new List<ArangoGraphEdgeDefinition>();
            ArangoGraphEdgeDefinition ed1 = new ArangoGraphEdgeDefinition(
               "ed1",
               new List<String>{"f1", "f2"},
               new List<String>{"t1", "t2"}
               );
            eds.Add(ed1);
            db.Graph.Create("UnitTestGraph1", eds, vertexCollections);
            db.Graph.Create("UnitTestGraph2");
            var result = db.Graph.Get();
            Assert.AreEqual(result.Count, 2);
        }
      
        [Test()]
        public void Should_get_graph_by_name()
        {
            List<String> vertexCollections = new List<String>();
            vertexCollections.Add("vc1");
            vertexCollections.Add("vc2");
            vertexCollections.Add("t1");
            
            List<ArangoGraphEdgeDefinition> eds = new List<ArangoGraphEdgeDefinition>();
            ArangoGraphEdgeDefinition ed1 = new ArangoGraphEdgeDefinition(
                "ed1",
                new List<String>{"f1", "f2"},
                new List<String>{"t1", "t2"}
               );
            eds.Add(ed1);
            string name = "UnitTestGraph";
            db.Graph.Create(name, eds, vertexCollections);
            ArangoGraph graph = db.Graph.Get(name);
            Assert.AreEqual(graph.Name, name);
            Assert.AreEqual(graph.edgeDefinitions.First().collection, ed1.collection);
            Assert.AreEqual(graph.edgeDefinitions.First().from, ed1.from);
            Assert.AreEqual(graph.edgeDefinitions.First().to, ed1.to);
//            Assert.AreEqual(2, graph.vertexCollections.Count);

        }

        [Test()]
        public void Should_create_and_get_empty_graph()
        {
            string name = "UnitTestGraph";
            db.Graph.Create(name);
            ArangoGraph graph = db.Graph.Get(name);
            Assert.AreEqual(name, graph.Name);
            Assert.AreEqual(0, graph.edgeDefinitions.Count);
            Assert.AreEqual(0, graph.orphanCollections.Count);

        }
        
        [Test()]
        public void Should_create_and_get_graph_with_orphanage()
        {
            List<string> orphanage = new List<string>{"v1"};
            string name = "UnitTestGraph";
            db.Graph.Create(name, orphanage);
            ArangoGraph graph = db.Graph.Get(name);
            Assert.AreEqual(name, graph.Name);
            Assert.AreEqual(0, graph.edgeDefinitions.Count);
            Assert.AreEqual(1, graph.orphanCollections.Count);

        }
        
        [Test()]
        public void Should_create_and_get_graph_with_edge_definition()
        {
            ArangoGraphEdgeDefinition ed = new ArangoGraphEdgeDefinition(
                "unitTestEdge",
                new List<string>{"unitTestFrom"},
                new List<string>{"unitTestTo"}
            );
            List<ArangoGraphEdgeDefinition> edgeDefinitions = new List<ArangoGraphEdgeDefinition>{ed};
            string name = "UnitTestGraph";
            db.Graph.Create(name, edgeDefinitions);
            ArangoGraph graph = db.Graph.Get(name);
            Assert.AreEqual(name, graph.Name);
            Assert.AreEqual(1, graph.edgeDefinitions.Count);
            Assert.AreEqual(0, graph.orphanCollections.Count);

        }
        
        [Test()]
        public void Should_delete_existing_graph()
        {
            string g1 = "UnitTestGraph1";
            string g2 = "UnitTestGraph2";
            string g3 = "UnitTestGraph3";
            db.Graph.Create(g1);
            db.Graph.Create(g2);
            db.Graph.Create(g3);
            var result = db.Graph.Get();
            Assert.AreEqual(3, result.Count);
            db.Graph.Delete(g1);
            result = db.Graph.Get();
            Assert.AreEqual(2, result.Count);
            db.Graph.Delete(g2);
            result = db.Graph.Get();
            Assert.AreEqual(1, result.Count);
            db.Graph.Delete(g3);
            result = db.Graph.Get();
            Assert.AreEqual(0, result.Count);
        }
      
        [Test()]
        public void Should_delete_existing_graph_and_drop_collections()
        {
            string v11 = "v11";
            string v12 = "v12";
            string t11 = "t11";
            string t12 = "t12";
            string f11 = "f11";
            string f12 = "f12";
            string ed1 = "ed1";
            string name1 = "UnitTestGraph1";
            string ed2 = "ed2";
            string name2 = "UnitTestGraph2";
            
            ArangoDatabase db = Database.GetTestDatabase();
            
            //Graph 1
            List<ArangoGraphEdgeDefinition> eds1 = new List<ArangoGraphEdgeDefinition>();
            ArangoGraphEdgeDefinition edgeDef1 = new ArangoGraphEdgeDefinition(
                ed1,
                new List<String>{f11, f12},
                new List<String>{t11, t12}
            );
            eds1.Add(edgeDef1);
            List<String> vertexCollections1 = new List<String>();
            vertexCollections1.Add(v11);
            vertexCollections1.Add(v12);
            db.Graph.Create(name1, eds1, vertexCollections1);
            
            //Graph 2
            List<ArangoGraphEdgeDefinition> eds2 = new List<ArangoGraphEdgeDefinition>();
            ArangoGraphEdgeDefinition edgeDef2 = new ArangoGraphEdgeDefinition(
                ed2,
                new List<String>{f11},
                new List<String>{t12}
            );
            eds2.Add(edgeDef2);
            List<String> vertexCollections2 = new List<String>();
            vertexCollections2.Add(v12);
            db.Graph.Create(name2, eds2, vertexCollections2);
            
            Assert.AreEqual(2, db.Graph.Get().Count);
            Assert.True(db.Graph.Exists(name1));
            Assert.True(db.Graph.Exists(name2));
            Assert.True(db.Collection.Exists(ed1));
            Assert.True(db.Collection.Exists(ed2));
            Assert.True(db.Collection.Exists(f11));
            Assert.True(db.Collection.Exists(f12));
            Assert.True(db.Collection.Exists(t11));
            Assert.True(db.Collection.Exists(t12));
            Assert.True(db.Collection.Exists(v11));
            Assert.True(db.Collection.Exists(v12));
            db.Graph.Delete(name1, true);
            Assert.False(db.Graph.Exists(name1));
            Assert.True(db.Graph.Exists(name2));
            Assert.AreEqual(1, db.Graph.Get().Count);
            Assert.False(db.Collection.Exists(ed1));
            Assert.True(db.Collection.Exists(ed2));
            Assert.True(db.Collection.Exists(f11));
            Assert.False(db.Collection.Exists(f12));
            Assert.False(db.Collection.Exists(t11));
            Assert.True(db.Collection.Exists(t12));
            Assert.False(db.Collection.Exists(v11));
            Assert.True(db.Collection.Exists(v12));
        }
      
    }
    
    [TestFixture()]
    public class ArangoGraphGraphTests : IDisposable
    {
        string graphName = "UnitTestGraph";
        string edgeCol1 = "UnitTestEdgeCollection1";
        string edgeCol2 = "UnitTestEdgeCollection2";
        string fromCol1 = "UnitTestFromCollection1";
        string fromCol2 = "UnitTestFromCollection2";
        string fromCol3 = "UnitTestFromCollection3";
        string toCol1 = "UnitTestToCollection1";
        string toCol2 = "UnitTestToCollection2";
        string toCol3 = "UnitTestToCollection3";
        string vertexCol1 = "UnitTestVertex1";
        string vertexCol2 = "UnitTestVertex2";
        string vertexCol3 = "UnitTestVertex3";
        ArangoGraphEdgeDefinition ed1;
        ArangoGraphEdgeDefinition ed2;
        ArangoDatabase db;
        ArangoGraph g;
        
        [SetUp()] public void Init()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            db = Database.GetTestDatabase();
            List<ArangoGraphEdgeDefinition> eds = new List<ArangoGraphEdgeDefinition>();
            ed1 = new ArangoGraphEdgeDefinition(
                edgeCol1,
                new List<String>{fromCol1, fromCol2},
                new List<String>{toCol1, toCol2}
            );
            eds.Add(ed1);
            ed2 = new ArangoGraphEdgeDefinition(
                edgeCol2,
                new List<String>{fromCol2, fromCol3},
                new List<String>{toCol2, toCol3}
            );
            eds.Add(ed2);
            List<String> vertexCollections = new List<String>();
            vertexCollections.Add(vertexCol1);
            vertexCollections.Add(vertexCol2);
            vertexCollections.Add(vertexCol3);
            g = db.Graph.Create(graphName, eds, vertexCollections);
        }
        
        [TearDown()] public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
        
        
        [Test()]
        public void Should_get_all_vertex_collections()
        {
            List<string> result = g.vertexCollections();
            Assert.False(result.Contains(edgeCol1));
            Assert.False(result.Contains(edgeCol2));
            Assert.True(result.Contains(fromCol1));
            Assert.True(result.Contains(fromCol2));
            Assert.True(result.Contains(fromCol3));
            Assert.True(result.Contains(toCol1));
            Assert.True(result.Contains(toCol2));
            Assert.True(result.Contains(toCol3));
            Assert.True(result.Contains(vertexCol1));
            Assert.True(result.Contains(vertexCol2));
            Assert.True(result.Contains(vertexCol3));
            Assert.AreEqual(9, result.Count());
        }
        
        [Test()]
        public void Should_add_a_vertex_collection()
        {
            string name = "UnitTestNewVertexCollection";
            List<string> result = g.vertexCollections();
            Assert.False(result.Contains(name));
            g.addVertexCollection(name);
            result = g.vertexCollections();
            Assert.True(result.Contains(name));
        }
        
        [Test()]
        public void Should_add_a_vertex_collection_already_exists()
        {
            string name = "UnitTestNewVertexCollection";
            List<string> result = g.vertexCollections();
            Assert.False(result.Contains(name));
            g.addVertexCollection(name);
            result = g.vertexCollections();
            Assert.True(result.Contains(name));
            try {
                ArangoGraph resultGraph = g.addVertexCollection(name);    
            } catch(ArangoException e) {
                Assert.NotNull(e);
            }
            
        }
        
        [Test()]
        public void Should_delete_a_vertex_collection()
        {
            List<string> result = g.vertexCollections();
            Assert.True(result.Contains(vertexCol3));
            g.deleteVertexCollection(vertexCol3);
            result = g.vertexCollections();
            Assert.False(result.Contains(vertexCol3));
            Assert.True(db.Collection.Exists(vertexCol3));
            g.deleteVertexCollection(vertexCol2);
            result = g.vertexCollections();
            Assert.False(result.Contains(vertexCol2));
            Assert.True(db.Collection.Exists(vertexCol2));
        }

        [Test()]
        public void Should_delete_a_vertex_collection_error()
        {
            try {
                ArangoGraph graph = g.deleteVertexCollection("blub");    
            } catch (ArangoException e) {
                Assert.NotNull(e);
            }
            
        }

        [Test()]
        public void Should_delete_a_vertex_collection_without_dropping()
        {
            List<string> result = g.vertexCollections();
            Assert.True(result.Contains(vertexCol3));
            g.deleteVertexCollection(vertexCol3, false);
            result = g.vertexCollections();
            Assert.False(result.Contains(vertexCol3));
            Assert.True(db.Collection.Exists(vertexCol3));
            g.deleteVertexCollection(vertexCol2, false);
            result = g.vertexCollections();
            Assert.False(result.Contains(vertexCol2));
            Assert.True(db.Collection.Exists(vertexCol2));
        }
        
        [Test()]
        public void Should_delete_a_vertex_collection_with_dropping()
        {
            List<string> result = g.vertexCollections();
            Assert.True(result.Contains(vertexCol3));
            Assert.True(result.Contains(vertexCol2));
            
            g.deleteVertexCollection(vertexCol3, true);
            result = g.vertexCollections();
            Assert.False(result.Contains(vertexCol3));
            Assert.False(db.Collection.Exists(vertexCol3));
            
            g.deleteVertexCollection(vertexCol2, true);
            result = g.vertexCollections();
            Assert.False(result.Contains(vertexCol2));
            Assert.False(db.Collection.Exists(vertexCol2));
        }
        
        [Test()]
        public void Should_get_all_edge_collections()
        {
            List<string> result = g.edgeCollections();
            Assert.True(result.Contains(edgeCol1));
            Assert.True(result.Contains(edgeCol2));
            Assert.AreEqual(2, result.Count());
        }
        
        [Test()]
        public void Should_get_all_single_edge_definition()
        {
            ArangoGraphEdgeDefinition result = g.edgeDefinition(edgeCol1);
            Assert.AreEqual(ed1.collection, result.collection);
            Assert.AreEqual(ed1.from, result.from);
            Assert.AreEqual(ed1.to, result.to);
        }
        
        [Test()]
        public void Should_get_all_single_edge_definition_null()
        {
            ArangoGraphEdgeDefinition result = g.edgeDefinition("blub");
            Assert.Null(result);
        }
        
        [Test()]
        public void Should_add_an_edge_definition()
        {
            Assert.AreEqual(2, g.edgeDefinitions.Count());
            string edgeCol = "UnitTestEdgeCol47";
            ArangoGraphEdgeDefinition ed = new ArangoGraphEdgeDefinition(
                edgeCol,
                new List<string>{fromCol1},
                new List<string>{toCol1}
            );
            g.addEdgeDefinition(ed);
            
            Assert.AreEqual(3, g.edgeDefinitions.Count());
        }
        
        [Test()]
        public void Should_add_an_edge_definition_error()
        {
            Assert.AreEqual(2, g.edgeDefinitions.Count());
            ArangoGraphEdgeDefinition ed = new ArangoGraphEdgeDefinition(
                edgeCol1,
                new List<string>{fromCol2},
                new List<string>{toCol2}
            );
            try {
                g.addEdgeDefinition(ed);    
            } catch (ArangoException e) {
                Assert.NotNull(e);
            }
        }
        
        [Test()]
        public void Should_update_an_edge_definition()
        {
            ArangoGraphEdgeDefinition ed = new ArangoGraphEdgeDefinition(
                edgeCol1,
                new List<string>{fromCol1, vertexCol1},
                new List<string>{toCol2, vertexCol3}
            );
            g.updateEdgeDefinition(ed);
            Assert.False(g.orphanCollections.Contains(vertexCol1));
            Assert.True(g.orphanCollections.Contains(vertexCol2));
            Assert.False(g.orphanCollections.Contains(vertexCol3));
            Assert.True(g.orphanCollections.Contains(toCol1));
        }
        
        [Test()]
        public void Should_update_an_edge_definition_error()
        {
            ArangoGraphEdgeDefinition ed = new ArangoGraphEdgeDefinition(
                "blub",
                new List<string>{fromCol1, vertexCol1},
                new List<string>{toCol2, vertexCol3}
            );
            try {
                g.updateEdgeDefinition(ed);    
            } catch (ArangoException e) {
                Assert.NotNull(e);
            }
        }
        
        [Test()]
        public void Should_delete_an_edge_definition()
        {
            g.deleteEdgeDefinition(edgeCol2);
            Assert.True(g.orphanCollections.Contains(fromCol3));
            Assert.True(g.orphanCollections.Contains(toCol3));
            Assert.True(db.Collection.Exists(edgeCol2));
            Assert.True(db.Collection.Exists(fromCol3));
            Assert.True(db.Collection.Exists(toCol3));
        }
        
        [Test()]
        public void Should_delete_an_edge_definition_error()
        {
            try {
                g.deleteEdgeDefinition("blub");    
            } catch (ArangoException e) {
                Assert.NotNull(e);
            }
            
        }
        
        [Test()]
        public void Should_delete_an_edge_definition_without_dropping_collections()
        {
            g.deleteEdgeDefinition(edgeCol2, false);
            Assert.True(g.orphanCollections.Contains(fromCol3));
            Assert.True(g.orphanCollections.Contains(toCol3));
            Assert.True(db.Collection.Exists(edgeCol2));
            Assert.True(db.Collection.Exists(fromCol3));
            Assert.True(db.Collection.Exists(toCol3));
        }
        
        [Test()]
        public void Should_delete_an_edge_definition_with_dropping_collections()
        {
            g.deleteEdgeDefinition(edgeCol2, true);
            Assert.True(g.orphanCollections.Contains(fromCol3));
            Assert.True(g.orphanCollections.Contains(toCol3));
            Assert.True(db.Collection.Exists(fromCol3));
            Assert.True(db.Collection.Exists(toCol3));
            Assert.False(db.Collection.Exists(edgeCol2));
        }
        
        [Test()]
        public void Should_create_a_new_vertex()
        {
            Document vertex = new Document();
            vertex.Add("name", "blub");
            vertex.Add("type", "vertex");
            Document newVertex = g.addVertex(fromCol1, vertex, false);
            Assert.True(newVertex.Has("_id"));
            Assert.True(newVertex.Has("_key"));
            Assert.True(newVertex.Has("_rev"));
            Assert.True(newVertex.Has("name"));
            Assert.AreEqual("blub", newVertex.String("name"));
            Assert.True(newVertex.Has("type"));
            Assert.AreEqual("vertex", newVertex.String("type"));
        }
        
//        [Test()]
//        public void Should_create_a_new_vertex_error()
//        {
//            Document vertex = new Document();
//            try {
//                Document newVertex = g.addVertex("blub", vertex, false);
//            } catch (ArangoException e) {
//                Assert.NotNull(e);
//            }
//        }
        
        [Test()]
        public void Should_get_an_existing_vertex_from_a_graph_collection()
        {
            Document vertex = new Document();
            vertex.Add("name", "blub");
            vertex.Add("type", "vertex");
            Document newVertex = g.addVertex(fromCol1, vertex, false);
            string key = newVertex.String("_key");
            Document v = g.getVertex(fromCol1, key);
            Assert.True(v.Has("_id"));
            Assert.True(v.Has("_key"));
            Assert.True(v.Has("_rev"));
            Assert.True(v.Has("name"));
            Assert.AreEqual("blub", v.String("name"));
            Assert.True(v.Has("type"));
            Assert.AreEqual("vertex", v.String("type"));
        }
        
        [Test()]
        public void Should_replace_an_existing_vertex_from_a_graph_collection()
        {
            Document vertex = new Document();
            vertex.Add("name", "myName");
            vertex.Add("type", "myType");
            vertex.Add("data", "myData");
            Document newVertex = g.addVertex(fromCol1, vertex, false);
            string key = newVertex.String("_key");
            vertex.Clear();
            vertex.Add("name", "myOtherName");
            vertex.Add("myKey", "myValue");
            Document v = g.replaceVertex(fromCol1, key, vertex, true);
            Assert.True(v.Has("_id"));
            Assert.True(v.Has("_key"));
            Assert.True(v.Has("_rev"));
            Assert.False(v.Has("type"));
            Assert.False(v.Has("data"));
            Assert.True(v.Has("name"));
            Assert.AreEqual("myOtherName", v.String("name"));
            Assert.True(v.Has("myKey"));
            Assert.AreEqual("myValue", v.String("myKey"));
        }
        
        [Test()]
        public void Should_update_an_existing_vertex_from_a_graph_collection()
        {
            Document vertex = new Document();
            vertex.Add("name", "myName");
            vertex.Add("type", "myType");
            vertex.Add("data", "myData");
            Document newVertex = g.addVertex(fromCol1, vertex, false);
            string key = newVertex.String("_key");
            vertex.Clear();
            vertex.Add("name", "myOtherName");
            vertex.Add("myKey", "myValue");
            Document v = g.updateVertex(fromCol1, key, vertex, true, true);
            Assert.True(v.Has("_id"));
            Assert.True(v.Has("_key"));
            Assert.True(v.Has("_rev"));
            Assert.True(v.Has("type"));
            Assert.AreEqual("myType", v.String("type"));
            Assert.True(v.Has("data"));
            Assert.AreEqual("myData", v.String("data"));
            Assert.True(v.Has("name"));
            Assert.AreEqual("myOtherName", v.String("name"));
            Assert.True(v.Has("myKey"));
            Assert.AreEqual("myValue", v.String("myKey"));
        }
        
        [Test()]
        public void Should_delete_an_existing_vertex_from_a_graph_collection()
        {
            Document vertex = new Document();
            vertex.Add("name", "myName");
            vertex.Add("type", "myType");
            vertex.Add("data", "myData");
            Document newVertex = g.addVertex(fromCol1, vertex, false);
            string key = newVertex.String("_key");
            g.deleteVertex(fromCol1, key, true);
            Assert.Null(g.getVertex(fromCol1, key));
        }

        [Test()]
        public void Should_create_a_new_edge()
        {
            Document vertex1 = new Document();
            vertex1.Add("name", "a");
            vertex1 = g.addVertex(fromCol1, vertex1, false);
            Document vertex2 = new Document();
            vertex2.Add("name", "b");
            vertex2 = g.addVertex(toCol1, vertex2, false);
            Document edge = new Document();
            edge.Add("_from", vertex1.String("_id"));
            edge.Add("_to", vertex2.String("_id"));
            edge.Add("relation", "myRelation");
            Document result = g.addEdge(edgeCol1, vertex1.String("_id"), vertex2.String("_id"), edge, false);
            Assert.True(result.Has("_id"));
            Assert.True(result.Has("_key"));
            Assert.True(result.Has("_rev"));
            Assert.True(result.Has("_from"));
            Assert.AreEqual(vertex1.String("_id"), result.String("_from"));
            Assert.True(result.Has("_to"));
            Assert.AreEqual(vertex2.String("_id"), result.String("_to"));
        }        
        
        [Test()]
        public void Should_create_a_new_edge_with_error()
        {
            Document vertex1 = new Document();
            vertex1.Add("name", "a");
            vertex1 = g.addVertex(fromCol1, vertex1, false);
            Document vertex2 = new Document();
            vertex2.Add("name", "b");
            vertex2 = g.addVertex(fromCol1, vertex2, false);
            Document edge = new Document();
            edge.Add("_from", vertex1.String("_id"));
            edge.Add("_to", vertex2.String("_id"));
            edge.Add("relation", "myRelation");
            try {
                Document result = g.addEdge(edgeCol1, vertex1.String("_id"), vertex2.String("_id"), edge, false);    
            } catch (ArangoException e) {
                Assert.NotNull(e);                
            }
            
        }        
        
        [Test()]
        public void Should_get_an_existing_edge_from_a_graph()
        {
            Document vertex1 = new Document();
            vertex1.Add("name", "a");
            vertex1 = g.addVertex(fromCol1, vertex1, false);
            Document vertex2 = new Document();
            vertex2.Add("name", "b");
            vertex2 = g.addVertex(toCol1, vertex2, false);
            Document edge = new Document();
            edge.Add("_from", vertex1.String("_id"));
            edge.Add("_to", vertex2.String("_id"));
            edge.Add("relation", "myRelation");
            edge = g.addEdge(edgeCol1, vertex1.String("_id"), vertex2.String("_id"), edge, false);
            string key = edge.String("_key");
            Document result = g.getEdge(edgeCol1, key);
            Assert.True(result.Has("_from"));
            Assert.AreEqual(vertex1.String("_id"), result.String("_from"));
            Assert.True(result.Has("_to"));
            Assert.AreEqual(vertex2.String("_id"), result.String("_to"));
            Assert.True(result.Has("relation"));
            Assert.AreEqual("myRelation", result.String("relation"));

            
        }        
        
        [Test()]
        public void Should_replace_an_existing_edge_from_a_graph()
        {
            Document vertex1 = new Document();
            vertex1.Add("name", "a");
            vertex1 = g.addVertex(fromCol1, vertex1, false);
            Document vertex2 = new Document();
            vertex2.Add("name", "b");
            vertex2 = g.addVertex(toCol1, vertex2, false);
            Document edge = new Document();
            edge.Add("_from", vertex1.String("_id"));
            edge.Add("_to", vertex2.String("_id"));
            edge.Add("relation", "myRelation");
            edge = g.addEdge(edgeCol1, vertex1.String("_id"), vertex2.String("_id"), edge, false);
            string key = edge.String("_key");
            edge.Clear();
            edge.Add("blub", "blub");
            Document result = g.replaceEdge(edgeCol1, key, edge, false);
            Assert.False(result.Has("relation"));
            Assert.True(result.Has("blub"));
            Assert.AreEqual("blub", result.String("blub"));
        }        
        
        [Test()]
        public void Should_replace_an_existing_edge_from_a_graph_not_found()
        {
            Document edge = new Document();
            Document result = g.replaceEdge(edgeCol1, "0815", edge, false);
            Assert.Null(result);
        }        
        
        [Test()]
        public void Should_update_an_existing_edge_from_a_graph()
        {
            Document vertex1 = new Document();
            vertex1.Add("name", "a");
            vertex1 = g.addVertex(fromCol1, vertex1, false);
            Document vertex2 = new Document();
            vertex2.Add("name", "b");
            vertex2 = g.addVertex(toCol1, vertex2, false);
            Document edge = new Document();
            edge.Add("_from", vertex1.String("_id"));
            edge.Add("_to", vertex2.String("_id"));
            edge.Add("r1", "1");
            edge.Add("r2", "2");
            edge = g.addEdge(edgeCol1, vertex1.String("_id"), vertex2.String("_id"), edge, false);
            string key = edge.String("_key");
            edge.Clear();
            edge.Add("blub", "blub");
            edge.Add("r1", "11");
            Document result = g.updateEdge(edgeCol1, key, edge, false, true);
            Assert.True(result.Has("r1"));
            Assert.AreEqual("11", result.String("r1"));
            Assert.True(result.Has("r2"));
            Assert.AreEqual("2", result.String("r2"));
            Assert.True(result.Has("blub"));
            Assert.AreEqual("blub", result.String("blub"));
        }        
        
        [Test()]
        public void Should_update_an_existing_edge_from_a_graph_not_found()
        {
            Document edge = new Document();
            Document result = g.updateEdge(edgeCol1, "0815", edge, false, false);
            Assert.Null(result);
        }        
        
        [Test()]
        public void Should_delete_an_existing_edge_from_a_graph()
        {
            Document vertex1 = new Document();
            vertex1.Add("name", "a");
            vertex1 = g.addVertex(fromCol1, vertex1, false);
            Document vertex2 = new Document();
            vertex2.Add("name", "b");
            vertex2 = g.addVertex(toCol1, vertex2, false);
            Document edge = new Document();
            edge.Add("_from", "blub");
            edge.Add("_to", "blub");
            edge.Add("r1", "1");
            edge.Add("r2", "2");
            edge = g.addEdge(edgeCol1, vertex1.String("_id"), vertex2.String("_id"), edge, false);
            string key = edge.String("_key");
            g.deleteEdge(edgeCol1, key, false);
            Assert.Null(g.getEdge(edgeCol1, key));
        }        
        
        
        
    }
}
