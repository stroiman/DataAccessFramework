using System.Collections.Generic;

namespace DataAccessFramework.Querying
{
	/// <summary>
	/// A where part representing a full text search.
	/// </summary>
	public class FullTextClause : WherePart
	{
		private string _searchText;
		private List<FieldReference> _fieldReferences;

		/// <summary>
		/// Creates a new <c>FullTextClause</c> instance.
		/// </summary>
		/// <param name="searchText">
		/// The text to search for
		/// </param>
		/// <param name="fieldReferences">
		/// A reference to the fields in which to search.
		/// </param>
		public FullTextClause(string searchText, params FieldReference[] fieldReferences)
		{
			_searchText = searchText;
			_fieldReferences = new List<FieldReference>(fieldReferences);
		}

		/// <summary>
		/// Gets whether or not the fulltext search is active, i.e. does
		/// it provide any filtering. If the search text is only wildcard,
		/// then <c>false</c> is returned
		/// </summary>
		public override bool Active
		{
			get
			{
				if (string.IsNullOrEmpty(_searchText))
					return false;
				if (_searchText == "*")
					return false;
				if (_searchText == "\"*\"")
					return false;
				return true;
			}
		}

		/// <summary>
		/// Builds the SQL for the full text.
		/// </summary>
		internal override void BuildSql(BuildSqlContext sqlContext)
		{
			sqlContext.Builder.Append("contains(");
			foreach (FieldReference reference in _fieldReferences)
			{
				reference.BuildSql(sqlContext);
				sqlContext.Builder.Append(",");
			}
			new StringConstant(_searchText).BuildSql(sqlContext);
			sqlContext.Builder.Append(")");
		}
	}
}