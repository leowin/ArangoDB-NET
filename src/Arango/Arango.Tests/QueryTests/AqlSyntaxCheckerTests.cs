/*
 * Created by SharpDevelop.
 * User: lombeyda
 * Date: 17.06.2014
 * Time: 14:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using Arango.Client.Protocol;

namespace Arango.Tests.QueryTests
{
	/// <summary>
	///  implements the functionality for checking the syntax 
	/// of a query by arangod
	/// </summary>
	public class AqlSyntaxCheckerTests : AqlTests
	{
		public AqlSyntaxCheckerTests()
		{
			Database.CreateTestDatabase(Database.TestDatabaseGeneral);
			Database.CreateTestCollection("Dinners");
		}
		/// <summary>
		///  sends to arango the resquest api/query for checking the
		///  syntax of the given query
		/// </summary>

		public override void assertQuerySyntax (string queryStr) {
			QueryParseResult result = Database.GetTestDatabase().SyntaxChecker.Post(queryStr);
			Assert.IsFalse(result.error);
			Assert.AreEqual(200, result.code);
		}

	}
}
