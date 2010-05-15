namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Represents an OR clause in a query.
	/// </summary>
	public class OrClause : OperatorClause
	{
		/// <summary>
		/// Creates a new <c>OrClause</c>
		/// </summary>
		/// <param name="parts"></param>
		public OrClause(params WherePart[] parts)
			: base("OR", parts)
		{
		}
	}
}