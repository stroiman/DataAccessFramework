using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessFramework.Querying
{
	public class EntityTable<T> : QueryTable
	{
		private List<FieldMapping<T>> _fields = new List<FieldMapping<T>>();

		public EntityTable(string tableName)
			: base(tableName)
		{ }

		protected FieldMapping<T> MapField(string fieldName, Func<T, object> getValue)
		{
			var result = new FieldMapping<T>(this, fieldName, getValue);
			_fields.Add(result);
			return result;
		}

		public Query Insert(T entity)
		{
			return new InsertQuery(base.TableName, GetFields(entity));
		}

		private IEnumerable<Tuple<string, object>> GetFields(T entity)
		{
			return _fields.Select(x =>
				new Tuple<string, object>(
					x.FieldName,
					x.GetValue(entity))
				);
		}
	}
}