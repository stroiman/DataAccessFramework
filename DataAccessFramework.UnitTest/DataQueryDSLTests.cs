using System;
using DataAccessFramework.Querying;
using NUnit.Framework;

namespace DataAccessFramework.UnitTest
{
	public class UserTable : QueryTable
	{
		public UserTable()
			: base("User")
		{
			ID = Field("ID");
			Name = Field("Name");
		}

		public readonly FieldReference ID;
		public readonly FieldReference Name;
	}

	public class BlogTable : QueryTable
	{
		public BlogTable()
			: base("Blog")
		{
			ID = Field("ID");
			UserID = Field("UserID");
		}

		public readonly FieldReference ID;
		public readonly FieldReference UserID;
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
		public void SelectingAnEntireTableShouldReturnExplicitRows()
		{
			// Setup
			var query = _userTable.Select();
			const string expected = "select [t1].[ID] as User_ID, [t1].[Name] as User_Name from [User] t1";

			// Exercise
			Execute(query);

			// Validate
			Assert.That(ExecutedSql, Is.EqualTo(expected));
		}

		[Test]
		public void SelectByID()
		{
			// Setup
			var query = _userTable.SelectWhere(_userTable.ID.EqualTo(1));
			const string expected = "select [t1].[ID] as User_ID, [t1].[Name] as User_Name from [User] t1 where t1.[ID]=@p1";

			// Exercise
			Execute(query);

			// Validate
			Assert.That(ExecutedSql, Is.EqualTo(expected));
		}

		[Test]
		public void LeftJoin()
		{
			// Setup
			var query = CreateSelectQuery();
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
		public void InnerJoin()
		{
			// Setup
			var query = CreateSelectQuery();
			query.AddTable(
				_userTable
				.InnerJoin(_blogTable)
				.On(
					_userTable.ID.EqualTo(_blogTable.UserID)
				));
			const string expected = "select * from [User] t1 inner join [Blog] t2 on t1.[ID]=t2.[UserID]";

			// Exercise
			Execute(query);

			// Validate
			Assert.That(ExecutedSql, Is.EqualTo(expected));
		}

		[Test]
		public void DoubleLeftJoin()
		{
			var query = CreateSelectQuery();
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

		[Test]
		public void SelectFromTable()
		{
			var query =
				_userTable
				.LeftJoin(_blogTable)
				.On(_userTable.ID.EqualTo(_blogTable.UserID))
				.SelectWhere(_userTable.ID.EqualTo(1));

			const string expected = "select [t1].[ID] as User_ID, [t1].[Name] as User_Name, [t2].[ID] as Blog_ID, [t2].[UserID] as Blog_UserID from [User] t1 left outer join [Blog] t2 on t1.[ID]=t2.[UserID] where t1.[ID]=@p1";

			// Exercise
			Execute(query);

			// Validate
			Assert.That(ExecutedSql, Is.EqualTo(expected));
		}
	}
}