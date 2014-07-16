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
    public class GraphOperation
    {
        private string _apiUri { get { return "_api/gharial"; } }
        private Connection _connection { get; set; }
        
        internal GraphOperation(Connection connection)
        {
            _connection = connection;
        }
        
        # region HELPERS

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
        
        # region GET
        
        /// <summary>
        /// Get all graphs of the database.
        /// </summary>
        internal List<ArangoGraph> Get() {
            
            List<ArangoGraph> result = new List<ArangoGraph>();
            var request = new Request(RequestType.Collection, HttpMethod.Get);
            request.RelativeUri = _apiUri;
            
            var response = _connection.Process(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    Document doc = response.Document;
                    var graphs = doc.List<Document>("graphs");
                    //Convert JSON into ArangoGraph objects 
                    foreach(Document graph in graphs) {
                        result.Add(this.documentToGraph(graph));
                    }
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
        /// Get all graphs of the database.
        /// </summary>
        internal ArangoGraph Get(String name) {
            var graph = new ArangoGraph(_connection, name);

            var request = new Request(RequestType.Collection, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, name);
            
            var response = _connection.Process(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    graph = this.responseToGraph(response);
                    break;
                case HttpStatusCode.NotFound:
                    graph = null;
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

            return graph;
        }
        
        #endregion
        
        # region POST
        
        
        internal ArangoGraph Post(string graphName)
        {
            return this.Post(graphName, null, null);
        }
        
        internal ArangoGraph Post(string graphName, List<ArangoGraphEdgeDefinition> edgeDefinitions)
        {
            return this.Post(graphName, edgeDefinitions, null);
        }
        
        internal ArangoGraph Post(string graphName, List<ArangoGraphEdgeDefinition> edgeDefinitions, List<String> vertexCollections)
        {
            var request = new Request(RequestType.Document, HttpMethod.Post);
            request.RelativeUri = _apiUri;
            Document body = new Document();
            body.Add("name", graphName);
            body.Add("orphanCollections", vertexCollections);
            body.Add("edgeDefinitions", edgeDefinitions);
            request.Body = body.Serialize();
            var response = _connection.Process(request);
            ArangoGraph graph = new ArangoGraph(_connection, graphName);

            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    graph = this.responseToGraph(response);
                    break;
                case HttpStatusCode.NotFound:
                    graph = null;
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
            return graph;
        }

        internal ArangoGraph Post(string graphName, List<String> vertexCollections)
        {
            return this.Post(graphName, null, vertexCollections);
        }

        #endregion
        
        #region DELETE
        
        internal void Delete(string graphName)
        {
            var request = new Request(RequestType.Document, HttpMethod.Delete);
            request.RelativeUri = string.Join("/", _apiUri, graphName);
            var response = _connection.Process(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.NotFound:
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
        
        internal void Delete(string graphName, bool dropCollections)
        {
            var request = new Request(RequestType.Document, HttpMethod.Delete);
            string uri = string.Join("/", _apiUri, graphName);
            if(dropCollections) {
                request.RelativeUri = string.Join("?", uri, "dropCollections=true");
            } else {
                request.RelativeUri = string.Join("?", uri, "dropCollections=false");
            }
            var response = _connection.Process(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.NotFound:
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
        
        #region EXISTS
        
        /// <summary>
        /// Determines if the graph with the specified name exists.
        /// </summary>
        internal bool Exists(String name) {
            var request = new Request(RequestType.Collection, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, name);
            
            var response = _connection.Process(request);
            
            bool result = false;
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result = true;
                    break;
                case HttpStatusCode.NotFound:
                    result = false;
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

        
    }
}
