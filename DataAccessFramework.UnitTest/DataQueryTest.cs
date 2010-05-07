using System.Data;
using Moq;
using NUnit.Framework;

namespace DataAccessFramework.UnitTest
{
	[TestFixture]
	public class DataQueryTest
	{
		private Mock<DataTool> _dataToolMock = new Mock<DataTool>();
		private string _executedSql;
		private IDataParameter[] _executedParameters;

		[SetUp]
		public void Setup()
		{
			_dataToolMock = new Mock<DataTool> { CallBase = true };
			_dataToolMock.Setup(x => x.ExecuteSqlReader(It.IsAny<string>(), It.IsAny<IDataParameter[]>()))
				.Callback((string sql, IDataParameter[] parameters) =>
				{
					_executedSql = sql;
					_executedParameters = parameters;
				});
			_dataToolMock.Setup(x => x.CreateIntParameter(It.IsAny<string>(), It.IsAny<int?>()))
				.Returns((string name, int? value) =>
				{
					var parameterMock = new Mock<IDataParameter>();
					parameterMock.Setup(y => y.ParameterName).Returns(name);
					parameterMock.Setup(y => y.Value).Returns(value);
					return parameterMock.Object;
				});
		}

		[Test]
		public void Create_Query_With_One_Table()
		{
			// Setup
			DataQuery dataQuery = new DataQuery();
			QueryTable table = new QueryTable("table");
			dataQuery.AddTable(table);

			// Execute
			_dataToolMock.Object.ExecuteQuery(dataQuery);

			// Validate
			Assert.AreEqual("select * from [table] t1", _executedSql);
		}

		[Test]
		public void Create_Query_With_One_Table_And_Int_Parameter()
		{
			// Setup
			DataQuery dataQuery = new DataQuery();
			QueryTable table = new QueryTable("table");
			var clause = new EqualsClause(new FieldReference(table, "field"), new IntConstant(5));
			dataQuery.AddWhere(clause);
			dataQuery.AddTable(table);

			// Execute
			_dataToolMock.Object.ExecuteQuery(dataQuery);

			// Validate
			Assert.AreEqual("select * from [table] t1 where t1.[field]=@p1", _executedSql);
			Assert.AreEqual(1, _executedParameters.Length);
			Assert.AreEqual("p1", _executedParameters[0].ParameterName);
		}

		[Test]
		public void Create_Query_With_One_Table_And_Two_Parameters()
		{
			// Setup
			DataQuery dataQuery = new DataQuery();
			QueryTable table = new QueryTable("table");
			var clause = new AndClause(new[]
			{
				new EqualsClause(new FieldReference(table, "field"), new IntConstant(5)),
				new EqualsClause(new FieldReference(table, "field2"), new IntConstant(10))
			});
			dataQuery.AddWhere(clause);
			dataQuery.AddTable(table);

			// Execute
			_dataToolMock.Object.ExecuteQuery(dataQuery);

			// Validate
			Assert.AreEqual("select * from [table] t1 where (t1.[field]=@p1 AND t1.[field2]=@p2)", _executedSql);
			Assert.AreEqual(2, _executedParameters.Length);
			Assert.AreEqual("p1", _executedParameters[0].ParameterName);
			Assert.AreEqual("p2", _executedParameters[1].ParameterName);
		}

		[Test]
		public void Create_Query_With_Two_Tables()
		{
			// Setup
			DataQuery dataQuery = new DataQuery();
			QueryTable t1 = new QueryTable("table1");
			QueryTable t2 = new QueryTable("table2");
			dataQuery.AddTable(t1);
			dataQuery.AddTable(t2);

			// Execute
			_dataToolMock.Object.ExecuteQuery(dataQuery);

			// Validate
			Assert.AreEqual("select * from [table1] t1, [table2] t2", _executedSql);
		}

		[Test]
		public void Create_FullText_Query()
		{
			// Setup
			DataQuery dataQuery = new DataQuery();
			QueryTable t1 = new QueryTable("table1");
			var clause = new FullTextClause("Peter", new FieldReference(t1, "field"));
			dataQuery.AddTable(t1);
			dataQuery.AddWhere(clause);

			// Execute
			_dataToolMock.Object.ExecuteQuery(dataQuery);

			// Validate
			Assert.AreEqual(
				"select * from [table1] t1 where contains(t1.[field],@p1)",
				_executedSql);
		}

		[Test]
		public void Fulltext_Clause_With_Only_Wildcard_Character_Is_Ignored()
		{
			// Setup
			DataQuery dataQuery = new DataQuery();
			QueryTable t1 = new QueryTable("table1");
			var clause = new FullTextClause("*", new FieldReference(t1, "field"));
			dataQuery.AddTable(t1);
			dataQuery.AddWhere(clause);

			// Execute
			_dataToolMock.Object.ExecuteQuery(dataQuery);

			// Validate
			Assert.AreEqual(
				"select * from [table1] t1",
				_executedSql);
		}
	}
}