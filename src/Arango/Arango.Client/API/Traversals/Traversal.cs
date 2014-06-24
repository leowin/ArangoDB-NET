using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arango.Client.API.Traversals
{
    public class Traversal
    {
        public string StartVertex { get; set; }
        public string EdgeCollection { get; set; }
        public string Filter { get; set; }
        public int MinDepth { get; set; }
        public int MaxDepth { get; set; }
        public string Visitor { get; set; }
        public TraversalDirection Direction { get; set; }
        public string Init { get; set; }
        public string Expander { get; set; }
        public string Sort { get; set; }
        public TraversalStrategy Strategy { get; set; }
        public TraversalOrder Order { get; set; }
        public TraversalItemOrder ItemOrder { get; set; }   

    }

    public enum TraversalDirection
    {
        Outbound, Inbound, Any  
    }

    public enum TraversalStrategy
    {
        DepthFirst, BreadthFirst
    }

    public enum TraversalOrder
    {
        PreOrder, PostOrder
    }

    public enum TraversalItemOrder
    {
        Forward, Backward
    }
}
