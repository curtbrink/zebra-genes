using Generator.Zebra.Clues.Abstract;

namespace Generator.Zebra.Clues.Types;

public record AttributeIsImmediatelyBeforeClue(string Category1, string Value1, string Category2, string Value2)
    : OrderedBinaryAttributeClue<AttributeIsImmediatelyBeforeClue>(Category1, Value1, Category2, Value2)
{
    public override bool Contradicts(Clue? other)
    {
        // the same set of contradictions as "before" clues applies 
        if (base.Contradicts(other)) return true;
        
        // also contradicts this case:
        // if I'm saying "A:x is immediately before B:y"
        // then "A:x is immediately before B:z" has to be false,
        // and "A:w is immediately before B:y" has to be false,
        // but "A:x is immediately before C:1" can still be true,
        // and "D:2 is immediately before B:y" can still be true.
        if (other is not AttributeIsImmediatelyBeforeClue aiibc) return false;
        if (AttributesAreEqual(A, aiibc.A))
        {
            return AttributesAreDifferentValueInSameCategory(B, aiibc.B);
        }

        if (AttributesAreEqual(B, aiibc.B))
        {
            return AttributesAreDifferentValueInSameCategory(A, aiibc.A);
        }

        return false;
    }

    public override bool Implies(Clue? other)
    {
        if (base.Implies(other)) return true;
        
        if (other is not BinaryAttributeClue bac) return false;

        return bac switch
        {
            AttributeIsBeforeClue aibc => AttributesAreEqual(A, aibc.A) && AttributesAreEqual(B, aibc.B),
            AttributesAreAdjacentClue aaac => InvolvesSameAttributes(aaac),
            _ => false
        };
    }
}
