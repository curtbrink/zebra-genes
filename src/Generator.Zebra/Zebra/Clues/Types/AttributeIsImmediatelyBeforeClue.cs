using Generator.Zebra.Zebra.Clues.Abstract;
using Generator.Zebra.Zebra.Types;

namespace Generator.Zebra.Zebra.Clues.Types;

public record AttributeIsImmediatelyBeforeClue(ZebraAttribute Attribute1, ZebraAttribute Attribute2)
    : OrderedBinaryAttributeClue<AttributeIsImmediatelyBeforeClue>(Attribute1, Attribute2)
{
    public override bool Contradicts(ZebraClue? other)
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
        if (Attribute1 == aiibc.Attribute1)
        {
            return Attribute2.IsExclusiveWith(aiibc.Attribute2);
        }

        if (Attribute2 == aiibc.Attribute2)
        {
            return Attribute1.IsExclusiveWith(aiibc.Attribute1);
        }

        return false;
    }

    public override bool Implies(ZebraClue? other)
    {
        if (base.Implies(other)) return true;
        
        if (other is not BinaryAttributeClue bac) return false;

        return bac switch
        {
            AttributeIsBeforeClue aibc => Attribute1 == aibc.Attribute1 && Attribute2 == aibc.Attribute2,
            AttributesAreAdjacentClue aaac => InvolvesSameAttributes(aaac),
            _ => false
        };
    }
}
