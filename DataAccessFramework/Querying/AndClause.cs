namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Represents an AND clause in a query
	/// </summary>
	public class AndClause : OperatorClause
	{
		/// <summary>
		/// Creates a new <c>AndClause</c> instance
		/// </summary>
		public AndClause(params WherePart[] parts)
			: base("AND", parts)
		{
		}
	}
}