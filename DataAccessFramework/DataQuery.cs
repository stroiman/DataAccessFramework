using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Linq;

namespace DataAccessFramework
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

		/// <summary>
		/// Adds a table to the query.
		/// </summary>
		public void AddTable(QueryTable table)
		{
			_tables.Add(table);
			SetAlias(table);
		}

		/// <summary>
		/// Adds a table to the query.
		/// </summary>
		public void AddTable(Join join)
		{
			_tables.Add(join);
			SetAlias(join);
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
			table.SetAlias("t" + _tableNo++);
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
			foreach (var table in _tables)
			{
				if (secondTable)
					builder.Append(", ");
				table.BuildSql(builder, dataTool, parameters);
				secondTable = true;
			}
			if (_whereClause.Active)
			{
				builder.Append(" where ");
				_whereClause.BuildSql(builder, dataTool, parameters);
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
					sortExpression.Field.BuildSql(builder, dataTool, parameters);
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

	public abstract class TableBase
	{
		public abstract string Name { get; }
		abstract internal void BuildSql(StringBuilder builder, DataTool dataTool, IList<IDataParameter> parameters);

		public Join LeftJoin(QueryTable table)
		{
			return new Join(this, table);
		}
	}

	/// <summary>
	/// Represents a table used in a query.
	/// </summary>
	public class QueryTable : TableBase
	{
		private readonly string _tableName;
		private string _alias;

		/// <summary>
		/// Creates a new <c>QueryTable</c> instance.
		/// </summary>
		/// <param name="tableName"></param>
		public QueryTable(string tableName)
		{
			_tableName = tableName;
		}

		/// <summary>
		/// Gets the table name
		/// </summary>
		public override string Name { get { return _tableName; } }

		/// <summary>
		/// Gets the alias used for the table in the query.
		/// </summary>
		public string Alias
		{
			get { return _alias; }
		}

		internal void SetAlias(string alias)
		{
			_alias = alias;
		}

		internal override void BuildSql(StringBuilder builder, DataTool dataTool, IList<IDataParameter> parameters)
		{
			builder.Append("[");
			builder.Append(_tableName);
			builder.Append("] ");
			builder.Append(_alias);
		}

		/// <summary>
		/// Creates a string representation of data table.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _tableName + " " + _alias;
		}

		public FieldReference Field(string name)
		{
			return new FieldReference(this, name);
		}
	}

	public class Join : TableBase
	{
		private readonly TableBase _left;
		private readonly QueryTable _right;
		private WherePart _wherePart;

		public Join(TableBase left, QueryTable right)
		{
			_left = left;
			_right = right;
		}

		public QueryTable Right
		{
			get { return _right; }
		}

		public TableBase Left
		{
			get { return _left; }
		}

		public Join On(WherePart clause)
		{
			_wherePart = clause;
			return this;
		}

		public override string Name
		{
			get { return "JOIN"; }
		}

		internal override void BuildSql(StringBuilder builder, DataTool dataTool, IList<IDataParameter> parameters)
		{
			Left.BuildSql(builder, dataTool, parameters);
			builder.Append(" left outer join ");
			Right.BuildSql(builder, dataTool, parameters);
			if (_wherePart == null)
				return;
			builder.Append(" on ");
			_wherePart.BuildSql(builder, dataTool, parameters);
		}
	}

		/// <summary>
	/// Represents a part in where part of an sql query.
	/// </summary>
	public abstract class WherePart
	{
		/// <summary>
		/// Gets whether or not the part is active, i.e. if it needs to
		/// go in the final query, or if it can be ignored. E.g. a fulltext
		/// search for *, or a like search for % can be ignored.
		/// An AND statement, where both parts are inactive, can be ignored.
		/// </summary>
		public virtual bool Active { get { return true; } }

		/// <summary>
		/// Builds the SQL for this part.
		/// </summary>
		/// <param name="builder">
		/// Builder, where the SQL will be output to.
		/// </param>
		/// <param name="tool">
		/// A data tool used for generating parameters
		/// </param>
		/// <param name="parameters">
		/// The current list of parameters, where new parameters will be added
		/// </param>
		public abstract void BuildSql(StringBuilder builder, DataTool tool, IList<IDataParameter> parameters);
	}

	/// <summary>
	/// A where clause for a LIKE statement
	/// </summary>
	public class LikeClause : WherePart
	{
		private WherePart _left;
		private StringConstant _right;

		/// <summary>
		/// Creates a new <c>LikeClause</c> instance
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public LikeClause(WherePart left, string right)
		{
			_left = left;
			_right = new StringConstant(right + "%");
		}

		/// <summary>
		/// Builds the sql for the clause.
		/// </summary>
		public override void BuildSql(StringBuilder builder, DataTool tool, IList<IDataParameter> parameters)
		{
			_left.BuildSql(builder, tool, parameters);
			builder.Append(" LIKE ");
			_right.BuildSql(builder, tool, parameters);
		}
	}

	/// <summary>
	/// Where clause for an equals operator.
	/// </summary>
	public class EqualsClause : WherePart
	{
		private WherePart _left;
		private WherePart _right;

		/// <summary>
		/// Creates a new <c>EqualsClause</c> instance
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public EqualsClause(WherePart left, WherePart right)
		{
			_left = left;
			_right = right;
		}

		/// <summary>
		/// Builds the SQL for this part.
		/// </summary>
		public override void BuildSql(StringBuilder builder, DataTool tool, IList<IDataParameter> parameters)
		{
			_left.BuildSql(builder, tool, parameters);
			builder.Append("=");
			_right.BuildSql(builder, tool, parameters);
		}

		/// <summary>
		/// Gets the left hand side of the equals part. Mainly exists for
		/// unit testing so data query generation logic can be validated
		/// </summary>
		public WherePart Left { get { return _left; } }
		/// <summary>
		/// Gets the right hand side of the equals part. Mainly exists for
		/// unit testing so data query generation logic can be validated
		/// </summary>
		public WherePart Right { get { return _right; } }
	}

	/// <summary>
	/// A where part representing a full text search.
	/// </summary>
	public class FullTextClause : WherePart
	{
		private string _searchText;
		private List<FieldReference> _fieldReferences;

		/// <summary>
		/// Creates a new <c>FullTextClause</c> instance.
		/// </summary>
		/// <param name="searchText">
		/// The text to search for
		/// </param>
		/// <param name="fieldReferences">
		/// A reference to the fields in which to search.
		/// </param>
		public FullTextClause(string searchText, params FieldReference[] fieldReferences)
		{
			_searchText = searchText;
			_fieldReferences = new List<FieldReference>(fieldReferences);
		}

		/// <summary>
		/// Gets whether or not the fulltext search is active, i.e. does
		/// it provide any filtering. If the search text is only wildcard,
		/// then <c>false</c> is returned
		/// </summary>
		public override bool Active
		{
			get
			{
				if (string.IsNullOrEmpty(_searchText))
					return false;
				if (_searchText == "*")
					return false;
				if (_searchText == "\"*\"")
					return false;
				return true;
			}
		}

		/// <summary>
		/// Builds the SQL for the full text.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="tool"></param>
		/// <param name="parameters"></param>
		public override void BuildSql(StringBuilder builder, DataTool tool, IList<IDataParameter> parameters)
		{
			builder.Append("contains(");
			foreach (FieldReference reference in _fieldReferences)
			{
				reference.BuildSql(builder, tool, parameters);
				builder.Append(",");
			}
			new StringConstant(_searchText).BuildSql(builder, tool, parameters);
			builder.Append(")");
		}
	}

	/// <summary>
	/// A part in a where clause that references a field in one
	/// of the included tables.
	/// </summary>
	public class FieldReference : WherePart
	{
		private readonly QueryTable _table;
		private readonly string _fieldName;

		/// <summary>
		/// Creates a new <c>FieldReference</c> instance.
		/// </summary>
		/// <param name="table">The table that we are referencing</param>
		/// <param name="fieldName">The name of the field</param>
		public FieldReference(QueryTable table, string fieldName)
		{
			_table = table;
			_fieldName = fieldName;
		}

		/// <summary>
		/// Builds the SQL for this field reference.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="tool"></param>
		/// <param name="parameters"></param>
		public override void BuildSql(StringBuilder builder, DataTool tool, IList<IDataParameter> parameters)
		{
			builder.Append(_table.Alias);
			builder.Append(".[");
			builder.Append(_fieldName);
			builder.Append("]");
		}

		/// <summary>
		/// Gets the referenced table. Mainly exists for
		/// unit testing so data query generation logic can be validated
		/// </summary>
		public QueryTable Table { get { return _table; } }

		/// <summary>
		/// Gets the name of the referenced field. Mainly exists for
		/// unit testing so data query generation logic can be validated
		/// </summary>
		public string FieldName { get { return _fieldName; } }

		/// <summary>
		/// Generates an <see cref="EqualsClause"/> for this field
		/// </summary>
		public EqualsClause EqualTo(int value)
		{
			return new EqualsClause(this, new IntConstant(value));
		}

		/// <summary>
		/// Generates an <see cref="EqualsClause"/> for this field
		/// </summary>
		public EqualsClause EqualTo(long value)
		{
			return new EqualsClause(this, new LongConstant(value));
		}

		public EqualsClause EqualTo(FieldReference field)
		{
			return new EqualsClause(this, field);
		}
	}

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
		public override void BuildSql(StringBuilder builder, DataTool tool, IList<IDataParameter> parameters)
		{
			int parameterNo = parameters.Count + 1;
			string parameterName = "p" + parameterNo;
			parameters.Add(tool.CreateIntParameter(parameterName, _value));
			builder.Append("@");
			builder.Append(parameterName);
		}

		/// <summary>
		/// Gets the value of the constant. Mainly exists for
		/// unit testing so data query generation logic can be validated
		/// </summary>
		public int Value { get { return _value; } }
	}

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
		public override void BuildSql(StringBuilder builder, DataTool tool, IList<IDataParameter> parameters)
		{
			int parameterNo = parameters.Count + 1;
			string parameterName = "p" + parameterNo;
			parameters.Add(tool.CreateLongParameter(parameterName, _value));
			builder.Append("@");
			builder.Append(parameterName);
		}
	}

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
		/// <param name="builder"></param>
		/// <param name="tool"></param>
		/// <param name="parameters"></param>
		public override void BuildSql(StringBuilder builder, DataTool tool, IList<IDataParameter> parameters)
		{
			int parameterNo = parameters.Count + 1;
			string parameterName = "p" + parameterNo;
			parameters.Add(tool.CreateBoolParameter(parameterName, _value));
			builder.Append("@");
			builder.Append(parameterName);
		}
	}

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
		/// <param name="builder"></param>
		/// <param name="tool"></param>
		/// <param name="parameters"></param>
		public override void BuildSql(StringBuilder builder, DataTool tool, IList<IDataParameter> parameters)
		{
			int parameterNo = parameters.Count + 1;
			string parameterName = "p" + parameterNo;
			parameters.Add(tool.CreateStringParameter(parameterName, _value, null));
			builder.Append("@");
			builder.Append(parameterName);
		}
	}

	/// <summary>
	/// A simple case class for creating AND and OR operators.
	/// </summary>
	public class OperatorClause : WherePart
	{
		private readonly string _operatorName;
		private readonly List<WherePart> _parts;

		/// <summary>
		/// Gets all the active parts in the clause. <see cref="WherePart.Active"/>
		/// </summary>
		private IEnumerable<WherePart> ActiveParts
		{
			get
			{
				return _parts.Where(x => x.Active);
			}
		}

		/// <summary>
		/// Gets whether or not this part is active.
		/// </summary>
		public override bool Active
		{
			get
			{
				return _parts.Exists(x => x.Active);
			}
		}

		/// <summary>
		/// Creates a new <c>OperatorClause</c> instance
		/// </summary>
		public OperatorClause(string operatorName, params WherePart[] parts)
		{
			_operatorName = operatorName;
			_parts = new List<WherePart>();
			foreach (var part in parts)
				AddPart(part);
		}

		/// <summary>
		/// Builds the SQL for the operator.
		/// </summary>
		public override void BuildSql(StringBuilder builder, DataTool tool, IList<IDataParameter> parameters)
		{
			var activeParts = new List<WherePart>(ActiveParts);
			if (activeParts.Count == 0)
				return;
			if (activeParts.Count == 1)
			{
				activeParts[0].BuildSql(builder, tool, parameters);
				return;
			}
			bool afterFirst = false;
			builder.Append("(");
			foreach (var part in activeParts)
			{
				if (afterFirst)
				{
					builder.Append(" ");
					builder.Append(_operatorName);
					builder.Append(" ");
				}
				part.BuildSql(builder, tool, parameters);
				afterFirst = true;
			}
			builder.Append(")");
		}

		/// <summary>
		/// Adds a new part to the clause.
		/// </summary>
		/// <param name="clause"></param>
		public void AddPart(WherePart clause)
		{
			if (clause != null)
				_parts.Add(clause);
		}

		/// <summary>
		/// Gets the number of parts in the clause.
		/// </summary>
		public int Count { get { return _parts.Count; } }

		/// <summary>
		/// Gets whether or not the clause is empty, i.e. if there
		/// are any parts in it.
		/// </summary>
		public bool Empty { get { return _parts.Count == 0; } }

		/// <summary>
		/// Gets the parts in the clause. Mainly exists for unit testing, 
		/// to validate that query parsing builds up the correct data query.
		/// </summary>
		public ReadOnlyCollection<WherePart> Parts { get { return _parts.AsReadOnly(); } }
	}

	/// <summary>
	/// Represents an AND clause in a query
	/// </summary>
	public class AndClause : OperatorClause
	{
		/// <summary>
		/// Creates a new <c>AndClause</c> instance
		/// </summary>
		public AndClause(params WherePart[] parts)
			: base("AND", parts)
		{
		}
	}

	/// <summary>
	/// Represents an OR clause in a query.
	/// </summary>
	public class OrClause : OperatorClause
	{
		/// <summary>
		/// Creates a new <c>OrClause</c>
		/// </summary>
		/// <param name="parts"></param>
		public OrClause(params WherePart[] parts)
			: base("OR", parts)
		{
		}
	}

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

	/// <summary>
	/// A where clause for specifying that a column should be null.
	/// </summary>
	public class IsNullClause : WherePart
	{
		private readonly FieldReference _field;

		/// <summary>
		/// Creates a new <see cref="IsNullClause"/> instance.
		/// </summary>
		/// <param name="field">A reference to the field that should be null.</param>
		public IsNullClause(FieldReference field)
		{
			_field = field;
		}

		/// <summary>
		/// Implements generating the sql for the clause
		/// </summary>
		public override void BuildSql(StringBuilder builder, DataTool tool, IList<IDataParameter> parameters)
		{
			_field.BuildSql(builder, tool, parameters);
			builder.Append(" IS NULL");
		}
	}
}