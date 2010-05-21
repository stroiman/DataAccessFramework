using System;
using System.Collections.Generic;
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

		public Func<T, object> GetValue
		{
			get { return _getValue; }
		}
	}

	public class EntityTable<T> : QueryTable
	{
		private List<FieldMapping<T>> _fields = new List<FieldMapping<T>>();

		public EntityTable(string tableName)
			: base(tableName)
		{ }

		protected FieldMapping<T> MapField(string fieldName, Func<T, object> getValue)
		{
			var result = new FieldMapping<T>(this, fieldName, getValue);
			_fields.Add(result);
			return result;
		}

		public Query Insert(T entity)
		{
			return new InsertQuery(base.TableName, GetFields(entity));
		}

		private IEnumerable<Tuple<string, object>> GetFields(T entity)
		{
			return _fields.Select(x =>
				new Tuple<string, object>(
					x.FieldName,
					x.GetValue(entity))
					);
		}
	}

	public class MyEntityTable : EntityTable<MyEntity>
	{
		public readonly new FieldMapping<MyEntity> Name;

		public MyEntityTable()
			: base("Entity")
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
			const string expectedSql = @"insert into [Entity] (Name) values (@p1)";

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