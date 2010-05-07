using System;
using System.Runtime.Serialization;

namespace DataAccessFramework
{
	/// <summary>
	/// Exception throw by the datahelper class
	/// </summary>
	public class DataToolException : Exception
	{
		/// <summary>
		/// Creates a new <c>DataToolException</c> instance
		/// </summary>
		public DataToolException()
			: base()
		{ }

		/// <summary>
		/// Creates a new <c>DataToolException</c> instance
		/// </summary>
		/// <param name="msg"></param>
		public DataToolException(string msg)
			: base(msg)
		{ }

		/// <summary>
		/// Creates a new <c>DataToolException</c> instance
		/// </summary>
		public DataToolException(string msg, Exception inner)
			: base(msg, inner)
		{
		}

		/// <summary>
		/// Creates a new <c>DataToolException</c> instance.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected DataToolException(SerializationInfo info,
									 StreamingContext context) :
			base(info, context)
		{ }
	}

	/// <summary>
	/// Exception thrown when trying to create a binary parameter
	/// that is bigger than the accepted max-length.
	/// </summary>
	public class BinaryParameterTooLongException : DataToolException
	{
		string _parameterName;
		int _maxLength;
		int _actualLength;

		/// <summary>
		/// Creates a new <c>BinaryParameterTooLongException</c> instance.
		/// </summary>
		/// <param name="parameterName">The name of the database parameter</param>
		/// <param name="maxLength">The max accepted length of the parameter</param>
		/// <param name="actualLength">The actual size of the passed parameter value</param>
		public BinaryParameterTooLongException(
			string parameterName, int maxLength, int actualLength)
			: base()
		{
			_parameterName = parameterName;
			_maxLength = maxLength;
			_actualLength = actualLength;
		}


		/// <summary>
		/// Gets a string describing the exception
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(
				"Error creating binary parameter. Value too long. " +
				"Parameter name: {0}. Max length: {1}. Actual length: {2}\n\r",
					_parameterName, _maxLength, _actualLength) + "/n/r" +
				base.ToString();
		}
	}

	/// <summary>
	/// Exception thrown when trying to create a string parameter
	/// that is longer than the accepted max-length.
	/// </summary>
	public class StringParameterTooLongException : DataToolException
	{
		string _parameterName;
		int _maxLength;
		int _actualLength;

		/// <summary>
		/// Creates a new <c>StringParameterTooLongException</c> instance.
		/// </summary>
		/// <param name="parameterName">The name of the database parameter</param>
		/// <param name="maxLength">The max accepted length of the parameter</param>
		/// <param name="actualLength">The actual size of the passed parameter value</param>
		public StringParameterTooLongException(
			string parameterName, int maxLength, int actualLength)
			: base()
		{
			_parameterName = parameterName;
			_maxLength = maxLength;
			_actualLength = actualLength;
		}

		/// <summary>
		/// Gets a string describing the exception
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(
				"Error creating string parameter. Value too long. " +
				"Parameter name: {0}. Max length: {1}. Actual length: {2}\n\r",
					_parameterName, _maxLength, _actualLength) + "/n/r" +
				base.ToString();
		}
	}
}