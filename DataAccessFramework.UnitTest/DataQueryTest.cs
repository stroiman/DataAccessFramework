using DataAccessFramework.Querying;
using NUnit.Framework;

namespace DataAccessFramework.UnitTest
{
	[TestFixture]
	public class DataQueryTest : DataQueryTestBase
	{
		[Test]
		public void Create_Query_With_One_Table()
		{
			// Setup
			var dataQuery = CreateSelectQuery();
			var table = new QueryTable("table");
			dataQuery.AddTable(table);

			// Execute
			Execute(dataQuery);

			// Validate
			Assert.AreEqual("select * from [table] t1", ExecutedSql);
		}

		[Test]
		public void Create_Query_With_One_Table_And_Int_Parameter()
		{
			// Setup
			var dataQuery = CreateSelectQuery();
			var table = new QueryTable("table");
			var clause = new EqualsClause(new FieldReference(table, "field"), new IntConstant(5));
			dataQuery.AddWhere(clause);
			dataQuery.AddTable(table);

			// Execute
			Execute(dataQuery);

			// Validate
			Assert.AreEqual("select * from [table] t1 where t1.[field]=@p1", ExecutedSql);
			Assert.AreEqual(1, ExecutedParameters.Length);
			Assert.AreEqual("p1", ExecutedParameters[0].ParameterName);
		}

		[Test]
		public void Create_Query_With_One_Table_And_Two_Parameters()
		{
			// Setup
			var dataQuery = CreateSelectQuery();
			var table = new QueryTable("table");
			var clause = new AndClause(new[]
			{
				new EqualsClause(new FieldReference(table, "field"), new IntConstant(5)),
				new EqualsClause(new FieldReference(table, "field2"), new IntConstant(10))
			});
			dataQuery.AddWhere(clause);
			dataQuery.AddTable(table);

			// Execute
			Execute(dataQuery);

			// Validate
			Assert.AreEqual("select * from [table] t1 where (t1.[field]=@p1 AND t1.[field2]=@p2)", ExecutedSql);
			Assert.AreEqual(2, ExecutedParameters.Length);
			Assert.AreEqual("p1", ExecutedParameters[0].ParameterName);
			Assert.AreEqual("p2", ExecutedParameters[1].ParameterName);
		}

		[Test]
		public void Create_Query_With_Two_Tables()
		{
			// Setup
			var dataQuery = CreateSelectQuery();
			var t1 = new QueryTable("table1");
			var t2 = new QueryTable("table2");
			dataQuery.AddTable(t1);
			dataQuery.AddTable(t2);

			// Execute
			Execute(dataQuery);

			// Validate
			Assert.AreEqual("select * from [table1] t1, [table2] t2", ExecutedSql);
		}

		[Test]
		public void Create_FullText_Query()
		{
			// Setup
			var dataQuery = CreateSelectQuery();
			var t1 = new QueryTable("table1");
			var clause = new FullTextClause("Peter", new FieldReference(t1, "field"));
			dataQuery.AddTable(t1);
			dataQuery.AddWhere(clause);

			// Execute
			Execute(dataQuery);

			// Validate
			Assert.AreEqual(
				"select * from [table1] t1 where contains(t1.[field],@p1)",
				ExecutedSql);
		}

		[Test]
		public void Fulltext_Clause_With_Only_Wildcard_Character_Is_Ignored()
		{
			// Setup
			var dataQuery = CreateSelectQuery();
			var t1 = new QueryTable("table1");
			var clause = new FullTextClause("*", new FieldReference(t1, "field"));
			dataQuery.AddTable(t1);
			dataQuery.AddWhere(clause);

			// Execute
			Execute(dataQuery);

			// Validate
			Assert.AreEqual(
				"select * from [table1] t1",
				ExecutedSql);
		}
	}
}