
namespace Arango.Client.Protocol
{
    internal static class AQL
    {
        // standard high level operations
        internal const string COLLECT = "COLLECT";
        internal const string FILTER = "FILTER";
        internal const string FOR = "FOR";
        internal const string IN = "IN";
        internal const string INTO = "INTO";
        internal const string LET = "LET";
        internal const string LIMIT = "LIMIT";
        internal const string RETURN = "RETURN";
        internal const string SORT = "SORT";

        // standard functions
        internal const string CONCAT = "CONCAT";
        internal const string CONTAINS = "CONTAINS";
        internal const string DOCUMENT = "DOCUMENT";
        internal const string EDGES = "EDGES";
        internal const string FIRST = "FIRST";
        internal const string LENGTH = "LENGTH";
        internal const string LOWER = "LOWER";
        internal const string TO_BOOL = "TO_BOOL";
        internal const string TO_LIST = "TO_LIST";
        internal const string TO_NUMBER = "TO_NUMBER";
        internal const string TO_STRING = "TO_STRING";
        internal const string UPPER = "UPPER";
        // graph
        internal const string GRAPH_JSON = "GRAPH_JSON";
        internal const string GRAPH_EDGES = "GRAPH_EDGES";
        internal const string GRAPH_VERTICES = "GRAPH_VERTICES";
        internal const string GRAPH_NEIGHBORS = "GRAPH_NEIGHBORS";
        internal const string GRAPH_COMMON_NEIGHBORS = "GRAPH_COMMON_NEIGHBORS";
        internal const string GRAPH_COMMON_PROPERTIES = "GRAPH_COMMON_PROPERTIES";
        internal const string GRAPH_PATHS = "GRAPH_PATHS";
        internal const string GRAPH_SHORTEST_PATH = "GRAPH_SHORTEST_PATH";
        internal const string GRAPH_TRAVERSAL = "GRAPH_TRAVERSAL";
        internal const string GRAPH_TRAVERSAL_TREE = "GRAPH_TRAVERSAL_TREE";
        internal const string GRAPH_DISTANCE_TO = "GRAPH_DISTANCE_TO";
        internal const string GRAPH_ABSOLUTE_ECCENTRICITY = "GRAPH_ABSOLUTE_ECCENTRICITY";
        internal const string GRAPH_ECCENTRICITY = "GRAPH_ECCENTRICITY";
        internal const string GRAPH_ABSOLUTE_CLOSENESS = "GRAPH_ABSOLUTE_CLOSENESS";
        internal const string GRAPH_CLOSENESS = "GRAPH_CLOSENESS";
        internal const string GRAPH_ABSOLUTE_BETWEENNESS = "GRAPH_ABSOLUTE_BETWEENNESS";
        internal const string GRAPH_BETWEENNESS = "GRAPH_BETWEENNESS";
        internal const string GRAPH_RADIUS = "GRAPH_RADIUS";
        internal const string GRAPH_DIAMETER = "GRAPH_DIAMETER";

        // symbols
        internal const string AND = "&&";
        internal const string NOT = "!";
        internal const string OR = "||";

        // internal operations
        internal const string Collection = "COLLECTION";
        internal const string Condition = "CONDITION";
        internal const string Field = "FIELD";
        internal const string List = "LIST";
        internal const string ListExpression = "LISTEXPRESSION";
        internal const string Object = "OBJECT";
        internal const string Root = "ROOT";
        internal const string SortDirection = "SORTDIRECTION";
        internal const string String = "STRING";
        internal const string Val = "VAL";
        internal const string Var = "VAR";

        //Numeric functions
        /// <summary>
        /// FLOOR(value): Returns the integer closest but not greater to value
        /// </summary>
        internal const string FLOOR = "FLOOR";
        
        /// <summary>
        /// CEIL(value): Returns the integer closest but not less than value
        /// </summary>
        internal const string CEIL = "CEIL";
        
        /// <summary>
        /// ROUND(value): Returns the integer closest to value
        /// </summary>
        internal const string ROUND = "ROUND";
        
        /// <summary>
        /// ABS(value): Returns the absolute part of value
        /// </summary>
        internal const string ABS = "ABS";
        
        /// <summary>
        /// SQRT(value): Returns the square root of value
        /// </summary>
        internal const string SQRT = "SQRT";

        /// <summary>
        /// RAND(): Returns a pseudo-random number between 0 and 1
        /// </summary>
        internal const string RAND = "RAND";

        /// <summary>
        /// DATE_TIMESTAMP(date): Creates a UTC timestamp value from date. 
        /// DATE_TIMESTAMP(year, month, day, hour, minute, second, millisecond): Same as before, but allows specifying the individual date components separately. All parameters after day are optional.
        /// </summary>
        internal const string DATE_TIMESTAMP = "DATE_TIMESTAMP";
        
        /// <summary>
        /// DATE_ISO8601(date): Returns an ISO8601 date time string from date. The date time string will always use UTC time, indicated by the Z at its end.
        /// DATE_ISO8601(year, month, day, hour, minute, second, millisecond): same as before, but allows specifying the individual date components separately. All parameters after day are optional.
        /// </summary>
        internal const string DATE_ISO8601 = "DATE_ISO8601";

        
        
    }
}
