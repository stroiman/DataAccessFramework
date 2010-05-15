namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Where part containing an boolean constant.
	/// </summary>
	public class BoolConstant : WherePart
	{
		private readonly bool _value;

		/// <summary>
		/// Creates a new <c>IntConstant</c> value
		/// </summary>
		/// <param name="value"></param>
		public BoolConstant(bool value)
		{
			_value = value;
		}

		/// <summary>
		/// Builds the SQL for the integer constants. Creates a parameter
		/// </summary>
		internal override void BuildSql(BuildSqlContext context)
		{
			int parameterNo = context.Parameters.Count + 1;
			string parameterName = "p" + parameterNo;
			context.Parameters.Add(context.DataTool.CreateBoolParameter(parameterName, _value));
			context.Builder.Append("@");
			context.Builder.Append(parameterName);
		}
	}
}