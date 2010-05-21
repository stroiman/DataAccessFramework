using System.Collections.Generic;
using System.Data;

namespace DataAccessFramework.Querying
{
	/// <summary>
	/// An object that represents a general SQL query, can be either a select,
	/// insert, or an update query.
	/// </summary>
	public abstract class Query
	{
		internal abstract ParseResult Parse(DataTool dataTool);

		internal class ParseResult
		{
			private readonly string _sql;
			private readonly IList<IDataParameter> _parameters;

			public ParseResult(string sql, IList<IDataParameter> parameters)
			{
				_sql = sql;
				_parameters = parameters;
			}

			public string Sql { get { return _sql; } }
			public IList<IDataParameter> Parameters { get { return _parameters; } }
		}
	}
}