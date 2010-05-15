namespace DataAccessFramework.Querying
{
	public class Join : TableBase
	{
		private readonly TableBase _left;
		private readonly QueryTable _right;
		private WherePart _wherePart;

		public Join(TableBase left, QueryTable right)
		{
			_left = left;
			_right = right;
		}

		public QueryTable Right
		{
			get { return _right; }
		}

		public TableBase Left
		{
			get { return _left; }
		}

		public Join On(WherePart clause)
		{
			_wherePart = clause;
			return this;
		}

		public override string Name
		{
			get { return "JOIN"; }
		}

		internal override void BuildSql(BuildSqlContext sqlContext)
		{
			Left.BuildSql(sqlContext);
			sqlContext.Builder.Append(" left outer join ");
			Right.BuildSql(sqlContext);
			if (_wherePart == null)
				return;
			sqlContext.Builder.Append(" on ");
			_wherePart.BuildSql(sqlContext);
		}
	}
}