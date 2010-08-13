using System;

namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Where clause for a string field that starts with a specific string
	/// </summary>
	public class StartsWithClause : WherePart
	{
		private readonly WherePart _left;
		private readonly string _name;

		public StartsWithClause(WherePart left, string name)
		{
			_left = left;
			_name = name;
		}

		internal override void BuildSql(BuildSqlContext sqlContext)
		{
			_left.BuildSql(sqlContext);
			sqlContext.Builder.Append(" LIKE ");

			int parameterNo = sqlContext.Parameters.Count + 1;
			string parameterName = "p" + parameterNo;
			sqlContext.Parameters.Add(sqlContext.DataTool.CreateStringParameter(parameterName, _name + "%", null));
			sqlContext.Builder.Append("@");
			sqlContext.Builder.Append(parameterName);
		}
	}
}