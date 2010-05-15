using System;
using DataAccessFramework.Querying;
using NUnit.Framework;

namespace DataAccessFramework.UnitTest
{
	public class UserTable : QueryTable
	{
		public UserTable()
			: base("User")
		{ }

		public FieldReference ID { get { return new FieldReference(this, "ID"); } }

		public FieldReference FirstName { get { return new FieldReference(this, "Name"); } }
	}

	public class BlogTable : QueryTable
	{
		public BlogTable()
			: base("Blog")
		{
		}

		public FieldReference ID { get { return new FieldReference(this, "ID"); } }

		public FieldReference UserID { get { return new FieldReference(this, "UserID"); } }
	}

	public class BlogEntryTable : QueryTable
	{
		public BlogEntryTable()
			: base("BlogEntry")
		{ }

		public FieldReference ID { get { return new FieldReference(this, "ID"); } }

		public FieldReference BlogID { get { return new FieldReference(this, "BlogID"); } }
	}

	// New suite of tests that helps a move towards a fluent interface
	[TestFixture]
	public class DataQueryDslTests : DataQueryTestBase
	{
		private UserTable _userTable;
		private BlogTable _blogTable;
		private BlogEntryTable _blogEntryTable;

		[SetUp]
		public void FixtureSetup()
		{
			_userTable = new UserTable();
			_blogTable = new BlogTable();
			_blogEntryTable = new BlogEntryTable();
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

		[Test]
		public void LeftJoin()
		{
			// Setup
			var query = new DataQuery();
			query.AddTable(
				_userTable
				.LeftJoin(_blogTable)
				.On(
					_userTable.ID.EqualTo(_blogTable.UserID)
				));
			const string expected = "select * from [User] t1 left outer join [Blog] t2 on t1.[ID]=t2.[UserID]";

			// Exercise
			Execute(query);

			// Validate
			Assert.That(ExecutedSql, Is.EqualTo(expected));
		}

		[Test]
		public void DoubleLeftJoin()
		{
			var query = new DataQuery();
			query.AddTable(
				_userTable
				.LeftJoin(_blogTable)
				.On(_userTable.ID.EqualTo(_blogTable.UserID))
				.LeftJoin(_blogEntryTable)
				.On(_blogTable.ID.EqualTo(_blogEntryTable.BlogID)));
			const string expected = 
				"select * from [User] t1 left outer join [Blog] t2 on t1.[ID]=t2.[UserID] " + 
				"left outer join [BlogEntry] t3 on t2.[ID]=t3.[BlogID]";

			// Exercise
			Execute(query);

			// Validate
			Assert.That(ExecutedSql, Is.EqualTo(expected));
		}
	}
}