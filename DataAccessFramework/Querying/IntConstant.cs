namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Where part containing an integer constant.
	/// </summary>
	public class IntConstant : WherePart
	{
		private readonly int _value;

		/// <summary>
		/// Creates a new <c>IntConstant</c> value
		/// </summary>
		/// <param name="value"></param>
		public IntConstant(int value)
		{
			_value = value;
		}

		/// <summary>
		/// Builds the SQL for the integer constants. Creates a parameter
		/// </summary>
		internal override void BuildSql(BuildSqlContext context)
		{
            var parameterName = context.CreateNextParameterName();
			context.Parameters.Add(context.DataTool.CreateIntParameter(parameterName, _value));
			context.Builder.Append("@");
			context.Builder.Append(parameterName);
		}

		/// <summary>
		/// Gets the value of the constant. Mainly exists for
		/// unit testing so data query generation logic can be validated
		/// </summary>
		public int Value { get { return _value; } }
	}
}