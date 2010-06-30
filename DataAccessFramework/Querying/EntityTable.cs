using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataAccessFramework.Querying
{
	/// <summary>
	/// A specialized <see cref="QueryTable"/> that can be used for generating insert statements
	/// </summary>
	/// <typeparam name="T">
	/// A data type from where all the data to insert into the table can be retrieved.
	/// </typeparam>
	public class EntityTable<T> : QueryTable
	{
		private readonly List<FieldMapping<T>> _fields;

		/// <summary>
		/// Creates a new <see cref="EntityTable{T}"/> instance
		/// </summary>
		/// <param name="tableName">
		/// The name of the table in the data store. This will be used for generating
		/// insert and select sql statements
		/// </param>
		public EntityTable(string tableName)
			: base(tableName)
		{
			_fields = new List<FieldMapping<T>>();
		}

		private FieldMapping<T> AddField(FieldMapping<T> field)
		{
			_fields.Add(field);
			return field;
		}

		/// <summary>
		/// Maps an integer field to to a lookup function
		/// </summary>
		/// <param name="fieldName">
		/// The name of the field in the database
		/// </param>
		/// <param name="getValue">
		/// A function that can return the value to insert into the field
		/// </param>
		/// <returns>
		/// A <see cref="FieldMapping{T}"/> instance that can be used to generate select statements
		/// </returns>
		protected FieldMapping<T> MapField(string fieldName, Func<T, int?> getValue)
		{
			return AddField(new FieldMapping<T>(this, fieldName, getValue));
		}

		/// <summary>
		/// Maps an date/time field to to a lookup function
		/// </summary>
		/// <param name="fieldName">
		/// The name of the field in the database
		/// </param>
		/// <param name="getValue">
		/// A function that can return the value to insert into the field
		/// </param>
		/// <returns>
		/// A <see cref="FieldMapping{T}"/> instance that can be used to generate select statements
		/// </returns>
		protected FieldMapping<T> MapField(string fieldName, Func<T, DateTime?> getValue)
		{
			return AddField(new FieldMapping<T>(this, fieldName, getValue));
		}

		/// <summary>
		/// Maps a 64 bit field to to a lookup function
		/// </summary>
		/// <param name="fieldName">
		/// The name of the field in the database
		/// </param>
		/// <param name="getValue">
		/// A function that can return the value to insert into the field
		/// </param>
		/// <returns>
		/// A <see cref="FieldMapping{T}"/> instance that can be used to generate select statements
		/// </returns>
		protected FieldMapping<T> MapField(string fieldName, Func<T, long?> getValue)
		{
			return AddField(new FieldMapping<T>(this, fieldName, getValue));
		}

		/// <summary>
		/// Maps an string field to to a lookup function
		/// </summary>
		/// <param name="fieldName">
		/// The name of the field in the database
		/// </param>
		/// <param name="getValue">
		/// A function that can return the value to insert into the field
		/// </param>
		/// <returns>
		/// A <see cref="FieldMapping{T}"/> instance that can be used to generate select statements
		/// </returns>
		protected FieldMapping<T> MapField(string fieldName, Func<T, string> getValue)
		{
			return AddField(new FieldMapping<T>(this, fieldName, getValue));
		}

		/// <summary>
		/// Maps an decimal field to to a lookup function
		/// </summary>
		/// <param name="fieldName">
		/// The name of the field in the database
		/// </param>
		/// <param name="getValue">
		/// A function that can return the value to insert into the field
		/// </param>
		/// <returns>
		/// A <see cref="FieldMapping{T}"/> instance that can be used to generate select statements
		/// </returns>
		protected FieldMapping<T> MapField(string fieldName, Func<T, decimal?> getValue)
		{
			return AddField(new FieldMapping<T>(this, fieldName, getValue));
		}

		/// <summary>
		/// Generates an insert query that can be used to insert data into the
		/// database.
		/// </summary>
		/// <param name="entity">
		/// An object that will be passed to the lookup functions in order to retrieve
		/// the actual value.
		/// </param>
		/// <returns>
		/// A <see cref="Query"/> object that can be executed by the <see cref="DataTool"/>
		/// </returns>
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