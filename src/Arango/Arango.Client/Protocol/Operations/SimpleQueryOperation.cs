using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Arango.Client.Protocol
{
    public class SimpleQueryOperationProtocol
    {
        private string _apiUri { get { return "_api/simple"; } }

		private Connection _connection { get; set; }

        internal SimpleQueryOperationProtocol(Connection connection)
		{
			_connection = connection;
		}

        #region PUT /_api/simple/all
        
        public List<object> All(Arango.Client.API.SimpleQueryOperation.AllRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/all");
        } 
        #endregion

        #region PUT /_api/simple/by-example

        public List<object> ByExample(Arango.Client.API.SimpleQueryOperation.ByExampleRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/by-example");
        }

        #endregion

        #region PUT /_api/simple/first-example

        public Document FirstExample(Arango.Client.API.SimpleQueryOperation.ByExampleRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return GetDocument(query, "/first-example");
        }

        #endregion

        #region PUT /_api/simple/by-example-hash

        public List<object> ByExampleHash(Arango.Client.API.SimpleQueryOperation.ByExampleIndexRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/by-example-hash");
        }

        #endregion

        #region PUT /_api/simple/by-example-skiplist

        public List<object> ByExampleSkipList(Arango.Client.API.SimpleQueryOperation.ByExampleIndexRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/by-example-skiplist");
        }

        #endregion

        #region PUT /_api/simple/by-example-bitarray

        public List<object> ByExampleBitArray(Arango.Client.API.SimpleQueryOperation.ByExampleIndexRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/by-example-bitarray");
        }

        #endregion

        #region PUT /_api/simple/by-condition-skiplist

        public List<object> ByConditionSkipList(Arango.Client.API.SimpleQueryOperation.ByConditionIndexRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/by-condition-skiplist");
        }

        #endregion

        #region PUT /_api/simple/by-condition-bitarray

        public List<object> ByConditionBitArray(Arango.Client.API.SimpleQueryOperation.ByConditionIndexRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/by-condition-bitarray");
        }

        #endregion

        #region PUT /_api/simple/any

        public Document Any(Arango.Client.API.SimpleQueryOperation.AnyRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return GetDocument(query, "/any");
        }

        #endregion

        #region PUT /_api/simple/range

        public List<object> Range(Arango.Client.API.SimpleQueryOperation.RangeRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/range");
        }

        #endregion

        #region PUT /_api/simple/near

        public List<object> Near(Arango.Client.API.SimpleQueryOperation.NearRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/near");
        }

        #endregion

        #region PUT /_api/simple/within

        public List<object> Within(Arango.Client.API.SimpleQueryOperation.WithinRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/within");
        }

        #endregion

        #region PUT /_api/simple/fulltext

        public List<object> FullText(Arango.Client.API.SimpleQueryOperation.FullTextRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/fulltext");
        }

        #endregion

        #region PUT /_api/simple/remove-by-example

        public int RemoveByExample(Arango.Client.API.SimpleQueryOperation.RemoveByExampleRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return GetDocumentIntFieldValue(query, "/remove-by-example", "deleted");
        }

        #endregion

        #region PUT /_api/simple/update-by-example

        public int UpdateByExample(Arango.Client.API.SimpleQueryOperation.UpdateByExampleRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return GetDocumentIntFieldValue(query, "/update-by-example", "updated");
        }

        #endregion

        #region PUT /_api/simple/first

        public List<object> First(Arango.Client.API.SimpleQueryOperation.FirstLastRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/first");
        }

        #endregion

        #region PUT /_api/simple/last

        public List<object> Last(Arango.Client.API.SimpleQueryOperation.FirstLastRequest request)
        {
            var query = JsonConvert.SerializeObject(request);
            return Execute(query, "/last");
        }

        #endregion
            
        private List<dynamic> Execute(string query, string urlPath)
        {
            var request = new Request(RequestType.Cursor, HttpMethod.Put);
            request.RelativeUri = _apiUri + urlPath;
            request.Body = query;

            var response = _connection.Process(request);
            var items = new List<dynamic>();

            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    items.AddRange(response.Document.List<object>("result"));
                    break;
                default:
                    items = null;

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

            return items;
        }

        private Document GetDocument(string query, string urlPath)
        {
            var request = new Request(RequestType.Cursor, HttpMethod.Put);
            request.RelativeUri = _apiUri + urlPath;
            request.Body = query;

            var response = _connection.Process(request);
            var document = new Document();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    document = response.Document.Object("document");
                    break;
                case HttpStatusCode.NotModified:
                    document.String("_rev", response.Headers.Get("etag").Replace("\"", ""));
                    break;
                case HttpStatusCode.NotFound:
                    document = null;
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

            return document;
        }

        private int GetDocumentIntFieldValue(string query, string urlPath, string field)
        {
            var request = new Request(RequestType.Cursor, HttpMethod.Put);
            request.RelativeUri = _apiUri + urlPath;
            request.Body = query;

            var response = _connection.Process(request);
            int result = 0;

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result = response.Document.Int(field);
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
    }
}
