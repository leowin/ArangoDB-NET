/*
 * Created by SharpDevelop.
 * User: gschwab
 * Date: 02.07.2014
 * Time: 16:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Arango;
using Arango.Client.Protocol;
using System.Collections.Generic;

namespace Arango.Client
{
    /// <summary>
    /// Description of ArangoGraph.
    /// </summary>
    public class ArangoGraph
    {
        private GraphGraphOperation _graphGraphOps;
        private string _name;
        public string Name { get { return _name; } }
        public List<String> orphanCollections { get; set; }
        public List<ArangoGraphEdgeDefinition> edgeDefinitions { get; set; }
        
        internal ArangoGraph(Connection connection, string name)
        {
            _name = name;
            _graphGraphOps = new GraphGraphOperation(connection, name);
        }
        
        private void updateGraph(ArangoGraph g) {
            this.edgeDefinitions = g.edgeDefinitions;
            this.orphanCollections = g.orphanCollections;
        }
        
        /// <summary>
        /// List all vertex collections used within the graph.
        /// </summary>
        public List<string> vertexCollections() {
            return _graphGraphOps.vertexCollections();
        }
        
        /// <summary>
        /// Adds a vertex collection to the graph.
        /// </summary>
        public ArangoGraph addVertexCollection(string colName) {
            ArangoGraph g = _graphGraphOps.addVertexCollection(colName);
            this.updateGraph(g);
            return g;
        }
        
        /// <summary>
        /// Removes a vertex collection from this graph. The collection will not be dropped.
        /// </summary>
        public ArangoGraph deleteVertexCollection(string colName) {
            ArangoGraph g = _graphGraphOps.deleteVertexCollection(colName, false);
            this.updateGraph(g);
            return g;
        }

        /// <summary>
        /// Removes a vertex collection from this graph. If this collection is used in one ore
        /// more edge definitions it will not be dropped, if this option is set 
        /// </summary>
        public ArangoGraph deleteVertexCollection(string colName, bool dropCollections) {
            ArangoGraph g = _graphGraphOps.deleteVertexCollection(colName, dropCollections);
            this.updateGraph(g);
            return g;
        }
        
        /// <summary>
        /// List all edge collections used within the graph.
        /// </summary>
        public List<string> edgeCollections() {
            return _graphGraphOps.edgeCollections();
        }
        
        /// <summary>
        /// Get edge definition by name.
        /// </summary>
        public ArangoGraphEdgeDefinition edgeDefinition(string collectionName) {
            if (this.edgeCollections().Contains(collectionName)) {
                foreach(ArangoGraphEdgeDefinition ed in this.edgeDefinitions) {
                    if (ed.collection == collectionName) {
                        return ed;
                    }
                }
                
            }
            return null;
        }
        
        /// <summary>
        /// Add new edge definition to the graph.
        /// </summary>
        public ArangoGraph addEdgeDefinition(ArangoGraphEdgeDefinition edgeDefinition) {
            ArangoGraph g = _graphGraphOps.addEdgeDefinition(edgeDefinition);
            this.updateGraph(g);
            return g;
        }
        
        /// <summary>
        /// Update an existing edge definition of the graph.
        /// </summary>
        public ArangoGraph updateEdgeDefinition(ArangoGraphEdgeDefinition edgeDefinition) {
            ArangoGraph g = _graphGraphOps.updateEdgeDefinition(edgeDefinition);
            this.updateGraph(g);
            return g;
        }
        
        /// <summary>
        /// Delete an existing edge definition of the graph. Collection will not be dropped.
        /// </summary>
        public ArangoGraph deleteEdgeDefinition(string edgeCollectionName) {
            return this.deleteEdgeDefinition(edgeCollectionName, false);
        }
        
        /// <summary>
        /// Delete an existing edge definition of the graph. If dropCollection is true, 
        /// the edge collection will be dropped. All vertex collections are moved to the orphanage
        /// if thy are used in one ore more edge definitions.
        /// </summary>
        public ArangoGraph deleteEdgeDefinition(string edgeCollectionName, bool dropCollection) {
            ArangoGraph g = _graphGraphOps.deleteEdgeDefinition(edgeCollectionName, dropCollection);
            this.updateGraph(g);
            return g;
        }
        
        /// <summary>
        /// Add new vertex to a collection of the graph.
        /// </summary>
        public Document addVertex(string collectionName, Document vertex, bool waitForSync) {
            return _graphGraphOps.addVertex(collectionName, vertex, waitForSync);
        }
        
        /// <summary>
        /// Get a vertex with the given key if it is contained in the graph
        /// </summary>
        public Document getVertex(string collectionName, string key) {
            return _graphGraphOps.getVertex(collectionName, key);
        }
        
        /// <summary>
        /// Replaces a vertex with the given id by the content in the body. 
        /// This will only run successfully if the vertex is contained within the graph.
        /// </summary>
        public Document replaceVertex(
            string collectionName, 
            string key, 
            Document vertex, 
            bool waitForSync
        ) {
            return _graphGraphOps.replaceVertex(collectionName, key, vertex, waitForSync);
        }
        
        /// <summary>
        /// Updates a vertex with the given id by adding the content in the body. 
        /// This will only run successfully if the vertex is contained within the graph.
        /// </summary>
        public Document updateVertex(
            string collectionName, 
            string key, 
            Document vertex, 
            bool waitForSync, 
            bool keepNull
        ) {
            return _graphGraphOps.updateVertex(collectionName, key, vertex, waitForSync, keepNull);
        }
        
        /// <summary>
        /// Deletes a vertex with the given id, if it is contained within the graph. 
        /// Furthermore all edges connected to this vertex will be deleted.
        /// </summary>
        public void deleteVertex(string collectionName, string key, bool waitForSync) {
            _graphGraphOps.deleteVertex(collectionName, key, waitForSync);
        }
        
        /// <summary>
        /// Stores a new edge with the information contained within the body into the given collection.
        /// </summary>
        public Document addEdge(string collectionName, string fromId, string toId, Document edge, bool waitForSync) {
            return _graphGraphOps.addEdge(collectionName, fromId, toId, edge, waitForSync);
        }
        
        /// <summary>
        /// Loads an edge with the given id if it is contained within your graph.
        /// </summary>
        public Document getEdge(string collectionName, string key) {
            return _graphGraphOps.getEdge(collectionName, key);
        }
        
        /// <summary>
        /// Replaces an edge with the given id by the content in the body. 
        /// This will only run successfully if the edge is contained within the graph.
        /// </summary>
        public Document replaceEdge(
            string collectionName, 
            string key, 
            Document edge, 
            bool waitForSync
        ) {
            return _graphGraphOps.replaceEdge(collectionName, key, edge, waitForSync);
        }
        
        /// <summary>
        /// Updates an edge with the given id by adding the content in the body. 
        /// This will only run successfully if the edge is contained within the graph.
        /// </summary>
        public Document updateEdge(
            string collectionName, 
            string key, 
            Document edge, 
            bool waitForSync, 
            bool keepNull
        ) {
            return _graphGraphOps.updateEdge(collectionName, key, edge, waitForSync, keepNull);
        }
        
        /// <summary>
        /// Deletes an edge with the given id, if it is contained within the graph. 
        /// </summary>
        public void deleteEdge(string collectionName, string key, bool waitForSync) {
            _graphGraphOps.deleteEdge(collectionName, key, waitForSync);
        }
        
    }
}
