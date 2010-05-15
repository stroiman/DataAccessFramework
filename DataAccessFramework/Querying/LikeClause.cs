namespace DataAccessFramework.Querying
{
	/// <summary>
	/// A where clause for a LIKE statement
	/// </summary>
	public class LikeClause : WherePart
	{
		private readonly WherePart _left;
		private readonly StringConstant _right;

		/// <summary>
		/// Creates a new <c>LikeClause</c> instance
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public LikeClause(WherePart left, string right)
		{
			_left = left;
			_right = new StringConstant(right + "%");
		}

		/// <summary>
		/// Builds the sql for the clause.
		/// </summary>
		internal override void BuildSql(BuildSqlContext sqlContext)
		{
			_left.BuildSql(sqlContext);
			sqlContext.Builder.Append(" LIKE ");
			_right.BuildSql(sqlContext);
		}
	}
}