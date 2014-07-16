using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Arango.Client.Protocol;
using Newtonsoft.Json;

namespace Arango.Client.Protocol
{
    /// <summary>
    /// Description of GraphOperation.
    /// </summary>
    public class GraphGraphOperation
    {
        private string _apiUri;
        private Connection _connection { get; set; }
        private string _graphName;
        
        internal GraphGraphOperation(Connection connection, string graphName)
        {
            _connection = connection;
            _graphName = graphName;
            _apiUri = "_api/gharial/" + graphName;
        }

        #region HELPERS
        
        private ArangoGraph documentToGraph(Document doc) {
            string graphName;
            if (doc.ContainsKey("name")) {
                graphName = doc.String("name");    
            } else {
                graphName = doc.String("_key");    
            }
            ArangoGraph graph = new ArangoGraph(_connection, graphName);
            graph.orphanCollections = doc.List<String>("orphanCollections");
            List<Object> eDs = doc.List<Object>("edgeDefinitions");
            List<ArangoGraphEdgeDefinition> edgeDefinitions = new List<ArangoGraphEdgeDefinition>();
            foreach(Document eD in eDs) {
                ArangoGraphEdgeDefinition edgeDefinition = new ArangoGraphEdgeDefinition(
                   eD.String("collection"),
                   eD.List<String>("from"),
                   eD.List<String>("to")
                   );
                edgeDefinitions.Add(edgeDefinition);
            }
            graph.edgeDefinitions = edgeDefinitions;

            return graph;
        }
        
        private ArangoGraph responseToGraph(Response response) {
            var json = response.JsonString;
            Document doc = response.Document.Object("graph");
            return this.documentToGraph(doc);
        }
        
        #endregion
        
        #region VertexCollection

        /// <summary>
        /// Gets the list of all vertex collections.
        /// </summary>
        internal List<string> vertexCollections() {
            var request = new Request(RequestType.Collection, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, "vertex");
            
            var response = _connection.Process(request);
            
            List<string> result = new List<string>();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result = response.Document.List<string>("collections");
                    break;
                case HttpStatusCode.NotFound:
                    result = null;
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Adds a vertex collection to the graph.
        /// </summary>
        internal ArangoGraph addVertexCollection(string colName) {
            var request = new Request(RequestType.Collection, HttpMethod.Post);
            request.RelativeUri = string.Join("/", _apiUri, "vertex");
            Document body = new Document();
            body.Add("collection", colName);
            request.Body = body.Serialize();
            
            var response = _connection.Process(request);
            
            ArangoGraph result = new ArangoGraph(_connection, _graphName);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    result = this.responseToGraph(response);
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }        


        /// <summary>
        /// Removes a vertex collection from this graph. If this collection is used in one ore
        /// more edge definitions it will not be dropped, if this option is set 
        /// </summary>
        internal ArangoGraph deleteVertexCollection(string colName, bool dropCollection) {
            string drop = "dropCollection=false";
            if (dropCollection) {
                drop = "dropCollection=true";
            }
            var request = new Request(RequestType.Collection, HttpMethod.Delete);
            string uri = string.Join("/", _apiUri, "vertex", colName);
            request.RelativeUri = string.Join("?", uri, drop);
           
            var response = _connection.Process(request);
            
            ArangoGraph result = new ArangoGraph(_connection, _graphName);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result = this.responseToGraph(response);
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }        

        #endregion
        
        #region EdgeCollection

        /// <summary>
        /// Gets the list of all edge collections.
        /// </summary>
        internal List<string> edgeCollections() {
            var request = new Request(RequestType.Collection, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, "edge");
            
            var response = _connection.Process(request);
            
            List<string> result = new List<string>();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result = response.Document.List<string>("collections");
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }
        
        /// <summary>
        /// Gets the list of all edge collections.
        /// </summary>
        internal ArangoGraph addEdgeDefinition(ArangoGraphEdgeDefinition edgeDefinition) {
            var request = new Request(RequestType.Collection, HttpMethod.Post);
            request.RelativeUri = string.Join("/", _apiUri, "edge");

            Document body = new Document();
            body.Add("collection", edgeDefinition.collection);
            body.Add("from", edgeDefinition.from);
            body.Add("to", edgeDefinition.to);
            request.Body = body.Serialize();
            
            var response = _connection.Process(request);
            
            ArangoGraph result = new ArangoGraph(_connection, _graphName);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    result = this.responseToGraph(response);
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Update an existing edge definition.
        /// </summary>
        internal ArangoGraph updateEdgeDefinition(ArangoGraphEdgeDefinition edgeDefinition) {
            var request = new Request(RequestType.Collection, HttpMethod.Put);
            request.RelativeUri = string.Join("/", _apiUri, "edge", edgeDefinition.collection);

            Document body = new Document();
            body.Add("collection", edgeDefinition.collection);
            body.Add("from", edgeDefinition.from);
            body.Add("to", edgeDefinition.to);
            request.Body = body.Serialize();
            
            var response = _connection.Process(request);
            
            ArangoGraph result = new ArangoGraph(_connection, _graphName);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result = this.responseToGraph(response);
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }
        
        
        /// <summary>
        /// Delete an existing edge definition.
        /// </summary>
        internal ArangoGraph deleteEdgeDefinition(string collectionName, bool dropCollections) {
            string drop = "dropCollection=false";
            if (dropCollections) {
                drop = "dropCollection=true";
            }
            var request = new Request(RequestType.Collection, HttpMethod.Delete);
            string uri = string.Join("/", _apiUri, "edge", collectionName);
            request.RelativeUri = string.Join("?", uri, drop);
            
            var response = _connection.Process(request);
            
            ArangoGraph result = new ArangoGraph(_connection, _graphName);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result = this.responseToGraph(response);
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }
        
        
        
        #endregion
        
        #region Vertex
        
        /// <summary>
        /// Add new vertex to a collection of the graph.
        /// </summary>
        internal Document addVertex(string collectionName, Document vertex, bool waitForSync) {
            string wFS = "false";
            if (waitForSync) {
                wFS = "true";
            }
            var request = new Request(RequestType.Collection, HttpMethod.Post);
            string uri = string.Join("/", _apiUri, "vertex", collectionName);
            request.RelativeUri = string.Join("?", uri, "waitForSync=" + wFS);
            request.Body = vertex.Serialize();
            
            var response = _connection.Process(request);
            
            Document result = new Document();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.Accepted:
                    result = response.Document.Object<Document>("vertex");
                    result.Merge(vertex, MergeOptions.MergeFields);
                    break;
                case HttpStatusCode.NotFound:
                    result = null;
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }        
        
        /// <summary>
        /// Get a vertex with the given key if it is contained in the graph.
        /// </summary>
        internal Document getVertex(string collectionName, string key) {
            var request = new Request(RequestType.Collection, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, "vertex", collectionName, key);
            
            var response = _connection.Process(request);
            
            Document result = new Document();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result = response.Document.Object<Document>("vertex");
                    break;
                case HttpStatusCode.NotFound:
                    result = null;
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }        
        
        /// <summary>
        /// Replaces a vertex with the given id by the content in the body. 
        /// This will only run successfully if the vertex is contained within the graph.
        /// </summary>
        internal Document replaceVertex(string collectionName, string key, Document vertex, bool waitForSync) {
            var request = new Request(RequestType.Collection, HttpMethod.Put);
            string uri = string.Join("/", _apiUri, "vertex", collectionName, key);
            request.RelativeUri = string.Join("?", uri, "waitForSync=" + waitForSync);
            request.Body = vertex.Serialize();

            var response = _connection.Process(request);
            
            Document result = new Document();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result = response.Document.Object<Document>("vertex");
                    result.Merge(vertex, MergeOptions.MergeFields);
                    break;
                case HttpStatusCode.NotFound:
                    result = null;
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }        
        
        /// <summary>
        /// Updates a vertex with the given id by adding the content in the body. 
        /// This will only run successfully if the vertex is contained within the graph.
        /// </summary>
        internal Document updateVertex(
            string collectionName, 
            string key, 
            Document vertex,
            bool waitForSync, 
            bool keepNull
        ) {
            Document result = this.getVertex(collectionName, key);
            var request = new Request(RequestType.Collection, HttpMethod.Patch);
            string uri = string.Join("/", _apiUri, "vertex", collectionName, key);
            request.RelativeUri = string.Join("?", uri, "waitForSync=" + waitForSync + "&keepNull=" + keepNull);
            request.Body = vertex.Serialize();

            var response = _connection.Process(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result.Merge(vertex, MergeOptions.ReplaceFields);
                    result.Merge(response.Document.Object<Document>("vertex"), MergeOptions.ReplaceFields);
                    break;
                case HttpStatusCode.NotFound:
                    result = null;
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }        
        
        /// <summary>
        /// Deletes a vertex with the given id, if it is contained within the graph. 
        /// Furthermore all edges connected to this vertex will be deleted.
        /// </summary>
        internal void deleteVertex(string collectionName, string key, bool waitForSync) {
            var request = new Request(RequestType.Collection, HttpMethod.Delete);
            request.RelativeUri = string.Join("/", _apiUri, "vertex", collectionName, key);

            var response = _connection.Process(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.Accepted:
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }
        }        
        
        
        #endregion
        
        #region Edge
        
        /// <summary>
        /// Stores a new edge with the information contained within the body into the given collection.
        /// </summary>
        internal Document addEdge(string collectionName, string fromId, string toId, Document edge, bool waitForSync) {
            var request = new Request(RequestType.Collection, HttpMethod.Post);
            string uri = string.Join("/", _apiUri, "edge", collectionName);
            request.RelativeUri = string.Join("?", uri, "waitForSync=" + waitForSync);
            if (edge.ContainsKey("_from")) {
                edge.Drop("_from");
            }
            if (edge.ContainsKey("_to")) {
                edge.Drop("_to");
            }
            
            Document body = edge;
            edge.Add("_from", fromId);
            edge.Add("_to", toId);
            
            request.Body = edge.Serialize();
            
            var response = _connection.Process(request);
            
            Document result = new Document();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.Accepted:
                    result = response.Document.Object<Document>("edge");
                    result.Merge(edge, MergeOptions.MergeFields);
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }        
        
        /// <summary>
        /// Loads an edge with the given id if it is contained within your graph.
        /// </summary>
        internal Document getEdge(string collectionName, string key) {
            var request = new Request(RequestType.Collection, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, "edge", collectionName, key);
            
            var response = _connection.Process(request);
            Document result = new Document();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result = response.Document.Object<Document>("edge");
                    break;
                case HttpStatusCode.NotFound:
                    result = null;
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }        
        
        /// <summary>
        /// Replaces an edge with the given id by the content in the body. 
        /// This will only run successfully if the edge is contained within the graph.
        /// </summary>
        internal Document replaceEdge(string collectionName, string key, Document edge, bool waitForSync) {
            var request = new Request(RequestType.Collection, HttpMethod.Put);
            string uri = string.Join("/", _apiUri, "edge", collectionName, key);
            request.RelativeUri = string.Join("?", uri, "waitForSync=" + waitForSync);
            request.Body = edge.Serialize();

            var response = _connection.Process(request);
            
            Document result = new Document();
            switch (response.StatusCode)
            {
                case HttpStatusCode.Accepted:
                    result = response.Document.Object<Document>("edge");
                    result.Merge(edge, MergeOptions.MergeFields);
                    break;
                case HttpStatusCode.NotFound:
                    result = null;
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }        
        
        /// <summary>
        /// Updates an edge with the given id by adding the content in the body. 
        /// This will only run successfully if the edge is contained within the graph.
        /// </summary>
        internal Document updateEdge(
            string collectionName, 
            string key, 
            Document edge,
            bool waitForSync, 
            bool keepNull
        ) {
            Document result = this.getEdge(collectionName, key);
            var request = new Request(RequestType.Collection, HttpMethod.Patch);
            string uri = string.Join("/", _apiUri, "edge", collectionName, key);
            request.RelativeUri = string.Join("?", uri, "waitForSync=" + waitForSync + "&keepNull=" + keepNull);
            request.Body = edge.Serialize();

            var response = _connection.Process(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.Accepted:
                    result.Merge(edge, MergeOptions.ReplaceFields);
                    result.Merge(response.Document.Object<Document>("edge"), MergeOptions.ReplaceFields);
                    break;
                case HttpStatusCode.NotFound:
                    result = null;
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }

            return result;
        }        
        
        /// <summary>
        /// Deletes an edge with the given id, if it is contained within the graph. 
        /// </summary>
        internal void deleteEdge(string collectionName, string key, bool waitForSync) {
            var request = new Request(RequestType.Collection, HttpMethod.Delete);
            request.RelativeUri = string.Join("/", _apiUri, "edge", collectionName, key);

            var response = _connection.Process(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.Accepted:
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }
        }        
        
        
        #endregion
    }
}
