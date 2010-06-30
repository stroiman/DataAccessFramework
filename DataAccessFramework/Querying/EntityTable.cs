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

		protected FieldMapping<T> AddField(FieldMapping<T> field)
		{
			_fields.Add(field);
			return field;
		}

		protected FieldMapping<T> MapField(string fieldName, Func<T, int?> getValue)
		{
			return AddField(new FieldMapping<T>(this, fieldName, getValue));
		}

		protected FieldMapping<T> MapField(string fieldName, Func<T, DateTime?> getValue)
		{
			return AddField(new FieldMapping<T>(this, fieldName, getValue));
		}

		protected FieldMapping<T> MapField(string fieldName, Func<T, long?> getValue)
		{
			return AddField(new FieldMapping<T>(this, fieldName, getValue));
		}

		protected FieldMapping<T> MapField(string fieldName, Func<T, string> getValue)
		{
			return AddField(new FieldMapping<T>(this, fieldName, getValue));
		}

		public Query Insert(T entity)
		{
			return new InsertQuery(base.TableName, GetFields(entity));
		}

		private IEnumerable<InsertQueryParameter> GetFields(T entity)
		{
			return _fields.Select(x =>
				new InsertQueryParameter(
					x.FieldName,
					x.CreateParameter(entity))
				);
		}
	}
}