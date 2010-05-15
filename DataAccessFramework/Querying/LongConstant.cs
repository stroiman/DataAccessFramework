namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Where part containing an 64bit integer constant.
	/// </summary>
	public class LongConstant : WherePart
	{
		private readonly long _value;

		/// <summary>
		/// Creates a new <see cref="LongConstant"/> instance
		/// </summary>
		/// <param name="value"></param>
		public LongConstant(long value)
		{
			_value = value;
		}

		/// <summary>
		/// Builds the SQL for the 64bit integer constants. Creates a parameter
		/// </summary>
		internal override void BuildSql(BuildSqlContext context)
		{
			int parameterNo = context.Parameters.Count + 1;
			string parameterName = "p" + parameterNo;
			context.Parameters.Add(context.DataTool.CreateLongParameter(parameterName, _value));
			context.Builder.Append("@");
			context.Builder.Append(parameterName);
		}
	}
}