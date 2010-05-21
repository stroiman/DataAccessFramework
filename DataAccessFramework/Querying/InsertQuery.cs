using System;

namespace DataAccessFramework.Querying
{
	/// <summary>
	/// A specialized <see cref="DataQuery"/> that represents an insert sql query.
	/// </summary>
	public class InsertQuery : DataQuery
	{
		internal override ParseResult Parse(DataTool dataTool)
		{
			throw new NotImplementedException();
		}
	}
}