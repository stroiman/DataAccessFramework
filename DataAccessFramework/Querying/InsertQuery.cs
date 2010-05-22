using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccessFramework.Querying
{
	/// <summary>
	/// A specialized <see cref="Query"/> that represents an insert sql query.
	/// </summary>
	public class InsertQuery : Query
	{
		private readonly string _tableName;
		private readonly List<Tuple<string, Func<DataTool, string, IDataParameter>>> _fields;

		public InsertQuery(string tableName, IEnumerable<Tuple<string, Func<DataTool, string, IDataParameter>>> fields)
		{
			_tableName = tableName;
			_fields = fields.ToList();
		}

		internal override ParseResult Parse(DataTool dataTool)
		{
			var parameters = new List<IDataParameter>();
			var builder = new StringBuilder();
			builder.Append("insert into [");
			builder.Append(_tableName);
			builder.Append("] (");
			var appendComma = false;
			foreach (var tuple in _fields)
			{
				if (appendComma)
					builder.Append(", ");
				appendComma = true;
				builder.Append(tuple.Item1);
			}
			builder.Append(") values (");
			int parameterNo = 0;
			foreach (var tuple in _fields)
			{
				if (parameterNo > 0)
					builder.Append(", ");
				parameterNo++;
				var parameterName = "p" + parameterNo;
				builder.Append("@");
				builder.Append(parameterName);
				parameters.Add(tuple.Item2(dataTool, parameterName));
			}
			builder.Append(")");
			return new ParseResult(builder.ToString(), parameters);
		}
	}
}