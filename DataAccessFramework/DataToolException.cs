using System;
using System.Runtime.Serialization;

namespace DataAccessFramework
{
	/// <summary>
	/// Exception throw by the datahelper class
	/// </summary>
	[Serializable]
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
	/// Common base class for <see cref="BinaryParameterTooLongException"/>
	/// and <see cref="StringParameterTooLongException"/>
	/// </summary>
	[Serializable]
	public class ParameterTooLongException : DataToolException
	{
		protected string ParameterName { get; private set; }
		protected int MaxLength { get; private set; }
		protected int ActualLength { get; private set; }

		/// <summary>
		/// Creates a new <c>BinaryParameterTooLongException</c> instance.
		/// </summary>
		/// <param name="parameterName">The name of the database parameter</param>
		/// <param name="maxLength">The max accepted length of the parameter</param>
		/// <param name="actualLength">The actual size of the passed parameter value</param>
		public ParameterTooLongException(
			string parameterName, int maxLength, int actualLength)
		{
			ParameterName = parameterName;
			MaxLength = maxLength;
			ActualLength = actualLength;
		}

		protected ParameterTooLongException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			ParameterName = info.GetString("parameterName");
			MaxLength = info.GetInt32("maxLength");
			ActualLength = info.GetInt32("actualLength");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parameterName", ParameterName);
			info.AddValue("maxLength", MaxLength);
			info.AddValue("actualLength", ActualLength);
		}
	}

	/// <summary>
	/// Exception thrown when trying to create a binary parameter
	/// that is bigger than the accepted max-length.
	/// </summary>
	[Serializable]
	public class BinaryParameterTooLongException : ParameterTooLongException
	{
		/// <summary>
		/// Creates a new <c>BinaryParameterTooLongException</c> instance.
		/// </summary>
		/// <param name="parameterName">The name of the database parameter</param>
		/// <param name="maxLength">The max accepted length of the parameter</param>
		/// <param name="actualLength">The actual size of the passed parameter value</param>
		public BinaryParameterTooLongException(
			string parameterName, int maxLength, int actualLength) 
			: base(parameterName, maxLength, actualLength)
		{}

		protected BinaryParameterTooLongException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{}

		/// <summary>
		/// Gets a string describing the exception
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(
				"Error creating binary parameter. Value too long. " +
				"Parameter name: {0}. Max length: {1}. Actual length: {2}\n\r",
					ParameterName, MaxLength, ActualLength) + "/n/r" +
				base.ToString();
		}
	}

	/// <summary>
	/// Exception thrown when trying to create a string parameter
	/// that is longer than the accepted max-length.
	/// </summary>
	[Serializable]
	public class StringParameterTooLongException : ParameterTooLongException
	{
		/// <summary>
		/// Creates a new <c>StringParameterTooLongException</c> instance.
		/// </summary>
		/// <param name="parameterName">The name of the database parameter</param>
		/// <param name="maxLength">The max accepted length of the parameter</param>
		/// <param name="actualLength">The actual size of the passed parameter value</param>
		public StringParameterTooLongException(
			string parameterName, int maxLength, int actualLength)
			: base(parameterName, maxLength, actualLength)
		{ }

		/// <summary>
		/// Creates a new <c>StringParameterTooLongException</c> instance.
		/// </summary>
		protected StringParameterTooLongException(SerializationInfo info, StreamingContext context)
			:base(info, context)
		{}

		/// <summary>
		/// Gets a string describing the exception
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(
				"Error creating string parameter. Value too long. " +
				"Parameter name: {0}. Max length: {1}. Actual length: {2}\n\r",
					ParameterName, MaxLength, ActualLength) + "/n/r" +
				base.ToString();
		}
	}
}