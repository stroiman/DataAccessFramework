namespace DataAccessFramework.Querying
{
	public abstract class TableBase
	{
		public abstract string Name { get; }
		abstract internal void BuildSql(BuildSqlContext sqlContext);

		public Join LeftJoin(QueryTable table)
		{
			return new Join(this, table);
		}
	}
}