using System;
using System.Linq;
using System.Data;

// TODO: Handle SqlException error code -2146232060 (disconnected)

namespace DataAccessFramework
{
	/// <summary>
	/// An abstract class providing helper functionality to connect to
	/// a database.
	/// </summary>
	/// <remarks>
	/// <p>
	/// This class provides functions that can simplfy common database
	/// access functionality, e.g. executing a stored procedure.
	/// </p>
	/// <p>
	/// The tool itself is not tied to any specific database provider.
	/// Using this tool to execute database operations separates the application
	/// from the actual database server software.
	/// </p>
	/// <p>
	/// The client code should also use functions in this class to create 
	/// parameters to pass to the stored procedure. That will ensure that
	/// the parameters are compatible with the database provider.
	/// </p>
	/// </remarks>
	public abstract class DataTool : IDisposable
	{
		bool _isDisposed;

		/// <summary>
		/// Template function for getting a connection object.
		/// </summary>
		/// <remarks>
		/// The <see cref="DataTool"/> class will call this internally
		/// when executing a query
		/// </remarks>
		/// <returns>
		/// A <see cref="IDbConnection"/> instance in an open state.
		/// </returns>
		protected abstract IDbConnection GetConnection();

		/// <summary>
		/// Template function for getting a transaction
		/// </summary>
		/// <remarks>
		/// The <see cref="DataTool"/> class will call this internally
		/// when executing a query
		/// </remarks>
		/// <returns>
		/// A <see cref="IDbTransaction"/> instance if a transaction
		/// has been started; otherwise <c>null</c>
		/// </returns>
		protected abstract IDbTransaction GetTransaction();

		/// <summary>
		/// Template function for creating a command object.
		/// </summary>
		/// <remarks>
		/// The <see cref="DataTool"/> class will call this internally
		/// when executing a query
		/// </remarks>
		/// <returns>
		/// A <see cref="IDbCommand"/> object.
		/// </returns>
		protected abstract IDbCommand CreateCommand();

		/// <summary>
		/// Executes a stored procedure that does not return a result.
		/// </summary>
		/// <param name="spName">
		/// Name of the stored procedure to execute.
		/// </param>
		/// <param name="parameters">
		/// Parameters to pass to the stored procedure.
		/// </param>
		/// <returns>
		/// The no of rows affected by the operation.
		/// </returns>
		public int ExecuteNonQuery(string spName,
									params IDataParameter[] parameters)
		{
			return ExecuteNonQuery(spName, CommandType.StoredProcedure, parameters);
		}

		/// <summary>
		/// Executes an sql statement that does not return a result.
		/// </summary>
		/// <param name="sql">
		/// The SQL statement to execute
		/// </param>
		/// <param name="parameters">
		/// Parameters to that are used by the statement.
		/// </param>
		/// <returns>
		/// The no of rows affected by the operation.
		/// </returns>
		public int ExecuteSqlNonQuery(string sql, params IDataParameter[] parameters)
		{
			try
			{
				return ExecuteNonQuery(sql, CommandType.Text, parameters);
			}
			catch (Exception e)
			{
				throw new DataToolException("Error executing sql query: " + sql, e);
			}
		}

		private int ExecuteNonQuery(string commandText, CommandType commandType,
									 params IDataParameter[] parameters)
		{
			AssertNotDisposed();
			using (IDbCommand command = CreateCommand())
			{
				command.Connection = GetConnection();
				command.Transaction = GetTransaction();
				command.CommandText = commandText;
				command.CommandType = commandType;

				foreach (IDataParameter parameter in parameters)
					command.Parameters.Add(parameter);

				return command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Executes a stored procedure that returns a single value.
		/// </summary>
		/// <param name="spName">
		/// Name of the stored procedure to execute.
		/// </param>
		/// <param name="parameters">
		/// Parameters to pass to the stored procedure.
		/// </param>
		/// <returns>
		/// The object returned by the stored procedure if any.
		/// If the stored procedure returns a NULL value or
		/// no value, <c>null</c> will be returned.
		/// </returns>
		public object ExecuteScalar(string spName,
									params IDataParameter[] parameters)
		{
			const CommandType commandType = CommandType.StoredProcedure;
			return ExecuteScalar(spName, commandType, parameters);
		}

		private object ExecuteScalar(string commandText,
									 CommandType commandType,
									 params IDataParameter[] parameters)
		{
			AssertNotDisposed();
			using (IDbCommand command = CreateCommand())
			{
				command.Connection = GetConnection();
				command.Transaction = GetTransaction();
				command.CommandText = commandText;
				command.CommandType = commandType;

				foreach (IDataParameter parameter in parameters)
					command.Parameters.Add(parameter);

				object retVal = command.ExecuteScalar();
				return retVal == DBNull.Value ? null : retVal;
			}
		}

		/// <summary>
		/// Executes an SQL text statement that returns a single value.
		/// </summary>
		/// <returns>The return value of the SQL statement</returns>
		public object ExecuteSqlScalar(string sql,
									   params IDataParameter[] parameters)
		{
			return ExecuteScalar(sql, CommandType.Text, parameters);
		}

		/// <summary>
		/// Executes an SQL statement that returns an <see cref="IDataReader"/>
		/// </summary>
		public virtual IDataReader ExecuteSqlReader(string sql,
													params IDataParameter[] parameters)
		{
			return ExecuteReader(
				sql, CommandType.Text,
				CommandBehavior.SingleResult,
				parameters);
		}

		/// <summary>
		/// Executes an SQL statement that returns a single row.
		/// </summary>
		public virtual IDataReader ExecuteSqlReaderSingleRow(string sql,
															 params IDataParameter[] parameters)
		{
			return ExecuteReader(
				sql, CommandType.Text,
				CommandBehavior.SingleRow,
				parameters);
		}

		/// <summary>
		/// Executes a stored procedure that returns a single result set.
		/// </summary>
		/// <param name="spName">
		/// Name of the stored procedure to execute.
		/// </param>
		/// <param name="parameters">
		/// Parameters to pass to the stored procedure.
		/// </param>
		/// <returns>
		/// A <see cref="IDataReader"/> instance for reading the data.
		/// </returns>
		public IDataReader ExecuteReader(string spName,
										  params IDataParameter[] parameters)
		{
			return ExecuteReader(spName, CommandType.StoredProcedure,
								  CommandBehavior.SequentialAccess |
								  CommandBehavior.SingleResult, parameters);
		}

		///// <summary>
		///// Executes a stored procedure that returns a multiple result sets.
		///// </summary>
		///// <param name="spName">
		///// Name of the stored procedure to execute.
		///// </param>
		///// <param name="parameters">
		///// Parameters to pass to the stored procedure.
		///// </param>
		///// <returns>
		///// A <see cref="IDataReader"/> instance for reading the data.
		///// </returns>
		//public IDataReader ExecuteReaderMultiResults( string spName,
		//    params IDataParameter[] parameters )
		//{
		//    return ExecuteReader( spName, CommandType.StoredProcedure,
		//        CommandBehavior.SequentialAccess, parameters );
		//}


		/// <summary>
		/// Executes a stored procedure that returns a single row.
		/// </summary>
		/// <param name="spName">
		/// Name of the stored procedure to execute.
		/// </param>
		/// <param name="parameters">
		/// Parameters to pass to the stored procedure.
		/// </param>
		/// <returns>
		/// A <see cref="IDataReader"/> instance for reading the data.
		/// </returns>
		public IDataReader ExecuteReaderSingleRow(string spName,
												   params IDataParameter[] parameters)
		{
			return ExecuteReader(spName, CommandType.StoredProcedure,
								  CommandBehavior.SequentialAccess |
								  CommandBehavior.SingleRow, parameters);
		}

		private IDataReader ExecuteReader(string sql,
										  CommandType commandType, CommandBehavior behavior,
										  params IDataParameter[] parameters)
		{
			AssertNotDisposed();
			using (IDbCommand command = CreateCommand())
			{
				command.Connection = GetConnection();
				command.Transaction = GetTransaction();

				command.CommandType = commandType;
				command.CommandText = sql;
				foreach (IDataParameter parameter in parameters)
					command.Parameters.Add(parameter);

				return command.ExecuteReader(behavior);
			}
		}

		#region Create parameter methods

		/// <summary>
		/// Creates database parameter for a string value.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter.
		/// </param>
		/// <param name="value">
		/// The value of the parameter
		/// </param>
		/// <param name="length">
		/// The length of the parameter as it is specified by the stored procedure.
		/// </param>
		/// <remarks>
		/// The implementation will check if the length of the string
		/// is actually within the acceptable length. If not, a 
		/// <see cref="StringParameterTooLongException"/> is thrown.
		/// </remarks>
		/// <returns>
		/// An <see cref="IDataParameter"/> instance that can be passed
		/// to any of the execute functions
		/// </returns>
		/// <exception cref="StringParameterTooLongException">
		/// The length of <c>value</c> exceeds that specified by
		/// <c>length</c>
		/// </exception>
		public abstract IDataParameter CreateStringParameter(
			string parameterName, string value, int? length);

		/// <summary>
		/// Creates database parameter for an integer value.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter.
		/// </param>
		/// <param name="value">
		/// The value of the parameter
		/// </param>
		/// <returns>
		/// An <see cref="IDataParameter"/> instance that can be passed
		/// to any of the execute functions
		/// </returns>
		public abstract IDataParameter CreateIntParameter(
			string parameterName, int? value);

		/// <summary>
		/// Creates database parameter for a GUID value.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter.
		/// </param>
		/// <param name="value">
		/// The value of the parameter
		/// </param>
		/// <returns>
		/// An <see cref="IDataParameter"/> instance that can be passed
		/// to any of the execute functions
		/// </returns>
		public abstract IDataParameter CreateGuidParameter(
			string parameterName, Guid? value);

		/// <summary>
		/// Creates database parameter for an integer value.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter.
		/// </param>
		/// <param name="value">
		/// The value of the parameter
		/// </param>
		/// <returns>
		/// An <see cref="IDataParameter"/> instance that can be passed
		/// to any of the execute functions
		/// </returns>
		public virtual IDataParameter CreateIntParameter(
			string parameterName, int value)
		{
			return CreateIntParameter(parameterName, (int?)value);
		}

		/// <summary>
		/// Creates a database parameter for a stamp value.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter.
		/// </param>
		/// <param name="stamp">
		/// The value of the parameter
		/// </param>
		/// <returns>
		/// An <see cref="IDataParameter"/> instance that can be passed
		/// to any of the execute functions
		/// </returns>
		public abstract IDataParameter CreateStampParameter(
			string parameterName, object stamp);

		/// <summary>
		/// Creates a database parameter for a Boolean value.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter.
		/// </param>
		/// <param name="value">
		/// The value of the parameter
		/// </param>
		/// <returns>
		/// An <see cref="IDataParameter"/> instance that can be passed
		/// to any of the execute functions
		/// </returns>
		public abstract IDataParameter CreateBoolParameter(
			string parameterName, bool value);

		/// <summary>
		/// Creates database parameter for a binary value.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter.
		/// </param>
		/// <param name="value">
		/// The value of the parameter
		/// </param>
		/// <param name="length">
		/// The length of the parameter as it is specified by the stored procedure.
		/// </param>
		/// <remarks>
		/// The implementation will check if the length of the string
		/// is actually within the acceptable length. If not, a 
		/// <see cref="BinaryParameterTooLongException"/> is thrown.
		/// </remarks>
		/// <returns>
		/// An <see cref="IDataParameter"/> instance that can be passed
		/// to any of the execute functions
		/// </returns>
		/// <exception cref="BinaryParameterTooLongException">
		/// The size of <c>value</c> exceeds that specified by
		/// <c>length</c>
		/// </exception>
		public abstract IDataParameter CreateBinaryParameter(
			string parameterName, byte[] value, int? length);

		/// <summary>
		/// Creates a database parameter for a datetime value
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter.
		/// </param>
		/// <param name="value">
		/// The value of the parameter
		/// </param>
		/// <returns>
		/// An <see cref="IDataParameter"/> instance that can be passed
		/// to any of the execute functions
		/// </returns>
		public abstract IDataParameter CreateDateParameter(
			string parameterName, DateTime? value);

		/// <summary>
		/// Creates a new SQL parameter representing a <see cref="DateTime"/> object
		/// </summary>
		/// <param name="parameterName">The name of the parameter</param>
		/// <param name="value">The value of the parameter</param>
		/// <returns>
		/// An <see cref="IDataParameter"/> instance that can be passed
		/// to any of the execute functions
		/// </returns>
		public virtual IDataParameter CreateDateParameter(
			string parameterName, DateTime value)
		{
			return CreateDateParameter(parameterName, (DateTime?)value);
		}

		/// <summary>
		/// Creates a database parameter for a decimal value used for carrying
		/// monetary amount.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter.
		/// </param>
		/// <param name="value">
		/// The value of the parameter
		/// </param>
		/// <returns>
		/// An <see cref="IDataParameter"/> instance that can be passed
		/// to any of the execute functions
		/// </returns>
		public abstract IDataParameter CreateMoneyParameter(
			string parameterName, decimal value);

		#endregion

		/// <summary>
		/// Disposes the current instance.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Template function for disposing resources.
		/// </summary>
		/// <param name="disposing">
		/// Boolean value indicating if we are calling from dispose or the finalizer.
		/// <c>true</c> indicates we are calling from Dispose; <c>false</c> indicates
		/// we are calling from the finalizer.
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			_isDisposed = true;
		}

		/// <summary>
		/// Starts a new transaction. Specialized classes should override this
		/// function and implement custom transaction handling
		/// </summary>
		public abstract void BeginTransaction();

		/// <summary>
		/// Commits the transaction. Specialized classes should override this
		/// function and implement custom transaction handling
		/// </summary>
		public abstract void CommitTransaction();

		/// <summary>
		/// Rolls back the transaction. Specialized classes should override this
		/// function and implement custom transaction handling
		/// </summary>
		public abstract void RollbackTransaction();

		/// <summary>
		/// Verifies that the object has not been disposed.
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		protected void AssertNotDisposed()
		{
			if (_isDisposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		/// <summary>
		/// Executes a query represendted by a <see cref="DataQuery"/> instance
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public IDataReader ExecuteQuery(DataQuery query)
		{
			var result = query.Parse(this);
			return ExecuteSqlReader(result.Sql, result.Parameters.ToArray());
		}
	}
}