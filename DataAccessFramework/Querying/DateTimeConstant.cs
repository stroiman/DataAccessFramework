using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessFramework.Querying
{
    /// <summary>
    /// A where part that represents a string constant. Will be converted to a parameter.
    /// </summary>
    public class DateTimeConstant : WherePart
    {
        private readonly DateTime _value;

        public DateTimeConstant(DateTime value)
        {
            _value = value;
        }

        internal override void BuildSql(BuildSqlContext context)
        {
            var parameterName = context.CreateNextParameterName();
            context.Parameters.Add(context.DataTool.CreateDateTimeParameter(parameterName, _value));
            context.Builder.Append("@");
            context.Builder.Append(parameterName);
        }
    }
}
