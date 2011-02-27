using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessFramework.UnitTest.Tables;
using NUnit.Framework;

namespace DataAccessFramework.UnitTest
{
    /// <summary>
    /// Verification of the different comparison methods
    /// </summary>
    [TestFixture]
    public class DataQueryComparisonTest : DataQueryTestBase
    {
        private TestTable _testTable;

        [SetUp]
        public void Setup()
        {
            _testTable = new TestTable();
        }

        [Test]
        public void LessThanForDateTimeTest()
        {
            // Setup
            var value = DateTime.UtcNow;
            var query = _testTable.SelectWhere(_testTable.DateTimeField.LessThan(value));

            // Exercise
            Execute(query);

            // Validate
            var expected = @"t1.[DateTimeField] < @p1";
            var actual = ExecutedWhereClause;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
