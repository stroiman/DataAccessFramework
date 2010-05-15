namespace DataAccessFramework.Querying
{
	/// <summary>
	/// A where part that represents a string constant. Will be converted to a parameter.
	/// </summary>
	public class StringConstant : WherePart
	{
		private readonly string _value;

		/// <summary>
		/// Creates a new <c>StringConstant</c> instance
		/// </summary>
		public StringConstant(string value)
		{
			_value = value;
		}

		/// <summary>
		/// Builds the SQL representing the string constant as a parameter.
		/// </summary>
		internal override void BuildSql(BuildSqlContext context)
		{
			int parameterNo = context.Parameters.Count + 1;
			string parameterName = "p" + parameterNo;
			context.Parameters.Add(context.DataTool.CreateStringParameter(parameterName, _value, null));
			context.Builder.Append("@");
			context.Builder.Append(parameterName);
		}
	}
}