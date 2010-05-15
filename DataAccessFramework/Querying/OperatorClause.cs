using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataAccessFramework.Querying
{
	/// <summary>
	/// A simple case class for creating AND and OR operators.
	/// </summary>
	public class OperatorClause : WherePart
	{
		private readonly string _operatorName;
		private readonly List<WherePart> _parts;

		/// <summary>
		/// Gets all the active parts in the clause. <see cref="WherePart.Active"/>
		/// </summary>
		private IEnumerable<WherePart> ActiveParts
		{
			get
			{
				return _parts.Where(x => x.Active);
			}
		}

		/// <summary>
		/// Gets whether or not this part is active.
		/// </summary>
		public override bool Active
		{
			get
			{
				return _parts.Exists(x => x.Active);
			}
		}

		/// <summary>
		/// Creates a new <c>OperatorClause</c> instance
		/// </summary>
		public OperatorClause(string operatorName, params WherePart[] parts)
		{
			_operatorName = operatorName;
			_parts = new List<WherePart>();
			foreach (var part in parts)
				AddPart(part);
		}

		/// <summary>
		/// Builds the SQL for the operator.
		/// </summary>
		internal override void BuildSql(BuildSqlContext context)
		{
			var activeParts = new List<WherePart>(ActiveParts);
			if (activeParts.Count == 0)
				return;
			if (activeParts.Count == 1)
			{
				activeParts[0].BuildSql(context);
				return;
			}
			bool afterFirst = false;
			context.Builder.Append("(");
			foreach (var part in activeParts)
			{
				if (afterFirst)
				{
					context.Builder.Append(" ");
					context.Builder.Append(_operatorName);
					context.Builder.Append(" ");
				}
				part.BuildSql(context);
				afterFirst = true;
			}
			context.Builder.Append(")");
		}

		/// <summary>
		/// Adds a new part to the clause.
		/// </summary>
		/// <param name="clause"></param>
		public void AddPart(WherePart clause)
		{
			if (clause != null)
				_parts.Add(clause);
		}

		/// <summary>
		/// Gets the number of parts in the clause.
		/// </summary>
		public int Count { get { return _parts.Count; } }

		/// <summary>
		/// Gets whether or not the clause is empty, i.e. if there
		/// are any parts in it.
		/// </summary>
		public bool Empty { get { return _parts.Count == 0; } }

		/// <summary>
		/// Gets the parts in the clause. Mainly exists for unit testing, 
		/// to validate that query parsing builds up the correct data query.
		/// </summary>
		public ReadOnlyCollection<WherePart> Parts { get { return _parts.AsReadOnly(); } }
	}
}