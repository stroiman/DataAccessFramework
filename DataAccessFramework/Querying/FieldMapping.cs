using System;
using System.Data;

namespace DataAccessFramework.Querying
{
	public class FieldMapping<T> : FieldReference
	{
		private readonly Func<DataTool, string, T, IDataParameter> _createParameter;

		public FieldMapping(
			QueryTable table, string fieldName, Func<T, int?> getValue)
			: base(table, fieldName)
		{
			_createParameter = (tool, name, entity) => tool.CreateIntParameter(name, getValue(entity));
		}

		public FieldMapping(
			QueryTable table, string fieldName, Func<T, DateTime?> getValue)
			: base(table, fieldName)
		{
			_createParameter = (tool, name, entity) => tool.CreateDateTimeParameter(name, getValue(entity));
		}

		public FieldMapping(
			QueryTable table, string fieldName, Func<T, long?> getValue)
			: base(table, fieldName)
		{
			_createParameter =
				(tool, name, entity) => tool.CreateLongParameter(name, getValue(entity));
		}

		public FieldMapping(
			QueryTable table, string fieldName, Func<T, string> getValue)
			: base(table, fieldName)
		{
			_createParameter = (tool, name, entity) => tool.CreateStringParameter(name, getValue(entity), null);
		}

		public FieldMapping(
			QueryTable table, string fieldName, Func<T, decimal?> getValue)
			: base(table, fieldName)
		{
			_createParameter = (tool, name, entity) => tool.CreateDecimalParameter(name, getValue(entity));
		}

		public FieldMapping(
			QueryTable table, string fieldName, Func<T, bool> getValue)
			: base(table, fieldName)
		{
			_createParameter = (tool, name, entity) => tool.CreateBoolParameter(name, getValue(entity));
		}

		public Func<DataTool, string, IDataParameter> CreateParameter(T entity)
		{
			return (DataTool tool, string fieldName) =>
				_createParameter(tool, fieldName, entity);
		}
	}
}