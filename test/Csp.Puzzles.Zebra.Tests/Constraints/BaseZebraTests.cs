using Csp.Core.Models.Models.Domain;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Zebra.Tests.Constraints;

public abstract class BaseZebraTests
{
    protected Variable VarA { get; } = new ("A");
    protected Variable VarB { get; } = new ("B");

    protected int[] InitialZebraDomain { get; } = [1, 2, 3, 4, 5];
    
    protected virtual DomainStore<int> GetDomainStore(int[]? a, int[]? b)
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [VarA] = new Domain<int>(a ?? InitialZebraDomain),
            [VarB] = new Domain<int>(b ?? InitialZebraDomain),
        };

        return new DomainStore<int>(dict);
    }
}