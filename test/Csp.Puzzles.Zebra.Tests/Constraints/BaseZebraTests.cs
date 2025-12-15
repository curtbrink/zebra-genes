using Csp.Core.Models.Models.Domain;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Zebra.Tests.Constraints;

public abstract class BaseZebraTests
{
    protected BaseVariable VarA { get; } = new ("A");
    protected BaseVariable VarB { get; } = new ("B");

    protected int[] InitialZebraDomain { get; } = [1, 2, 3, 4, 5];
    
    protected DomainStore<int> GetDomainStore(int[]? a, int[]? b)
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [VarA] = new ImmutableDomain<int>(a ?? InitialZebraDomain),
            [VarB] = new ImmutableDomain<int>(b ?? InitialZebraDomain),
        };

        return new DomainStore<int>(dict);
    }
}