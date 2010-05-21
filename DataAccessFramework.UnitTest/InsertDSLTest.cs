using System;
using System.Linq;
using DataAccessFramework.Querying;
using NUnit.Framework;

namespace DataAccessFramework.UnitTest
{
	public class MyEntity
	{
		public string Name { get; set; }
	}

	public class FieldMapping<T> : FieldReference
	{
		private readonly Func<T, object> _getValue;

		public FieldMapping(
			QueryTable table, string fieldName, Func<T, object> getValue)
			: base(table, fieldName)
		{
			_getValue = getValue;
		}
	}

	public class EntityTable<T> : QueryTable
	{
		public EntityTable(string tableName,
			params FieldMapping<T>[] mappings)
			: base(tableName)
		{ }

		protected FieldMapping<T> MapField(string fieldName, Func<T, object> getValue)
		{
			return new FieldMapping<T>(this, fieldName, getValue);
		}

		public DataQuery Insert(T entity)
		{
			return new DataQuery();
		}
	}

	public class MyEntityTable : EntityTable<MyEntity>
	{
		public readonly new FieldMapping<MyEntity> Name;

		public MyEntityTable()
			: base("Table")
		{
			Name = MapField("Name", x => x.Name);
		}
	}

	[TestFixture]
	public class InsertDslTest : DataQueryTestBase
	{
		[Test]
		public void InsertIntoTable()
		{
			// Setup
			const string expectedName = "name";
			var entity = new MyEntity { Name = expectedName };
			const string expectedSql = @"insert into [Entity] (Name) values(@p1)";

			// Exercise
			var query = new MyEntityTable().Insert(entity);
			Execute(query);

			// Validate
			Assert.That(ExecutedSql, Is.EqualTo(expectedSql));
			var parameter = ExecutedParameters.Single();
			Assert.That(parameter.ParameterName, Is.EqualTo("p1"));
			Assert.That(parameter.Value, Is.EqualTo(expectedName));
		}
	}
}