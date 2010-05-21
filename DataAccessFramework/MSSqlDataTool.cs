using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessFramework
{
	/// <summary>
	/// An abstract generalization of the <see cref="CreateCommand"/> that
	/// can connect to an MS SqlServer database. 
	/// </summary>
	/// <remarks>
	/// <p>
	/// This has to be extended in a specific class that can provide 
	/// logic for creating the connection.
	/// </p>
	/// <p>
	/// This implementation implements the <c>CreateXYZParameter</c> 
	/// functions and the <see cref="DataTool"/> function.
	/// </p>
	/// </remarks>
	public abstract class MSSqlDataTool : DataTool
	{
		/// <summary>
		/// Implements <see cref="DataTool.CreateCommand"/>
		/// </summary>
		/// <returns>
		/// An <see cref="SqlCommand"/> object.
		/// </returns>
		protected override IDbCommand CreateCommand()
		{
			return new SqlCommand();
		}

		/// <summary>
		/// Implements <see cref="DataTool.CreateStringParameter"/>. Creates 
		/// a database parameter for a string value.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter to create.
		/// </param>
		/// <param name="value">
		/// The value of the parameter
		/// </param>
		/// <param name="length">
		/// The max length of the parameter. If no value is passed, 
		/// a nvarchar(MAX) parameter is created.
		/// </param>
		/// <returns>
		/// An <see cref="SqlParameter"/> object.
		/// </returns>
		public override IDataParameter CreateStringParameter(string parameterName, string value, int? length)
		{
			if (length.HasValue && value != null)
			{
				if (value.Length > length)
					throw new StringParameterTooLongException(
						parameterName, length.Value, value.Length);
			}

			var parameter = new SqlParameter(parameterName, SqlDbType.NVarChar)
			{
				Value =
					value == null
						? DBNull.Value
						: (object)value
			};
			if (length.HasValue)
				parameter.Size = length.Value;
			return parameter;
		}

		/// <summary>
		/// Implements <see cref="DataTool.CreateIntParameter(string,int?)" />.
		/// Creates a database parameter for an integer value.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter to create.
		/// </param>
		/// <param name="value">
		/// The value for the parameter.
		/// </param>
		/// <returns>
		/// An <see cref="SqlParameter"/> object.
		/// </returns>
		public override IDataParameter CreateIntParameter(string parameterName, int? value)
		{
			var parameter = new SqlParameter(parameterName, SqlDbType.Int);
			if (value.HasValue)
				parameter.Value = value;
			else
				parameter.Value = DBNull.Value;
			return parameter;
		}

		public override IDataParameter CreateLongParameter(string parameterName, long? value)
		{
			return new SqlParameter(parameterName, SqlDbType.BigInt) 
			{
				Value = value.HasValue ? (object)value.Value : DBNull.Value
			};
		}

		/// <summary>
		/// Implements <see cref="DataTool.CreateBoolParameter(string,bool)" />.
		/// Creates a database parameter for a Boolean value.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter to create.
		/// </param>
		/// <param name="value">
		/// The value for the parameter.
		/// </param>
		/// <returns>
		/// An <see cref="SqlParameter"/> object.
		/// </returns>
		public override IDataParameter CreateBoolParameter(string parameterName, bool value)
		{
			var parameter = new SqlParameter(parameterName, SqlDbType.Bit) { Value = value };
			return parameter;
		}

		/// <summary>
		/// Creates a parameter for a timestamp field.
		/// </summary>
		public override IDataParameter CreateStampParameter(string parameterName, object value)
		{
			if (!(value is byte[]))
				throw new ArgumentOutOfRangeException("value", "Value must be a byte array");
			var parameter = new SqlParameter(parameterName, SqlDbType.Binary) { Value = value };
			return parameter;
		}

		/// <summary>
		/// Creats a parameter representing a moneray amount. I.e. decimal type.
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override IDataParameter CreateMoneyParameter(string parameterName, decimal value)
		{
			var parameter = new SqlParameter(parameterName, SqlDbType.Money) { Value = value };
			return parameter;
		}

		/// <summary>
		/// Implements <see cref="DataTool.CreateBinaryParameter"/>.
		/// Creates a database parameter for binary data.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter to create
		/// </param>
		/// <param name="value">
		/// The value of the parameter.
		/// </param>
		/// <param name="length">
		/// The length of the parameter. If no value is passed
		/// a binary(MAX) parameter is created.
		/// </param>
		/// <returns>
		/// An <see cref="SqlParameter"/> object.
		/// </returns>
		public override IDataParameter CreateBinaryParameter(string parameterName, byte[] value, int? length)
		{
			if (length.HasValue)
			{
				if (value.Length > length)
					throw new BinaryParameterTooLongException(
						parameterName, length.Value, value.Length);
			}

			var parameter = new SqlParameter(parameterName, SqlDbType.Binary) { Value = value };
			if (length.HasValue)
				parameter.Size = length.Value;
			return parameter;
		}

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
		public override IDataParameter CreateDateTimeParameter(string parameterName, DateTime? value)
		{
			var parameter = new SqlParameter(parameterName, SqlDbType.DateTime2);
			if (value.HasValue)
				parameter.Value = value.Value;
			else
				parameter.Value = DBNull.Value;
			return parameter;
		}

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
		public override IDataParameter CreateGuidParameter(string parameterName, Guid? value)
		{
			var parameter = new SqlParameter(parameterName, SqlDbType.UniqueIdentifier);
			if (value.HasValue)
				parameter.Value = value.Value;
			else
				parameter.Value = DBNull.Value;
			return parameter;
		}

		/// <summary>
		/// Creates a parameter for an object. The type of parameter will be
		/// deducted from the <param name="value"/> parameter.
		/// </summary>
		public override IDataParameter CreateParameter(string parameterName, object value)
		{
			return new SqlParameter(parameterName, value);
		}
	}
}