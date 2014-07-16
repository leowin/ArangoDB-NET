using Arango.Client.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arango.Client.API
{
    public abstract class SimpleRequest
    {
        [JsonProperty("collection")]
        public string Collection { get; set; }

        [JsonProperty("skip")]
        public int Skip { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        public SimpleRequest()
        {
            Limit = Int32.MaxValue;
        }
    }

    public class SimpleQueryOperation
    {
        SimpleQueryOperationProtocol _protocol;

        public SimpleQueryOperation(SimpleQueryOperationProtocol protocol)
        {
            _protocol = protocol;
        }


        #region PUT /_api/simple/all

        public class AllRequest : SimpleRequest
        {

        }

        public List<Document> All(AllRequest request)
        {
            var list =_protocol.All(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> All<T>(AllRequest request)
        {
            var list = All(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/by-example

        public class ByExampleRequest : SimpleRequest
        {
            [JsonProperty("example")]
            public dynamic Example { get; set; }
        }

        public List<Document> ByExample(ByExampleRequest request)
        {
            var list = _protocol.ByExample(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> ByExample<T>(ByExampleRequest request)
        {
            var list = ByExample(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/first-example

        public Document FirstExample(ByExampleRequest request)
        {
            var document = _protocol.FirstExample(request);
            return document;
        }

        public T FirstExample<T>(ByExampleRequest request) where T : class, new()
        {
            var document = FirstExample(request);
            var obj = (T)document.ToObject<T>();

            document.MapAttributesTo(obj);

            return obj;
        }

        #endregion

        #region PUT /_api/simple/by-example-hash

        public class ByExampleIndexRequest : SimpleRequest
        {
            [JsonProperty("example")]
            public dynamic Example { get; set; }

            [JsonProperty("index")]
            public string Index { get; set; }
        }

        public List<Document> ByExampleHash(ByExampleIndexRequest request)
        {
            var list = _protocol.ByExampleHash(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> ByExampleHash<T>(ByExampleIndexRequest request)
        {
            var list = ByExampleHash(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/by-example-skiplist

        public List<Document> ByExampleSkipList(ByExampleIndexRequest request)
        {
            var list = _protocol.ByExampleSkipList(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> ByExampleSkipList<T>(ByExampleIndexRequest request)
        {
            var list = ByExampleSkipList(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/by-example-bitarray

        public List<Document> ByExampleBitArray(ByExampleIndexRequest request)
        {
            var list = _protocol.ByExampleBitArray(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> ByExampleBitArray<T>(ByExampleIndexRequest request)
        {
            var list = ByExampleBitArray(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/by-condition-skiplist

        public class ByConditionIndexRequest : SimpleRequest
        {
            [JsonProperty("condition")]
            public dynamic Condition { get; set; }

            [JsonProperty("index")]
            public string Index { get; set; }
        }

        public List<Document> ByConditionSkipList(ByConditionIndexRequest request)
        {
            var list = _protocol.ByConditionSkipList(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> ByConditionSkipList<T>(ByConditionIndexRequest request)
        {
            var list = ByConditionSkipList(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/by-condition-bitarray

        public List<Document> ByConditionBitArray(ByConditionIndexRequest request)
        {
            var list = _protocol.ByConditionBitArray(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> ByConditionBitArray<T>(ByConditionIndexRequest request)
        {
            var list = ByConditionBitArray(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/Any

        public class AnyRequest : SimpleRequest
        {
        }

        public Document Any(AnyRequest request)
        {
            var document = _protocol.Any(request);
            return document;
        }

        public T FirstExample<T>(AnyRequest request) where T : class, new()
        {
            var document = Any(request);
            var obj = (T)document.ToObject<T>();

            document.MapAttributesTo(obj);

            return obj;
        }

        #endregion

        #region PUT /_api/simple/range

        public class RangeRequest : SimpleRequest
        {
            [JsonProperty("attribute")]
            public string Attribute { get; set; }

            [JsonProperty("left")]
            public int Left { get; set; }

            [JsonProperty("right")]
            public int Right { get; set; }

            [JsonProperty("closed")]
            public bool Closed { get; set; }

        }

        public List<Document> Range(RangeRequest request)
        {
            var list = _protocol.Range(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> Range<T>(RangeRequest request)
        {
            var list = Range(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/near

        public class NearRequest : SimpleRequest
        {
            [JsonProperty("latitude")]
            public double Latitude { get; set; }

            [JsonProperty("longitude")]
            public double Longitude { get; set; }

            [JsonProperty("distance")]
            public string Distance { get; set; }

            [JsonProperty("geo")]
            public string Geo { get; set; }

        }

        public List<Document> Near(NearRequest request)
        {
            var list = _protocol.Near(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> Near<T>(NearRequest request)
        {
            var list = Near(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/within

        public class WithinRequest : NearRequest
        {
            [JsonProperty("radius")]
            public double Radius { get; set; }

        }

        public List<Document> Within(WithinRequest request)
        {
            var list = _protocol.Within(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> Within<T>(WithinRequest request)
        {
            var list = Within(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/fulltext

        public class FullTextRequest : SimpleRequest
        {
            [JsonProperty("attribute")]
            public string Attribute { get; set; }

            [JsonProperty("query")]
            public string Query { get; set; }

            [JsonProperty("index")]
            public string Index { get; set; }

        }

        public List<Document> FullText(FullTextRequest request)
        {
            var list = _protocol.FullText(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> FullText<T>(FullTextRequest request)
        {
            var list = FullText(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/remove-by-example

        public class RemoveByExampleRequest : SimpleRequest
        {
            [JsonProperty("example")]
            public dynamic Example { get; set; }

            [JsonProperty("waitForSync")]
            public bool WaitForSync { get; set; }

        }

        public int RemoveByExample(RemoveByExampleRequest request)
        {
            return _protocol.RemoveByExample(request);
        }

        #endregion

        #region PUT /_api/simple/update-by-example

        public class UpdateByExampleRequest : SimpleRequest
        {
            [JsonProperty("example")]
            public dynamic Example { get; set; }

            [JsonProperty("newValue")]
            public dynamic NewValue { get; set; }

            [JsonProperty("keepNull")]
            public bool KeepNull { get; set; }

            [JsonProperty("waitForSync")]
            public bool WaitForSync { get; set; }

        }

        public int UpdateByExample(UpdateByExampleRequest request)
        {
            return _protocol.UpdateByExample(request);
        }

        #endregion

        #region PUT /_api/simple/first

        public class FirstLastRequest : SimpleRequest
        {
            [JsonProperty("count")]
            public int Count { get; set; }

        }

        public List<Document> First(FirstLastRequest request)
        {
            var list = _protocol.First(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> First<T>(FirstLastRequest request)
        {
            var list = First(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region PUT /_api/simple/last

        public List<Document> Last(FirstLastRequest request)
        {
            var list = _protocol.Last(request);
            return list.Cast<Document>().ToList();
        }

        public List<T> Last<T>(FirstLastRequest request)
        {
            var list = Last(request);
            return ToGenericList<T>(list);
        }

        #endregion

        #region ToList

        private List<T> ToGenericList<T>(List<Document> items)
        {
            var type = typeof(T);
            var genericCollection = new List<T>();

            if (type.IsPrimitive ||
                (type == typeof(string)) ||
                (type == typeof(DateTime)) ||
                (type == typeof(decimal)))
            {
                foreach (object item in items)
                {
                    genericCollection.Add((T)Convert.ChangeType(item, type));
                }
            }
            else if (type == typeof(Document))
            {
                genericCollection = items.Cast<T>().ToList();
            }
            else
            {
                foreach (object item in items)
                {
                    var document = (Document)item;
                    var genericObject = Activator.CreateInstance(type);

                    document.ToObject(genericObject);
                    document.MapAttributesTo(genericObject);

                    genericCollection.Add((T)genericObject);
                }
            }

            return genericCollection;
        }

        #endregion
    }
}
