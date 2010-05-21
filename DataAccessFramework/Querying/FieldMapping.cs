using System;

namespace DataAccessFramework.Querying
{
	public class FieldMapping<T> : FieldReference
	{
		private readonly Func<T, object> _getValue;

		public FieldMapping(
			QueryTable table, string fieldName, Func<T, object> getValue)
			: base(table, fieldName)
		{
			_getValue = getValue;
		}

		public Func<T, object> GetValue
		{
			get { return _getValue; }
		}
	}
}