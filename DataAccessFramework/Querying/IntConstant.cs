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
		internal override void BuildSql(BuildSqlContext sqlContext)
		{
			int parameterNo = sqlContext.Parameters.Count + 1;
			string parameterName = "p" + parameterNo;
			sqlContext.Parameters.Add(sqlContext.DataTool.CreateIntParameter(parameterName, _value));
			sqlContext.Builder.Append("@");
			sqlContext.Builder.Append(parameterName);
		}

		/// <summary>
		/// Gets the value of the constant. Mainly exists for
		/// unit testing so data query generation logic can be validated
		/// </summary>
		public int Value { get { return _value; } }
	}
}