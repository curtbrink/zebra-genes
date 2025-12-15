using Csp.Core.Models.Models.Domain;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Zebra.Constraints;

namespace Csp.Puzzles.Zebra.Tests.Constraints;

public class AdjacentConstraintTests
{
    private readonly Variable _varA = new ("A");
    private readonly Variable _varB = new ("B");

    private readonly List<int> _initialZebraDomain = [1, 2, 3, 4, 5];

    [Fact]
    public void AdjacentConstraint_IsSatisfiable_WithDefaultZebraDomains()
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [_varA] = new Domain<int>(_initialZebraDomain),
            [_varB] = new Domain<int>(_initialZebraDomain)
        };

        var domainStore = new DomainStore<int>(dict);
        
        var sut = CreateAdjacentConstraint();

        Assert.True(sut.IsSatisfiable(domainStore));
    }
    
    [Fact]
    public void AdjacentConstraint_IsSatisfiable_WithForcedAdjacentDomains()
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [_varA] = new Domain<int>(4),
            [_varB] = new Domain<int>(5),
        };

        var domainStore = new DomainStore<int>(dict);
        
        var sut = CreateAdjacentConstraint();

        Assert.True(sut.IsSatisfiable(domainStore));
    }
    
    [Fact]
    public void AdjacentConstraint_IsSatisfiable_WithSomeDisjointButValidDomains()
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [_varA] = new Domain<int>(1, 3),
            [_varB] = new Domain<int>(4, 5),
        };

        var domainStore = new DomainStore<int>(dict);
        
        var sut = CreateAdjacentConstraint();

        Assert.True(sut.IsSatisfiable(domainStore));
    }
    
    [Fact]
    public void AdjacentConstraint_IsNotSatisfiable_WithNonAdjacentDomains()
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [_varA] = new Domain<int>(1, 2),
            [_varB] = new Domain<int>(4, 5),
        };

        var domainStore = new DomainStore<int>(dict);
        
        var sut = CreateAdjacentConstraint();

        Assert.False(sut.IsSatisfiable(domainStore));
    }

    private AdjacentConstraint CreateAdjacentConstraint() => new(_varA, _varB);
}