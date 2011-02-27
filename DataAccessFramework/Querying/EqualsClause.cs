namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Where clause for an equals operator.
	/// </summary>
	public class EqualsClause : WherePart
	{
		private WherePart _left;
		private WherePart _right;

		/// <summary>
		/// Creates a new <c>EqualsClause</c> instance
		/// </summary>
		public EqualsClause(WherePart left, WherePart right)
		{
			_left = left;
			_right = right;
		}

		/// <summary>
		/// Builds the SQL for this part.
		/// </summary>
		internal override void BuildSql(BuildSqlContext sqlContext)
		{
			_left.BuildSql(sqlContext);
			sqlContext.Builder.Append("=");
			_right.BuildSql(sqlContext);
		}

		/// <summary>
		/// Gets the left hand side of the equals part. Mainly exists for
		/// unit testing so data query generation logic can be validated
		/// </summary>
		public WherePart Left { get { return _left; } }
		/// <summary>
		/// Gets the right hand side of the equals part. Mainly exists for
		/// unit testing so data query generation logic can be validated
		/// </summary>
		public WherePart Right { get { return _right; } }
	}
}