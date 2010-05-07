using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessFramework
{
	/// <summary>
	/// An implementation of the <see cref="DataTool"/> that uses
	/// Sql Server Express to connect to an .mdf file.
	/// </summary>
	public class SqlExpressDataTool : MSSqlDataTool
	{
		readonly IDbConnection _connection;
		IDbTransaction _transaction;

		/// <summary>
		/// Gets an open <see cref="SqlConnection"/>
		/// </summary>
		/// <returns></returns>
		protected override IDbConnection GetConnection()
		{
			return _connection;
		}

		/// <summary>
		/// Gets a <see cref="SqlTransaction"/> instance if
		/// a transaction has been started; otherwise <c>null</c>
		/// is returned.
		/// </summary>
		/// <returns></returns>
		protected override IDbTransaction GetTransaction()
		{
			return _transaction;
		}

		/// <summary>
		/// Creates a new <c>SqlExpressDataTool</c> instance.
		/// </summary>
		/// <param name="dataFile">
		/// The full path to the .mdf file containing the database.
		/// </param>
		public SqlExpressDataTool(string dataFile)
		{
			string connectionString = string.Format(
				@"Data Source=.\SQLEXPRESS;AttachDbFilename={0};Integrated Security=True;User Instance=True",
				dataFile);
			_connection = new SqlConnection(connectionString);
			_connection.Open();
			_transaction = null;
		}

		/// <summary>
		/// Disposes the current instance, closing the connection.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_transaction != null)
					_transaction.Dispose();
				_connection.Close();
			}
		}

		public override void BeginTransaction()
		{
			if (_transaction != null)
				throw new DataToolException("Cannot begin transaction, transaction already started");
			_transaction = GetConnection().BeginTransaction();
		}

		public override void CommitTransaction()
		{
			if (_transaction == null)
				throw new Exception("Cannot commit transaction, transaction was not started");
			var transaction = _transaction;
			_transaction = null;
			transaction.Commit();
		}

		public override void RollbackTransaction()
		{
			if (_transaction == null)
				throw new Exception("Cannot roll back transaction, transaction was not started");
			var t = _transaction;
			_transaction = null;
			t.Rollback();
		}
	}
}