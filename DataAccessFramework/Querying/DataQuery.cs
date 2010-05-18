using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;

namespace DataAccessFramework.Querying
{
	///<summary>
	/// An object that represents an SQL query, containing references
	/// to the tables to join, and the where clauses.
	///</summary>
	public class DataQuery
	{
		private readonly List<TableBase> _tables = new List<TableBase>();
		private readonly AndClause _whereClause = new AndClause();
		private readonly List<SortExpression> _sortExpressions =
			new List<SortExpression>();
		private int _tableNo = 1;
		private readonly Dictionary<QueryTable, string> _aliasMap = new Dictionary<QueryTable, string>();

		/// <summary>
		/// Adds a table to the query.
		/// </summary>
		public void AddTable(TableBase table)
		{
			_tables.Add(table);
			SetAlias(table);
		}

		private string GetAlias(QueryTable table)
		{
			return _aliasMap[table];
		}

		public void SetAlias(TableBase tableBase)
		{
			var join = tableBase as Join;
			if (join != null)
			{
				SetAlias(join.Left);
				SetAlias(join.Right);
				return;
			}
			var table = (QueryTable)tableBase;
			_aliasMap.Add(table, "t" + _tableNo++);
		}

		/// <summary>
		/// Adds an expression to the data query describing how to sort the data.
		/// </summary>
		public void AddSortExpression(SortExpression sortExpression)
		{
			_sortExpressions.Add(sortExpression);
		}

		/// <summary>
		/// Gets all the tables in the data query.
		/// </summary>
		public ReadOnlyCollection<TableBase> Tables
		{
			get { return _tables.AsReadOnly(); }
		}

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

		internal ParseResult Parse(DataTool dataTool)
		{
			IList<IDataParameter> parameters = new List<IDataParameter>();
			var builder = new StringBuilder();
			builder.Append("select * from ");
			var secondTable = false;
			var buildSqlContext = new BuildSqlContext(builder, dataTool, parameters, GetAlias);
			foreach (var table in _tables)
			{
				if (secondTable)
					builder.Append(", ");
				table.BuildSql(buildSqlContext);
				secondTable = true;
			}
			if (_whereClause.Active)
			{
				builder.Append(" where ");
				_whereClause.BuildSql(buildSqlContext);
			}
			if (_sortExpressions.Count > 0)
			{
				builder.Append(" order by ");
				var first = true;
				foreach (var sortExpression in _sortExpressions)
				{
					if (!first)
						builder.Append(", ");
					first = false;
					sortExpression.Field.BuildSql(buildSqlContext);
				}
			}
			return new ParseResult(builder.ToString(), parameters);
		}

		/// <summary>
		/// Adds a where clause to the query.
		/// </summary>
		/// <param name="clause"></param>
		/// <remarks>
		/// All where clases added using this function must be true,
		/// i.e. there is an AND operator between them.
		/// </remarks>
		public void AddWhere(WherePart clause)
		{
			_whereClause.AddPart(clause);
		}

		/// <summary>
		/// Finds the <see cref="QueryTable"/> instance previously
		/// added with a specific name.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public TableBase FindTable(string tableName)
		{
			return _tables.Find(x => x.Name == tableName);
		}

		/// <summary>
		/// Gets the where parts in this data query.
		/// </summary>
		public AndClause WhereClause { get { return _whereClause; } }
	}
}