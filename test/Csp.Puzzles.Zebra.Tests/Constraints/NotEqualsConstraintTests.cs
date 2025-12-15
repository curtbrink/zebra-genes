using Csp.Puzzles.Zebra.Constraints;

namespace Csp.Puzzles.Zebra.Tests.Constraints;

public class NotEqualsConstraintTests : BaseZebraTests
{
    [Theory]
    [InlineData(null , null, true)]
    [InlineData(new [] {1, 2, 3}, new [] {1, 2, 3}, true)]
    [InlineData(new [] {3}, new [] {1, 2, 3}, true)]
    [InlineData(new [] {1, 2, 3}, new [] {1}, true)]
    [InlineData(new [] {1, 2, 3}, new [] {4, 5}, true)]
    [InlineData(new [] {2, 3}, new [] {1, 2}, true)]
    [InlineData(new [] {2, 3}, new [] {1, 5}, true)]
    [InlineData(new [] {2}, new [] {2}, false)]
    public void NotEqualsConstraint_IsSatisfiable_WithValidDomains(int[]? dA, int[]? dB, bool isValid)
    {
        var domainStore = GetDomainStore(dA, dB);
        
        var sut = CreateNotEqualsConstraint();

        Assert.Equal(isValid, sut.IsSatisfiable(domainStore));
    }
    
    private NotEqualsConstraint CreateNotEqualsConstraint() => new(VarA, VarB);
}