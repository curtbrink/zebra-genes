using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Selfref.Constraints;

public abstract class BaseSelfRefConstraint : IConstraint<string>
{
    protected readonly IList<string> Options = ["A", "B", "C", "D", "E"];

    public abstract string Name { get; }
    public abstract string Description { get; }
    
    public abstract IReadOnlyList<IVariable> Scope { get; }
    
    // set this flag when using as part of an OnlyTrueStatement, so that we always look in choiceList["A"].
    public bool OverrideSingle = false;

    public abstract bool IsSatisfiable(IDomainStore<string> domains);
}