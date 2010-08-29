using System.Collections.Generic;

namespace DataAccessFramework.Querying
{
	public abstract class TableBase
	{
		public abstract string TableName { get; }

		abstract internal void BuildSql(BuildSqlContext sqlContext);

		protected internal abstract IEnumerable<FieldReference> Fields { get; }

		public Join LeftJoin(QueryTable table)
		{
			return new Join(this, table, JoinType.Left);
		}

		public Join InnerJoin(QueryTable table)
		{
			return new Join(this, table, JoinType.Inner);
		}

		public Query Select()
		{
			var result = new SelectQuery();
			foreach (var field in Fields)
				result.AddSelectField(field);
			result.AddTable(this);
			return result;
		}

		public Query SelectWhere(WherePart condition)
		{
			var result = new SelectQuery();
			foreach (var field in Fields)
				result.AddSelectField(field);
			result.AddTable(this);
			result.AddWhere(condition);
			return result;
		}
	}
}