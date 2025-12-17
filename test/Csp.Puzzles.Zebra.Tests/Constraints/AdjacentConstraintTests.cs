using Csp.Puzzles.Zebra.Constraints;

namespace Csp.Puzzles.Zebra.Tests.Constraints;

public class AdjacentConstraintTests : BaseZebraTests
{
    [Theory]
    [InlineData(null, null, true)]
    [InlineData(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, true)]
    [InlineData(new[] { 3 }, new[] { 1, 2, 3 }, true)]
    [InlineData(new[] { 1, 2, 3 }, new[] { 1 }, true)]
    [InlineData(new[] { 1, 2, 3 }, new[] { 4, 5 }, true)]
    [InlineData(new[] { 2, 3 }, new[] { 1, 2 }, true)]
    [InlineData(new[] { 2, 3 }, new[] { 1, 5 }, true)]
    [InlineData(new[] { 2 }, new[] { 4, 5 }, false)]
    public void AdjacentConstraint_IsSatisfiable_WithValidDomains(int[]? dA, int[]? dB, bool isValid)
    {
        var domainStore = GetDomainStore(dA, dB);

        var sut = CreateAdjacentConstraint();
        
        Assert.Equal(isValid, sut.IsSatisfiable(domainStore));
    }

    private AdjacentConstraint CreateAdjacentConstraint() => new(VarA, VarB);
}