using System;
using System.Data.SqlClient;
using System.Data;

namespace DataAccessFramework
{
	/// <summary>
	/// Specialization of <see cref="DataTool"/> that works with
	/// MS Sql Server
	/// </summary>
	public class SqlServerDataTool : MSSqlDataTool
	{
		private readonly string _connectionString;
		private SqlConnection _connection;
		private SqlTransaction _transaction;

		/// <summary>
		/// Creates a new <c>SqlServerDataTool</c> instance.
		/// </summary>
		/// <param name="connectionString"></param>
		public SqlServerDataTool(string connectionString)
		{
			_connectionString = connectionString;
		}

		/// <summary>
		/// Implements template function <see cref="DataTool.GetConnection"/>
		/// </summary>
		protected override IDbConnection GetConnection()
		{
			return Connection;
		}

		/// <summary>
		/// Gets the connection.
		/// </summary>
		public SqlConnection Connection
		{
			get
			{
				if (_connection == null)
				{
					var connection = new SqlConnection(_connectionString);
					connection.Open();
					_connection = connection;
				}
				return _connection;
			}
		}

		/// <summary>
		/// Implements <see cref="DataTool.GetTransaction"/> for Sql Server.
		/// </summary>
		protected override IDbTransaction GetTransaction()
		{
			return _transaction;
		}

		/// <summary>
		/// Disposes the object, closing transactions and connections. Only
		/// if disposing = true;
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				if (_transaction != null)
					_transaction.Dispose();
				if (_connection != null)
					_connection.Dispose();
			}
		}

		/// <summary>
		/// Starts an Sql Server transaction.
		/// </summary>
		public override void BeginTransaction()
		{
			if (_transaction != null)
				throw new InvalidOperationException("Error starting transaction. Transaction already started");
			_transaction = Connection.BeginTransaction();
		}

		/// <summary>
		/// Commits the SQL Server transaction.
		/// </summary>
		public override void CommitTransaction()
		{
			_transaction.Commit();
			_transaction.Dispose();
			_transaction = null;
		}

		/// <summary>
		/// Rolls back the SQL Server transaction
		/// </summary>
		public override void RollbackTransaction()
		{
			_transaction.Rollback();
			_transaction.Dispose();
			_connection.Dispose();
			_transaction = null;
			_connection = null;
		}
	}
}