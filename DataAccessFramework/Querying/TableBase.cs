namespace DataAccessFramework.Querying
{
	public abstract class TableBase
	{
		public abstract string TableName { get; }
		abstract internal void BuildSql(BuildSqlContext sqlContext);

		public Join LeftJoin(QueryTable table)
		{
			return new Join(this, table, JoinType.Left);
		}

		public Join InnerJoin(QueryTable table)
		{
			return new Join(this, table, JoinType.Inner);
		}

		public Query SelectWhere(WherePart condition)
		{
			var result = new SelectQuery();
			result.AddTable(this);
			result.AddWhere(condition);
			return result;
		}
	}
}