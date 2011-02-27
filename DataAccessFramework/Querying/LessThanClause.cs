using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessFramework.Querying
{
    /// <summary>
    /// Where clause for a less-than operator , i.e. &lt;
    /// </summary>
    public class LessThanClause : WherePart
    {
        private readonly WherePart _left;
        private readonly WherePart _right;

        /// <summary>
        /// Creates a new <see cref="LessThanClause"/>
        /// </summary>
        public LessThanClause(WherePart left, WherePart right)
        {
            _left = left;
            _right = right;
        }

        internal override void BuildSql(BuildSqlContext sqlContext)
        {
            _left.BuildSql(sqlContext);
            sqlContext.Builder.Append(" < ");
            _right.BuildSql(sqlContext);
        }
    }
}
