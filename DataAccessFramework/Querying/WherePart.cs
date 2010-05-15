namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Represents a part in where part of an sql query.
	/// </summary>
	public abstract class WherePart
	{
		/// <summary>
		/// Gets whether or not the part is active, i.e. if it needs to
		/// go in the final query, or if it can be ignored. E.g. a fulltext
		/// search for *, or a like search for % can be ignored.
		/// An AND statement, where both parts are inactive, can be ignored.
		/// </summary>
		public virtual bool Active { get { return true; } }

		/// <summary>
		/// Builds the SQL for this part.
		/// </summary>
		internal abstract void BuildSql(BuildSqlContext sqlContext);
	}
}