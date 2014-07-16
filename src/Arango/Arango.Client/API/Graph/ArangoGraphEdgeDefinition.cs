/*
 * Created by SharpDevelop.
 * User: gschwab
 * Date: 03.07.2014
 * Time: 14:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
namespace Arango.Client
{
    /// <summary>
    /// Description of ArangoGraphEdgeDefinition.
    /// </summary>
    public class ArangoGraphEdgeDefinition
    {
        public string collection { get; set; }
        public List<String> from { get; set; }
        public List<String> to { get; set; }
        
        public ArangoGraphEdgeDefinition(string collection, List<String> from, List<String> to) {
            this.collection = collection;
            this.from = from;
            this.to = to;
        }
    }
}
