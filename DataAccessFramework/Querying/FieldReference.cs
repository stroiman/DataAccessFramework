namespace DataAccessFramework.Querying
{
	/// <summary>
	/// A part in a where clause that references a field in one
	/// of the included tables.
	/// </summary>
	public class FieldReference : WherePart
	{
		private readonly QueryTable _table;
		private readonly string _fieldName;

		/// <summary>
		/// Creates a new <c>FieldReference</c> instance.
		/// </summary>
		/// <param name="table">The table that we are referencing</param>
		/// <param name="fieldName">The name of the field</param>
		public FieldReference(QueryTable table, string fieldName)
		{
			_table = table;
			_fieldName = fieldName;
		}

		/// <summary>
		/// Builds the SQL for this field reference.
		/// </summary>
		internal override void BuildSql(BuildSqlContext context)
		{
			context.Builder.Append(context.ResolveAlias(_table));
			context.Builder.Append(".[");
			context.Builder.Append(_fieldName);
			context.Builder.Append("]");
		}

		/// <summary>
		/// Gets the referenced table. Mainly exists for
		/// unit testing so data query generation logic can be validated
		/// </summary>
		public QueryTable Table { get { return _table; } }

		/// <summary>
		/// Gets the name of the referenced field. Mainly exists for
		/// unit testing so data query generation logic can be validated
		/// </summary>
		public string FieldName { get { return _fieldName; } }

		/// <summary>
		/// Generates an <see cref="EqualsClause"/> for this field
		/// </summary>
		public EqualsClause EqualTo(int value)
		{
			return new EqualsClause(this, new IntConstant(value));
		}

		/// <summary>
		/// Generates an <see cref="EqualsClause"/> for this field
		/// </summary>
		public EqualsClause EqualTo(long value)
		{
			return new EqualsClause(this, new LongConstant(value));
		}

		public EqualsClause EqualTo(FieldReference field)
		{
			return new EqualsClause(this, field);
		}
	}
}