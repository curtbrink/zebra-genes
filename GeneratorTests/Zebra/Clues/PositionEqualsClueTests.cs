using Generator.Zebra.Clues.Types;

namespace GeneratorTests.Zebra.Clues;

public class PositionEqualsClueTests
{
    private const string CatA = "Cat1";
    private const string ValA = "Val1";
    private const int PosA = 17;

    private const string CatB = "Cat2";
    private const string ValB = "Val2";
    private const int PosB = 11;

    [Theory]
    [InlineData(CatA, ValA, PosA, true)]
    [InlineData(CatA, ValA, PosB, false)]
    [InlineData(CatA, ValB, PosA, false)]
    [InlineData(CatA, ValB, PosB, false)]
    [InlineData(CatB, ValA, PosA, false)]
    [InlineData(CatB, ValA, PosB, false)]
    [InlineData(CatB, ValB, PosA, false)]
    [InlineData(CatB, ValB, PosB, false)]
    public void PositionEqualsClue_IsEquivalentToItself(string a, string b, int c, bool isEquivalent)
    {
        // always test against A
        var sut = BuildClue(CatA, ValA, PosA);
        
        // pull values to test
        var other = BuildClue(a, b, c);
        
        Assert.Equal(isEquivalent, sut.IsEquivalentTo(other));
    }

    [Fact]
    public void PositionEqualsClue_IsNotEquivalentToNull()
    {
        var sut = BuildClue(CatA, ValA, PosA);
        Assert.False(sut.IsEquivalentTo(null));
    }

    [Fact]
    public void PositionEqualsClue_DoesNotContradictNull()
    {
        var sut = BuildClue(CatA, ValA, PosA);
        Assert.False(sut.IsEquivalentTo(null));
    }
    
    [Fact]
    public void PositionEqualsClue_DoesNotContradictItself()
    {
        var sut = BuildClue(CatA, ValA, PosA);
        
        var sameByRef = sut;
        Assert.False(sut.Contradicts(sameByRef));

        var sameByVal = BuildClue(CatA, ValA, PosA);
        Assert.False(sut.Contradicts(sameByVal));
    }
    
    [Fact]
    public void PositionEqualsClue_DoesNotContradictOtherTypes()
    {
        var sut = BuildClue(CatA, ValA, PosA);

        var test = new AttributeIsBeforeClue(CatA, ValA, CatB, ValB);
        Assert.False(sut.Contradicts(test));
    }
    
    [Fact]
    public void PositionEqualsClue_ContradictsOtherPositionEqualsClues_WithSameAttributeAndDifferentPos()
    {
        var sut = BuildClue(CatA, ValA, PosA);
        var test = BuildClue(CatA, ValA, PosB);
        Assert.True(sut.Contradicts(test));
    }
    
    [Fact]
    public void PositionEqualsClue_ContradictsOtherPositionEqualsClues_WithDifferentAttributeInSameCategoryAndSamePos()
    {
        var sut = BuildClue(CatA, ValA, PosA);
        var test = BuildClue(CatA, ValB, PosA);
        Assert.True(sut.Contradicts(test));
    }
    
    [Fact]
    public void PositionEqualsClue_DoesNotContradictPositionEqualsClues_WithDifferentAttributesAndPositions()
    {
        var sut = BuildClue(CatA, ValA, PosA);
        var test = BuildClue(CatB, ValB, PosB);
        Assert.False(sut.Contradicts(test));
    }

    [Fact]
    public void PositionEqualsClue_ImpliesItself()
    {
        var sut = BuildClue(CatA, ValA, PosA);

        var sameByRef = sut;
        Assert.True(sut.Implies(sameByRef));
        
        var sameByVal = BuildClue(CatA, ValA, PosA);
        Assert.True(sut.Implies(sameByVal));
    }
    
    [Fact]
    public void PositionEqualsClue_DoesNotImplyAnythingElse()
    {
        var sut = BuildClue(CatA, ValA, PosA);
        Assert.False(sut.Implies(null));

        var differentPos = BuildClue(CatA, ValA, PosB);
        Assert.False(sut.Implies(differentPos));
        
        var differentAttr = BuildClue(CatB, ValB, PosA);
        Assert.False(sut.Implies(differentAttr));
        
        var differentValInCategory = BuildClue(CatA, ValB, PosA);
        Assert.False(sut.Implies(differentValInCategory));
        
        var allDifferent = BuildClue(CatB, ValB, PosB);
        Assert.False(sut.Implies(allDifferent));

        var differentClueType = new AttributeIsBeforeClue(CatA, ValA, CatB, ValB);
        Assert.False(sut.Implies(differentClueType));
    }

    private static PositionEqualsClue BuildClue(string a, string b, int c) => new (a, b, c);
}