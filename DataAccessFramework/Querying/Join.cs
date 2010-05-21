using System;

namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Defines the type of join to execute
	/// </summary>
	public enum JoinType
	{
		Inner,
		Left
	}

	public class Join : TableBase
	{
		private readonly TableBase _left;
		private readonly QueryTable _right;
		private readonly JoinType _joinType;
		private WherePart _wherePart;

		public Join(TableBase left, QueryTable right, JoinType joinType)
		{
			_left = left;
			_right = right;
			_joinType = joinType;
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

		public override string TableName
		{
			get { return "JOIN"; }
		}

		internal override void BuildSql(BuildSqlContext sqlContext)
		{
			Left.BuildSql(sqlContext);
			var builder = sqlContext.Builder;
			switch (_joinType)
			{
				case JoinType.Left:
					builder.Append(" left outer join ");
					break;
				case JoinType.Inner:
					builder.Append(" inner join ");
					break;
				default:
					throw new InvalidOperationException("Unknown join type");
			}
			Right.BuildSql(sqlContext);
			if (_wherePart == null)
				return;
			builder.Append(" on ");
			_wherePart.BuildSql(sqlContext);
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