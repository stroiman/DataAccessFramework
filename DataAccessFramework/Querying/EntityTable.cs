using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataAccessFramework.Querying
{
	public class EntityTable<T> : QueryTable
	{
		private List<FieldMapping<T>> _fields = new List<FieldMapping<T>>();

		public EntityTable(string tableName)
			: base(tableName)
		{ }

		protected FieldMapping<T> MapField(string fieldName, Func<T, int?> getValue)
		{
			var result = new FieldMapping<T>(this, fieldName, getValue);
			_fields.Add(result);
			return result;
		}

		protected FieldMapping<T> MapField(string fieldName, Func<T, DateTime?> getValue)
		{
			var result = new FieldMapping<T>(this, fieldName, getValue);
			_fields.Add(result);
			return result;
		}

		protected FieldMapping<T> MapField(string fieldName, Func<T, long?> getValue)
		{
			var result = new FieldMapping<T>(this, fieldName, getValue);
			_fields.Add(result);
			return result;
		}

		protected FieldMapping<T> MapField(string fieldName, Func<T, string> getValue)
		{
			var result = new FieldMapping<T>(this, fieldName, getValue);
			_fields.Add(result);
			return result;
		}

		public Query Insert(T entity)
		{
			return new InsertQuery(base.TableName, GetFields(entity));
		}

		private IEnumerable<Tuple<string, Func<DataTool, string, IDataParameter>>> GetFields(T entity)
		{
			return _fields.Select(x =>
				new Tuple<string, Func<DataTool, string, IDataParameter>>(
					x.FieldName,
					x.CreateParameter(entity))
				);
		}
	}
}