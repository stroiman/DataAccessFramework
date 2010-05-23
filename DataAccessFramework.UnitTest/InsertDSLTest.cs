using System.Linq;
using DataAccessFramework.Querying;
using NUnit.Framework;

namespace DataAccessFramework.UnitTest
{
	public class MyEntity
	{
		public string Name { get; set; }
	}

	public class MyEntityTable : EntityTable<MyEntity>
	{
		public readonly FieldMapping<MyEntity> Name;

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
			const string expectedSql = @"insert into [Entity] ([Name]) values (@p1)";

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