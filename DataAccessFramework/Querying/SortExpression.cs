namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Represents an expression for specifying sorting in the data query.
	/// </summary>
	public class SortExpression
	{
		private readonly FieldReference _field;

		/// <summary>
		/// Creates a new <see cref="SortExpression"/> instance.
		/// </summary>
		/// <param name="field">
		/// Value for the <see cref="SortExpression.Field"/> property.
		/// </param>
		public SortExpression(FieldReference field)
		{
			_field = field;
		}

		/// <summary>
		/// Gets a reference to the field that is the target of the sort.
		/// </summary>
		public FieldReference Field
		{
			get { return _field; }
		}
	}
}