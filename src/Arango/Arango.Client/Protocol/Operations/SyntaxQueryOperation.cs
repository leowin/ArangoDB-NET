/*
 * Created by SharpDevelop.
 * User: lombeyda
 * Date: 17.06.2014
 * Time: 12:57
 * 
 */
using System;
using System.Collections.Generic;
using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client.Protocol
{
	/// <summary>
	/// Description of QueryOperation.
	/// </summary>
	public class SyntaxQueryOperation
	{

		private string _apiUri { get { return "_api/query"; } }

		private Connection _connection { get; set; }

		internal SyntaxQueryOperation (Connection connection)
		{
			_connection = connection;
		}

#region POST

		public QueryParseResult Post(string query)
		{
			var request = new Request(RequestType.Cursor, HttpMethod.Post);
			request.RelativeUri = _apiUri;

			var bodyDocument = new Document();

			// set AQL string
			bodyDocument.String("query", query);

			request.Body = bodyDocument.Serialize();

			var response = _connection.Process(request);
			QueryParseResult parseResult = null;

			switch (response.StatusCode)
			{ 

				case HttpStatusCode.OK:
					parseResult = new QueryParseResult();
					parseResult.bindVars = response.Document.List<string>("bindVars");
					parseResult.collections = response.Document.List<string>("collections");
					// error
					parseResult.error = response.Document.Bool("error");
					parseResult.code = response.Document.Int("code");


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

			return parseResult;
		}

#endregion

	}
	/// <summary>
	/// contains the information which is yield by the
	/// Rest Class api/query
	/// </summary>
	public class QueryParseResult {


		public IList<string> bindVars;

		public IList<string> BindVars {
			get { return bindVars; }
			set { bindVars = value; }
		}
		public IList<string> collections;

		public IList<string> Collections {
			get { return collections; }
			set { collections = value; }
		}
		public bool error;		

		public bool Error {
			get { return error; }
			set { error = value; }
		}
		public int code;

		public int Code {
			get { return code; }
			set { code = value; }
		}
	}
}
