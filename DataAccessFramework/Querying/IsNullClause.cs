namespace DataAccessFramework.Querying
{
	/// <summary>
	/// A where clause for specifying that a column should be null.
	/// </summary>
	public class IsNullClause : WherePart
	{
		private readonly FieldReference _field;

		/// <summary>
		/// Creates a new <see cref="IsNullClause"/> instance.
		/// </summary>
		/// <param name="field">A reference to the field that should be null.</param>
		public IsNullClause(FieldReference field)
		{
			_field = field;
		}

		/// <summary>
		/// Implements generating the sql for the clause
		/// </summary>
		internal override void BuildSql(BuildSqlContext context)
		{
			_field.BuildSql(context);
			context.Builder.Append(" IS NULL");
		}
	}
}