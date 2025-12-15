using Csp.Core.Models.Models.Domain;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Zebra.Constraints;

namespace Csp.Puzzles.Zebra.Tests.Constraints;

public class BeforeConstraintTests
{
    private readonly Variable _varA = new ("A");
    private readonly Variable _varB = new ("B");

    private readonly List<int> _initialZebraDomain = [1, 2, 3, 4, 5];

    [Fact]
    public void BeforeConstraint_IsSatisfiable_WithDefaultDomains()
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [_varA] = new Domain<int>(_initialZebraDomain),
            [_varB] = new Domain<int>(_initialZebraDomain)
        };

        var domainStore = new DomainStore<int>(dict);
        
        var sut = CreateBeforeConstraint();

        Assert.True(sut.IsSatisfiable(domainStore));
    }
    
    [Fact]
    public void BeforeConstraint_IsSatisfiable_WithForcedValidDomains()
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [_varA] = new Domain<int>(2),
            [_varB] = new Domain<int>(5),
        };

        var domainStore = new DomainStore<int>(dict);
        
        var sut = CreateBeforeConstraint();

        Assert.True(sut.IsSatisfiable(domainStore));
    }
    
    [Fact]
    public void BeforeConstraint_IsSatisfiable_WithOnlyOneForcedDomain()
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [_varA] = new Domain<int>(1),
            [_varB] = new Domain<int>(1, 3),
        };

        var domainStore = new DomainStore<int>(dict);
        
        var sut = CreateBeforeConstraint();

        Assert.True(sut.IsSatisfiable(domainStore));
    }
    
    [Fact]
    public void BeforeConstraint_IsNotSatisfiable_WithForcedEqualDomains()
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [_varA] = new Domain<int>(2),
            [_varB] = new Domain<int>(2),
        };

        var domainStore = new DomainStore<int>(dict);
        
        var sut = CreateBeforeConstraint();

        Assert.False(sut.IsSatisfiable(domainStore));
    }
    
    [Fact]
    public void BeforeConstraint_IsNotSatisfiable_WithForcedAfterDomains()
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [_varA] = new Domain<int>(4),
            [_varB] = new Domain<int>(2, 3),
        };

        var domainStore = new DomainStore<int>(dict);
        
        var sut = CreateBeforeConstraint();

        Assert.False(sut.IsSatisfiable(domainStore));
    }
    
    [Fact]
    public void BeforeConstraint_IsNotSatisfiable_WithMultipleDisjointDomains()
    {
        var dict = new Dictionary<IVariable, IDomain<int>>
        {
            [_varA] = new Domain<int>(4, 5),
            [_varB] = new Domain<int>(1, 2, 3, 4),
        };

        var domainStore = new DomainStore<int>(dict);
        
        var sut = CreateBeforeConstraint();

        Assert.False(sut.IsSatisfiable(domainStore));
    }

    private BeforeConstraint CreateBeforeConstraint() => new(_varA, _varB);
}