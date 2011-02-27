using System;
using System.Data;
using DataAccessFramework.Querying;
using Moq;
using NUnit.Framework;

namespace DataAccessFramework.UnitTest
{
    public class DataQueryTestBase
    {
        private Mock<DataTool> _dataToolMock = new Mock<DataTool>();
        protected string ExecutedSql;
        protected IDataParameter[] ExecutedParameters;

        [SetUp]
        public void DataQueryTestBaseSetup()
        {
            _dataToolMock = new Mock<DataTool> { CallBase = true };
            _dataToolMock.Setup(
                x => x.ExecuteSqlReader(It.IsAny<string>(), It.IsAny<IDataParameter[]>()))
                .Callback(
                    (string sql, IDataParameter[] parameters) =>
                    {
                        ExecutedSql = sql;
                        ExecutedParameters = parameters;
                    });
            var createParameterMock = (Func<string, object, IDataParameter>)CreateParameterMock;
            _dataToolMock.Setup(
                x => x.CreateIntParameter(It.IsAny<string>(), It.IsAny<int?>()))
                .Returns(createParameterMock);
            _dataToolMock.Setup(
                x => x.CreateStringParameter(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()))
                .Returns(
                    (string name, object value, int? length) => CreateParameterMock(name, value));
            _dataToolMock.Setup(
                x => x.CreateDateTimeParameter(It.IsAny<string>(), It.IsAny<DateTime?>()))
                .Returns(createParameterMock);
            _dataToolMock.Setup(
                x => x.CreateDecimalParameter(It.IsAny<string>(), It.IsAny<decimal?>()))
                .Returns(createParameterMock);
        }

        private static IDataParameter CreateParameterMock(string name, object value)
        {
            var parameterMock = new Mock<IDataParameter>();
            parameterMock.Setup(y => y.ParameterName).Returns(name);
            parameterMock.Setup(y => y.Value).Returns(value);
            return parameterMock.Object;
        }

        protected void Execute(Query query)
        {
            _dataToolMock.Object.ExecuteQuery(query);
        }

        protected static SelectQuery CreateSelectQuery()
        {
            return new SelectQuery();
        }
    }
}