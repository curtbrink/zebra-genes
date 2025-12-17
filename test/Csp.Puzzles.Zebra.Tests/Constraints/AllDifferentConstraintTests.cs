using Csp.Puzzles.Zebra.Constraints;

namespace Csp.Puzzles.Zebra.Tests.Constraints;

public class AllDifferentConstraintTests : BaseZebraTests
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
    public void AllDifferentConstraint_IsSatisfiable_WithValidDomains(int[]? dA, int[]? dB, bool isValid)
    {
        var domainStore = GetDomainStore(dA, dB);
        
        var sut = CreateAllDifferentConstraint();

        Assert.Equal(isValid, sut.IsSatisfiable(domainStore));
    }

    private AllDifferentConstraint CreateAllDifferentConstraint() => new([VarA, VarB], "testcategory");
}