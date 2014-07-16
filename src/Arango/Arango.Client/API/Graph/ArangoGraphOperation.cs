using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    /// <summary>
    /// Description of ArangoGraph.
    /// </summary>
    public class ArangoGraphOperation
    {
        private GraphOperation _graphOperation;
        
        internal ArangoGraphOperation(GraphOperation graphOperation)
        {
            _graphOperation = graphOperation;
        }
        
        public List<ArangoGraph> Get()
        {
            return _graphOperation.Get();
        }

        public ArangoGraph Get(string name)
        {
            return _graphOperation.Get(name);
        }

        /// <summary>
        /// Create an empty graph.
        /// </summary>
        public ArangoGraph Create(string name)
        {
            return _graphOperation.Post(name);
        }

        /// <summary>
        /// Create a graph with edgeDefinitions.
        /// </summary>
        public ArangoGraph Create(string name, List<ArangoGraphEdgeDefinition> edgeDefinitions)
        {
            return _graphOperation.Post(name, edgeDefinitions);
        }

        /// <summary>
        /// Create a graph with edgeDefinitions and vertexCollections.
        /// </summary>
        public ArangoGraph Create(string name, List<ArangoGraphEdgeDefinition> edgeDefinitions, List<string> vertexCollections)
        {
            return _graphOperation.Post(name, edgeDefinitions, vertexCollections);
        }

        /// <summary>
        /// Create a graph with vertexCollections but no edgeDefinitions.
        /// </summary>
        public ArangoGraph Create(string name, List<string> vertexCollections)
        {
            return _graphOperation.Post(name, vertexCollections);
        }

        public void Delete(string name)
        {
            _graphOperation.Delete(name);
        }

        public void Delete(string name, bool dropCollections)
        {
            _graphOperation.Delete(name, dropCollections);
        }

        public bool Exists(string name)
        {
            return _graphOperation.Exists(name);
        }
    }
}
