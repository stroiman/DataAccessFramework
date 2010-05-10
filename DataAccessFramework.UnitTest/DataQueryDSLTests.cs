using NUnit.Framework;

namespace DataAccessFramework.UnitTest
{
	public class UserTable : QueryTable
	{
		public UserTable() : base("User")
		{}

		public FieldReference ID { get { return new FieldReference(this, "ID"); } }

		public FieldReference FirstName { get { return new FieldReference(this, "Name"); } }
	}

	// New suite of tests that helps a move towards a fluent interface
	[TestFixture]
	public class DataQueryDslTests : DataQueryTestBase
	{
		private UserTable _userTable;
		
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			_userTable = new UserTable();
		}

		[Test]
		public void SelectByID()
		{
			// Setup
			var query = new DataQuery();
			query.AddTable(_userTable);
			query.AddWhere(_userTable.ID.EqualTo(1));
			const string expected = "select * from [User] t1 where t1.[ID]=@p1";

			// Exercise
			Execute(query);

			// Validate
			Assert.That(ExecutedSql, Is.EqualTo(expected));
		}
	}
}
