using Csp.Puzzles.Zebra.Constraints;

namespace Csp.Puzzles.Zebra.Tests.Constraints;

public class OffsetConstraintTests : BaseZebraTests
{
    [Theory]
    [InlineData(null , null, true)]
    [InlineData(new [] {1, 2, 3}, new [] {1, 2, 3}, true)]
    [InlineData(new [] {3}, new [] {1, 2, 3}, false)]
    [InlineData(new [] {1, 2, 3}, new [] {1}, false)]
    [InlineData(new [] {1, 2, 3}, new [] {4, 5}, true)]
    [InlineData(new [] {2, 3}, new [] {1, 2}, false)]
    [InlineData(new [] {2, 3}, new [] {1, 4, 5}, true)]
    [InlineData(new [] {2}, new [] {2}, false)]
    public void OffsetConstraint_IsSatisfiable_WithValidDomains(int[]? dA, int[]? dB, bool isValid)
    {
        var domainStore = GetDomainStore(dA, dB);
        
        var sut = CreateOffsetConstraint();

        Assert.Equal(isValid, sut.IsSatisfiable(domainStore));
    }
    
    private OffsetConstraint CreateOffsetConstraint() => new(VarA, VarB);
}