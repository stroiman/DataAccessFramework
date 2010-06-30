using System;
using System.Linq;
using DataAccessFramework.Querying;
using NUnit.Framework;

namespace DataAccessFramework.UnitTest
{
	public class MyEntity
	{
		public string Name { get; set; }
		public DateTime Date { get; set; }
		public int SomeID { get; set; }
		public decimal SomeDecimal { get; set; }
	}

	public class MyEntityTable : EntityTable<MyEntity>
	{
		public readonly FieldMapping<MyEntity> Name;
		public readonly FieldMapping<MyEntity> Date;
		public readonly FieldMapping<MyEntity> SomeID;
		public readonly FieldMapping<MyEntity> SomeDecimal;

		public MyEntityTable()
			: base("Entity")
		{
			Name = MapField("Name", x => x.Name);
			Date = MapField("Date", x => x.Date);
			SomeID = MapField("SomeID", x => x.SomeID);
			SomeDecimal = MapField("SomeDecimal", x => x.SomeDecimal);
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
			var expectedDate = DateTime.Now;
			const int expectedID = 42;
			var expectedDecimal = 123.45m;
			var entity = new MyEntity
			             	{
			             		Name = expectedName, 
								Date = expectedDate, 
								SomeID = expectedID,
								SomeDecimal = expectedDecimal
			             	};
			const string expectedSql = @"insert into [Entity] ([Name], [Date], [SomeID], [SomeDecimal]) values (@p1, @p2, @p3, @p4)";

			// Exercise
			var query = new MyEntityTable().Insert(entity);
			Execute(query);

			// Validate
			Assert.That(ExecutedSql, Is.EqualTo(expectedSql));
			var stringParameter = ExecutedParameters[0];
			var dateParameter = ExecutedParameters[1];
			var intParameter = ExecutedParameters[2];
			var decimalParameter = ExecutedParameters[3];

			Assert.That(stringParameter.ParameterName, Is.EqualTo("p1"));
			Assert.That(stringParameter.Value, Is.EqualTo(expectedName));

			Assert.That(dateParameter.ParameterName, Is.EqualTo("p2"));
			Assert.That(dateParameter.Value, Is.EqualTo(expectedDate));

			Assert.That(intParameter.ParameterName, Is.EqualTo("p3"));
			Assert.That(intParameter.Value, Is.EqualTo(expectedID));

			Assert.That(decimalParameter.ParameterName, Is.EqualTo("p4"));
			Assert.That(decimalParameter.Value, Is.EqualTo(expectedDecimal));
		}
	}
}