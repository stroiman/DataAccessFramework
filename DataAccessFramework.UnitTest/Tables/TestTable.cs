using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessFramework.Querying;

namespace DataAccessFramework.UnitTest.Tables
{
    public class TestTable : QueryTable
    {
        public TestTable() : base("Test")
        {
            DateTimeField = Field("DateTimeField");
        }

        public readonly FieldReference DateTimeField;
    }
}
