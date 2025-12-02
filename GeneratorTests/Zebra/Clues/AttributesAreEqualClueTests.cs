using Generator.Zebra.Clues.Abstract;
using Generator.Zebra.Clues.Types;

namespace GeneratorTests.Zebra.Clues;

public class AttributesAreEqualClueTests
{
    private const string CatA = "CatA";
    private const string ValA = "ValA";

    private const string CatB = "CatB";
    private const string ValB = "ValB";

    private const string CatC = "CatC";
    private const string ValC = "ValC";

    [Fact]
    public void AttributesAreEqualClue_IsEquivalentToItself()
    {
        var sut = BuildClue(CatA, ValA, CatB, ValB);

        var sameByRef = sut;
        Assert.True(sut.IsEquivalentTo(sameByRef));

        var sameByVal = BuildClue(CatA, ValA, CatB, ValB);
        Assert.True(sut.IsEquivalentTo(sameByVal));
    }
    
    [Fact]
    public void AttributesAreEqualClue_IsNotEquivalentToNull()
    {
        var sut = BuildClue(CatA, ValA, CatB, ValB);
        var test = BuildClue(CatA, ValA, CatC, ValC);

        Assert.False(sut.IsEquivalentTo(test));
    }
    
    [Fact]
    public void AttributesAreEqualClue_IsEquivalentToOtherAttributesAreEqualClues_WithSameAttributes()
    {
        var sut = BuildClue(CatA, ValA, CatB, ValB);
        var test = BuildClue(CatB, ValB, CatA, ValA);

        Assert.True(sut.IsEquivalentTo(test));
    }
    
    [Fact]
    public void AttributesAreEqualClue_IsNotEquivalentToOtherAttributesAreEqualClues_WithDifferentAttributes()
    {
        var sut = BuildClue(CatA, ValA, CatB, ValB);
        var test = BuildClue(CatA, ValA, CatC, ValC);

        Assert.False(sut.IsEquivalentTo(test));
    }
    
    [Fact]
    public void AttributesAreEqualClue_IsNotEquivalentToCluesOfOtherTypes()
    {
        var sut = BuildClue(CatA, ValA, CatB, ValB);
        var test = new AttributeIsBeforeClue(CatA, ValA, CatB, ValB);

        Assert.False(sut.IsEquivalentTo(test));
    }

    [Fact]
    public void AttributesAreEqualCLue_DoesNotContradictNull()
    {
        var sut = BuildClue(CatA, ValA, CatB, ValB);
        Assert.False(sut.Contradicts(null));
    }
    
    [Fact]
    public void AttributesAreEqualClue_DoesNotContradictItself()
    {
        var sut = BuildClue(CatA, ValA, CatB, ValB);

        var sameByRef = sut;
        Assert.False(sut.Contradicts(sameByRef));

        var sameByVal = BuildClue(CatA, ValA, CatB, ValB);
        Assert.False(sut.Contradicts(sameByVal));
    }

    [Theory]
    [InlineData(0, false, false)]
    [InlineData(0, true, true)]
    [InlineData(1, false, false)]
    [InlineData(1, true, true)]
    [InlineData(2, false, false)]
    [InlineData(2, true, true)]
    public void AttributesAreEqualClue_ContradictsOtherBinaryClueTypes_WithSameAttributes(int idx, bool hasSame,
        bool isContradiction)
    {
        var otherAttr = hasSame ? (CatB, ValB) : (CatC, ValC);
        BinaryAttributeClue? test = idx switch
        {
            0 => new AttributeIsBeforeClue(CatA, ValA, otherAttr.Item1, otherAttr.Item2),
            1 => new AttributeIsImmediatelyBeforeClue(CatA, ValA, otherAttr.Item1, otherAttr.Item2),
            2 => new AttributesAreAdjacentClue(CatA, ValA, otherAttr.Item1, otherAttr.Item2),
            _ => null
        };
        
        Assert.NotNull(test);
        
        var sut = BuildClue(CatA, ValA, CatB, ValB);
        Assert.Equal(isContradiction, sut.Contradicts(test));
        
        // invert!
        BinaryAttributeClue? test2 = idx switch
        {
            0 => new AttributeIsBeforeClue(otherAttr.Item1, otherAttr.Item2, CatA, ValA),
            1 => new AttributeIsImmediatelyBeforeClue(otherAttr.Item1, otherAttr.Item2, CatA, ValA),
            2 => new AttributesAreAdjacentClue(otherAttr.Item1, otherAttr.Item2, CatA, ValA),
            _ => null
        };
        
        Assert.NotNull(test2);

        Assert.Equal(isContradiction, sut.Contradicts(test2));
    }

    [Fact]
    public void AttributesAreEqualClue_DoesNotContradictAttributesAreEqualClues_WithNoMatchingAttributes()
    {
        var sut = BuildClue(CatA, ValA, CatB, ValB);
        var test = BuildClue("not", "a", "matching", "set");
        Assert.False(sut.Contradicts(test));
    }

    [Theory]
    [InlineData(0, false, false)]
    [InlineData(0, true, true)]
    [InlineData(1, false, false)]
    [InlineData(1, true, true)]
    [InlineData(2, false, false)]
    [InlineData(2, true, true)]
    [InlineData(3, false, false)]
    [InlineData(3, true, true)]
    public void AttributesAreEqualClue_ContradictsCertainOtherAttributesAreEqualClues(int matchingIdx, 
        bool otherSameCategory, bool isContradiction)
    {
        // always the same sut
        var sut = BuildClue(CatA, ValA, CatB, ValB);

        var nonMatchingCat = !otherSameCategory ? CatC : matchingIdx is 0 or 1 ? CatB : CatA;
        var test = matchingIdx switch
        {
            0 => BuildClue(CatA, ValA, nonMatchingCat, ValC),
            1 => BuildClue(nonMatchingCat, ValC, CatA, ValA),
            2 => BuildClue(CatB, ValB, nonMatchingCat, ValC),
            3 => BuildClue(nonMatchingCat, ValC, CatB, ValB),
            _ => null
        };
        Assert.NotNull(test);
        
        Assert.Equal(isContradiction, sut.Contradicts(test));
    }
    
    [Fact]
    public void AttributesAreEqualClue_ImpliesItself()
    {
        var sut = BuildClue(CatA, ValA, CatB, ValB);

        var sameByRef = sut;
        Assert.True(sut.Implies(sameByRef));
        
        var sameByVal = BuildClue(CatA, ValA, CatB, ValB);
        Assert.True(sut.Implies(sameByVal));
    }
    
    [Fact]
    public void AttributesAreEqualClue_DoesNotImplyAnythingElse()
    {
        var sut = BuildClue(CatA, ValA, CatB, ValB);
        Assert.False(sut.Implies(null));

        var differentAttrValue = BuildClue(CatA, ValA, CatB, ValC);
        Assert.False(sut.Implies(differentAttrValue));
        
        var differentAttr = BuildClue(CatB, ValB, CatC, ValC);
        Assert.False(sut.Implies(differentAttr));
        
        var allDifferent = BuildClue("not", "a", "matching", "set");
        Assert.False(sut.Implies(allDifferent));

        var differentClueType = new AttributeIsBeforeClue(CatA, ValA, CatB, ValB);
        Assert.False(sut.Implies(differentClueType));
    }

    private static AttributesAreEqualClue BuildClue(string a, string a2, string b, string b2) => new(a, a2, b, b2);
}