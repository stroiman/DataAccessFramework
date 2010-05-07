using System;
using NUnit.Framework;
using Moq;

namespace DataAccessFramework.UnitTest
{
	[TestFixture]
	public class DataToolTests
	{
		Mock<DataTool> _dataToolMock;
		DataTool _dataTool;

		[SetUp]
		public void Setup()
		{
			_dataToolMock = new Mock<DataTool>();
			_dataToolMock.CallBase = true;
			_dataTool = _dataToolMock.Object;
		}

		[Test, ExpectedException(typeof(ObjectDisposedException))]
		public void ExecuteNonQuery_On_Disposed_DataTool_Throws_Exception()
		{
			// Execute
			_dataTool.Dispose();
			_dataTool.ExecuteNonQuery("test");
		}

		[Test, ExpectedException(typeof(ObjectDisposedException))]
		public void ExecuteScalar_On_Disposed_DataTool_Throws_Exception()
		{
			// Execute
			_dataTool.Dispose();
			_dataTool.ExecuteScalar("test");
		}

		[Test, ExpectedException(typeof(ObjectDisposedException))]
		public void ExecuteSqlScalar_On_Disposed_DataTool_Throws_Exception()
		{
			// Execute
			_dataTool.Dispose();
			_dataTool.ExecuteSqlScalar("test");
		}

		[Test, ExpectedException(typeof(ObjectDisposedException))]
		public void ExecuteSqlReader_On_Disposed_DataTool_Throws_Exception()
		{
			// Execute
			_dataTool.Dispose();
			_dataTool.ExecuteSqlReader("test");
		}

		[Test, ExpectedException(typeof(ObjectDisposedException))]
		public void ExecuteSqlReaderSingleRow_On_Disposed_DataTool_Throws_Exception()
		{
			// Execute
			_dataTool.Dispose();
			_dataTool.ExecuteSqlReaderSingleRow("test");
		}

		[Test, ExpectedException(typeof(ObjectDisposedException))]
		public void ExecuteReader_On_Disposed_DataTool_Throws_Exception()
		{
			// Execute
			_dataTool.Dispose();
			_dataTool.ExecuteReader("test");
		}

		[Test, ExpectedException(typeof(ObjectDisposedException))]
		public void ExecuteReaderSingleRow_On_Disposed_DataTool_Throws_Exception()
		{
			// Execute
			_dataTool.Dispose();
			_dataTool.ExecuteReaderSingleRow("test");
		}
	}
}


