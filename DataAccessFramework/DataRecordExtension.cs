using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccessFramework
{
	public static class DataRecordExtension
	{
		public static object GetObject(this IDataRecord record, string fieldName)
		{
			var result = record[fieldName];
			return result == DBNull.Value ? null : result;
		}

		public static string GetString(this IDataRecord record, string fieldName)
		{
			return (string)GetObject(record, fieldName);
		}

		public static long GetLong(this IDataRecord record, string fieldName)
		{
			var result = record[fieldName];
			if (result == DBNull.Value)
				throw new InvalidOperationException(
					string.Format(
						"Cannot get long parameter for field, {0}. Field returned NULL which was not allowed",
						fieldName));
			return (long)result;
		}

		public static T Get<T>(this IDataRecord record, string fieldName)
		{
			var result = record[fieldName];
			if (result == DBNull.Value)
				throw new InvalidOperationException(
					string.Format(
						"Cannot get parameter for field, {0}. Field returned NULL which was not allowed",
						fieldName));
			return (T)result;
		}

		public static T? GetNullable<T>(this IDataRecord record, string fieldName)
			where T : struct
		{
			var result = record[fieldName];
			if (result == DBNull.Value)
				return null;
			return (T)result;
		}
	}
}