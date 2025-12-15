using Generator.Zebra.Zebra.Clues.Types;
using Generator.Zebra.Zebra.Types;

namespace Generator.Zebra.Zebra.Clues.Abstract;

public abstract record OrderedBinaryAttributeClue(ZebraAttribute Attribute1, ZebraAttribute Attribute2)
    : BinaryAttributeClue(Attribute1, Attribute2);

public abstract record OrderedBinaryAttributeClue<TSelf>(ZebraAttribute Attribute1, ZebraAttribute Attribute2)
    : OrderedBinaryAttributeClue(Attribute1, Attribute2)
    where TSelf : OrderedBinaryAttributeClue<TSelf>
{
    public override bool IsEquivalentTo(ZebraClue? other) => this == other; // must match type and order exactly
    
    // order matters for the before clues
    public override bool Contradicts(ZebraClue? other)
    {
        if (other is not BinaryAttributeClue bac || IsEquivalentTo(bac)) return false;

        return other switch
        {
            // contradicts any clues where A = B
            AttributesAreEqualClue aec => InvolvesSameAttributes(aec),
            // contradicts any before clues where B is before A
            AttributeIsBeforeClue aibc => ContradictsOrder(aibc),
            AttributeIsImmediatelyBeforeClue aiibc => ContradictsOrder(aiibc),
            _ => false
        };
    }

    private bool ContradictsOrder(OrderedBinaryAttributeClue other) =>
        Attribute1 == other.Attribute2 && Attribute2 == other.Attribute1;
}