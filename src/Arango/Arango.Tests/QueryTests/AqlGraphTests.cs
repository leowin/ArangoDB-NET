using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.AqlGraphTests
{
    [TestFixture()]
    public class AqlGraphTests
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
        ArangoDatabase db;
        ArangoGraph g;
        
        [SetUp()] public void Init()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            db = Database.GetTestDatabase();
            List<ArangoGraphEdgeDefinition> eds = new List<ArangoGraphEdgeDefinition>();
            ArangoGraphEdgeDefinition ed1 = new ArangoGraphEdgeDefinition(
                edgeCol1,
                new List<String>{fromCol1, fromCol2},
                new List<String>{toCol1, toCol2}
            );
            eds.Add(ed1);
            ArangoGraphEdgeDefinition ed2 = new ArangoGraphEdgeDefinition(
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
        public void Should_generate_query_for_GRAPH_EDGES_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_EDGES(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_EDGES('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            Assert.AreEqual(aql, expression.ToString());
        }

        [Test()]
        public void Should_generate_query_for_GRAPH_EDGES_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_EDGES(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_EDGES('" + graphName + "', {'k1':'v1'}, {'k2':'v2'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_EDGES('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }

        [Test()]
        public void Should_generate_query_for_GRAPH_VERTICES_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_VERTICES(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_VERTICES('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            Assert.AreEqual(aql, expression.ToString());
        }

        [Test()]
        public void Should_generate_query_for_GRAPH_VERTICES_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_VERTICES(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_VERTICES('" + graphName + "', {'k1':'v1'}, {'k2':'v2'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_VERTICES('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }
        
        [Test()]
        public void Should_generate_query_for_GRAPH_NEIGHBORS_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_NEIGHBORS(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_NEIGHBORS('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            Assert.AreEqual(aql, expression.ToString());
        }

        [Test()]
        public void Should_generate_query_for_GRAPH_COMMON_NEIGHBORS_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_COMMON_NEIGHBORS(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_COMMON_NEIGHBORS('" + graphName + "', {'k1':'v1'}, {'k2':'v2'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_COMMON_NEIGHBORS('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }
        
        [Test()]
        public void Should_generate_query_for_GRAPH_COMMON_NEIGHBORS_1()
        {
        ArangoQueryOperation expression = new ArangoQueryOperation()
            .Aql(_ => _
                .FOR("edges")
                   .IN(
                       _.GRAPH_COMMON_NEIGHBORS(
                          graphName,
                          _.GRAPH_JSON("{'k1':'v1'}"),
                          _.GRAPH_JSON("{'k2':'v2'}"),
                          _.GRAPH_JSON("{'k3':'v3'}"),
                          _.GRAPH_JSON("{'k4':'v4'}")
                       )
                   )
               .RETURN.Var("edges")
            );
            var aql = "FOR edges IN GRAPH_COMMON_NEIGHBORS('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}, {'k3':'v3'}, {'k4':'v4'})\nRETURN edges";
            Assert.AreEqual(aql, expression.ToString());
        }

        [Test()]
        public void Should_generate_query_for_GRAPH_NEIGHBORS_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_NEIGHBORS(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_NEIGHBORS('" + graphName + "', {'k1':'v1'}, {'k2':'v2'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_NEIGHBORS('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }
        
        [Test()]
        public void Should_generate_query_for_GRAPH_COMMON_PROPERTIES_1()
        {
        ArangoQueryOperation expression = new ArangoQueryOperation()
            .Aql(_ => _
                .FOR("edges")
                   .IN(
                       _.GRAPH_COMMON_PROPERTIES(
                          graphName,
                          _.GRAPH_JSON("{'k1':'v1'}"),
                          _.GRAPH_JSON("{'k2':'v2'}"),
                          _.GRAPH_JSON("{'k3':'v3'}")
                       )
                   )
               .RETURN.Var("edges")
            );
            var aql = "FOR edges IN GRAPH_COMMON_PROPERTIES('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}, {'k3':'v3'})\nRETURN edges";
            Assert.AreEqual(aql, expression.ToString());
        }

        [Test()]
        public void Should_generate_query_for_GRAPH_COMMON_PROPERTIES_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_COMMON_PROPERTIES(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_COMMON_PROPERTIES('" + graphName + "', {'k1':'v1'}, {'k2':'v2'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_COMMON_PROPERTIES('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }
        
        [Test()]
        public void Should_generate_query_for_GRAPH_PATHS_1()
        {
        ArangoQueryOperation expression = new ArangoQueryOperation()
            .Aql(_ => _
                .FOR("edges")
                   .IN(
                       _.GRAPH_PATHS(
                          graphName
                       )
                   )
               .RETURN.Var("edges")
            );
            var aql = "FOR edges IN GRAPH_PATHS('" + graphName + "')\nRETURN edges";
            Assert.AreEqual(aql, expression.ToString());
        }

        [Test()]
        public void Should_generate_query_for_GRAPH_PATHS_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_PATHS(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_PATHS('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_PATHS('" + graphName + "', {'k1':'v1'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }
        
        [Test()]
        public void Should_generate_query_for_GRAPH_SHORTEST_PATH_1()
        {
        ArangoQueryOperation expression = new ArangoQueryOperation()
            .Aql(_ => _
                .FOR("edges")
                   .IN(
                       _.GRAPH_SHORTEST_PATH(
                          graphName,
                          _.GRAPH_JSON("{'k1':'v1'}"),
                          _.GRAPH_JSON("{'k2':'v2'}"),
                          _.GRAPH_JSON("{'k3':'v3'}")
                       )
                   )
               .RETURN.Var("edges")
            );
            var aql = "FOR edges IN GRAPH_SHORTEST_PATH('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}, {'k3':'v3'})\nRETURN edges";
            Assert.AreEqual(aql, expression.ToString());
        }

        [Test()]
        public void Should_generate_query_for_GRAPH_SHORTEST_PATH_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_SHORTEST_PATH(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_SHORTEST_PATH('" + graphName + "', {'k1':'v1'}, {'k2':'v2'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_SHORTEST_PATH('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }
        
        [Test()]
        public void Should_generate_query_for_GRAPH_TRAVERSAL_1()
        {
        ArangoQueryOperation expression = new ArangoQueryOperation()
            .Aql(_ => _
                .FOR("edges")
                   .IN(
                       _.GRAPH_TRAVERSAL(
                          graphName,
                          _.GRAPH_JSON("{'k1':'v1'}"),
                          _.GRAPH_JSON("{'k2':'v2'}"),
                          _.GRAPH_JSON("{'k3':'v3'}")
                       )
                   )
               .RETURN.Var("edges")
            );
            var aql = "FOR edges IN GRAPH_TRAVERSAL('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}, {'k3':'v3'})\nRETURN edges";
            Assert.AreEqual(aql, expression.ToString());
        }

        [Test()]
        public void Should_generate_query_for_GRAPH_TRAVERSAL_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_TRAVERSAL(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_TRAVERSAL('" + graphName + "', {'k1':'v1'}, {'k2':'v2'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_TRAVERSAL('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_TRAVERSAL_3()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_TRAVERSAL(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_TRAVERSAL('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_TRAVERSAL('" + graphName + "', {'k1':'v1'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_TRAVERSAL_TREE_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_TRAVERSAL_TREE(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}"),
                             _.GRAPH_JSON("{'k3':'v3'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_TRAVERSAL_TREE('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}, {'k3':'v3'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_TRAVERSAL_TREE('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}, {'k3':'v3'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_TRAVERSAL_TREE_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_TRAVERSAL_TREE(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}"),
                             _.GRAPH_JSON("{'k3':'v3'}"),
                             _.GRAPH_JSON("{'k4':'v4'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_TRAVERSAL_TREE('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}, {'k3':'v3'}, {'k4':'v4'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_TRAVERSAL_TREE('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}, {'k3':'v3'}, {'k4':'v4'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_DISTANCE_TO_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_DISTANCE_TO(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_DISTANCE_TO('" + graphName + "', {'k1':'v1'}, {'k2':'v2'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_DISTANCE_TO('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_DISTANCE_TO_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_DISTANCE_TO(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}"),
                             _.GRAPH_JSON("{'k3':'v3'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_DISTANCE_TO('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}, {'k3':'v3'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_DISTANCE_TO('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}, {'k3':'v3'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                
 
        [Test()]
        public void Should_generate_query_for_GRAPH_ABSOLUTE_ECCENTRICITY_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_ABSOLUTE_ECCENTRICITY(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_ABSOLUTE_ECCENTRICITY('" + graphName + "', {'k1':'v1'}, {'k2':'v2'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_ABSOLUTE_ECCENTRICITY('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_ABSOLUTE_ECCENTRICITY_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_ABSOLUTE_ECCENTRICITY(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_ABSOLUTE_ECCENTRICITY('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_ABSOLUTE_ECCENTRICITY('" + graphName + "', {'k1':'v1'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                
    
        [Test()]
        public void Should_generate_query_for_GRAPH_ECCENTRICITY_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_ECCENTRICITY(
                             graphName
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_ECCENTRICITY('" + graphName + "')\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_ECCENTRICITY('" + graphName + "') RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_ECCENTRICITY_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_ECCENTRICITY(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_ECCENTRICITY('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_ECCENTRICITY('" + graphName + "', {'k1':'v1'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_ABSOLUTE_CLOSENESS_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_ABSOLUTE_CLOSENESS(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_ABSOLUTE_CLOSENESS('" + graphName + "', {'k1':'v1'}, {'k2':'v2'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_ABSOLUTE_CLOSENESS('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_ABSOLUTE_CLOSENESS_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_ABSOLUTE_CLOSENESS(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_ABSOLUTE_CLOSENESS('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_ABSOLUTE_CLOSENESS('" + graphName + "', {'k1':'v1'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_CLOSENESS_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_CLOSENESS(
                             graphName
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_CLOSENESS('" + graphName + "')\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_CLOSENESS('" + graphName + "') RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_CLOSENESS_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_CLOSENESS(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_CLOSENESS('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_CLOSENESS('" + graphName + "', {'k1':'v1'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_ABSOLUTE_BETWEENNESS_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_ABSOLUTE_BETWEENNESS(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}"),
                             _.GRAPH_JSON("{'k2':'v2'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_ABSOLUTE_BETWEENNESS('" + graphName + "', {'k1':'v1'}, {'k2':'v2'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_ABSOLUTE_BETWEENNESS('" + graphName + "', {'k1':'v1'}, {'k2':'v2'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_ABSOLUTE_BETWEENNESS_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_ABSOLUTE_BETWEENNESS(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_ABSOLUTE_BETWEENNESS('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_ABSOLUTE_BETWEENNESS('" + graphName + "', {'k1':'v1'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                
            
        
        
        [Test()]
        public void Should_generate_query_for_GRAPH_BETWEENNESS_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_BETWEENNESS(
                             graphName
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_BETWEENNESS('" + graphName + "')\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_BETWEENNESS('" + graphName + "') RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_BETWEENNESS_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_BETWEENNESS(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_BETWEENNESS('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_BETWEENNESS('" + graphName + "', {'k1':'v1'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_RADIUS_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_RADIUS(
                             graphName
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_RADIUS('" + graphName + "')\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_RADIUS('" + graphName + "') RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_RADIUS_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_RADIUS(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_RADIUS('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_RADIUS('" + graphName + "', {'k1':'v1'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_DIAMETER_1()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_DIAMETER(
                             graphName
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_DIAMETER('" + graphName + "')\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_DIAMETER('" + graphName + "') RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                

        [Test()]
        public void Should_generate_query_for_GRAPH_DIAMETER_2()
        {
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                     .FOR("edges")
                     .IN(
                         _.GRAPH_DIAMETER(
                             graphName,
                             _.GRAPH_JSON("{'k1':'v1'}")
                            )
                        )
                     .RETURN.Var("edges")
                    );
            var aql = "FOR edges IN GRAPH_DIAMETER('" + graphName + "', {'k1':'v1'})\nRETURN edges";
            var aql2 = "FOR edges IN GRAPH_DIAMETER('" + graphName + "', {'k1':'v1'}) RETURN edges";
            Assert.AreEqual(aql, expression.ToString());
            Assert.AreEqual(aql2, expression.ToString(false));
        }                


    }
}
