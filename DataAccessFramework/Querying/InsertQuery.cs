using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Serves as input to the <see cref="InsertQuery"/>. Each instance
	/// of this class tells the name of a field in a table, as well as
	/// a function that can generate an <see cref="IDataParameter"/>
	/// containing a value to insert into the field.
	/// </summary>
	public class InsertQueryParameter
	{
		private readonly string _fieldName;
		private readonly Func<DataTool, string, IDataParameter> _createParameterFunction;
		public InsertQueryParameter(string fieldName, Func<DataTool, string, IDataParameter> createParameterFunction)
		{
			_fieldName = fieldName;
			_createParameterFunction = createParameterFunction;
		}

		/// <summary>
		/// Creates an <see cref="IDataParameter"/> containing the value to insert
		/// into a field
		/// </summary>
		/// <param name="dataTool">
		/// A <see cref="DataTool"/> instance used to create the value
		/// </param>
		/// <param name="parameterName">
		/// The name of the parameter to create
		/// </param>
		/// <returns>
		/// The actual parameter
		/// </returns>
		public IDataParameter CreateParameter(DataTool dataTool, string parameterName)
		{
			return _createParameterFunction(dataTool, parameterName);
		}

		/// <summary>
		/// Gets the name of the field in the database that should be updated
		/// </summary>
		public string FieldName
		{
			get { return _fieldName; }
		}
	}

	/// <summary>
	/// A specialized <see cref="Query"/> that represents an insert sql query.
	/// </summary>
	public class InsertQuery : Query
	{
		private readonly string _tableName;
		private readonly List<InsertQueryParameter> _fields;

		public InsertQuery(string tableName, IEnumerable<InsertQueryParameter> fields)
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
				builder.Append("[");
				builder.Append(tuple.FieldName);
				builder.Append("]");
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
				parameters.Add(tuple.CreateParameter(dataTool, parameterName));
			}
			builder.Append(")");
			return new ParseResult(builder.ToString(), parameters);
		}
	}
}