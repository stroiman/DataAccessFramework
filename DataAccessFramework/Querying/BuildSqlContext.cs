using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessFramework.Querying
{
	/// <summary>
	/// Context for generating SQL. This contains all the data that is build up
	/// when generating SQL for a <see cref="Query"/>
	/// </summary>
	internal class BuildSqlContext
	{
		private readonly StringBuilder _builder;
		private readonly DataTool _dataTool;
		private readonly IList<IDataParameter> _parameters;
		private readonly Func<QueryTable, string> _resolveAlias;

		public BuildSqlContext(
			StringBuilder builder,
			DataTool dataTool,
			IList<IDataParameter> parameters,
			Func<QueryTable, string> resolveAlias)
		{
			_builder = builder;
			_dataTool = dataTool;
			_parameters = parameters;
			_resolveAlias = resolveAlias;
		}

		/// <summary>
		/// Gets a function for resolving the alias for a table.
		/// </summary>
		public Func<QueryTable, string> ResolveAlias
		{
			get { return _resolveAlias; }
		}

		/// <summary>
		/// Gets all the parameters that have been added so far.
		/// </summary>
		public IList<IDataParameter> Parameters
		{
			get { return _parameters; }
		}

		/// <summary>
		/// Gets a <see cref="DataTool"/> that can be used for creating new parameters
		/// </summary>
		public DataTool DataTool
		{
			get { return _dataTool; }
		}

		/// <summary>
		/// Gets a <see cref="StringBuilder"/> that is used to append to
		/// </summary>
		public StringBuilder Builder
		{
			get { return _builder; }
		}

        public string CreateNextParameterName()
        {
            var parameterNo = Parameters.Count + 1;
            return "p" + parameterNo;
        }
	}
}