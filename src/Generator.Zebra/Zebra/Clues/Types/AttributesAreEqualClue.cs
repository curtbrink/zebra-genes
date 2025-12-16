using Generator.Zebra.Zebra.Clues.Abstract;
using Generator.Zebra.Zebra.Types;

namespace Generator.Zebra.Zebra.Clues.Types;

public record AttributesAreEqualClue(ZebraAttribute Attribute1, ZebraAttribute Attribute2)
    : BinaryAttributeClue(Attribute1, Attribute2)
{
    public override bool IsEquivalentTo(ZebraClue? other) =>
        other is AttributesAreEqualClue aec && InvolvesSameAttributes(aec);

    public override bool Contradicts(ZebraClue? other)
    {
        if (other is null || IsEquivalentTo(other)) return false;

        return other switch
        {
            AttributesAreEqualClue aec => ContradictsAttributeEquals(aec),
            OrderedBinaryAttributeClue obac => InvolvesSameAttributes(obac),
            AttributesAreAdjacentClue aiac => InvolvesSameAttributes(aiac),
            _ => false
        };
    }

    private bool ContradictsAttributeEquals(AttributesAreEqualClue aec)
    {
        // known different attribute equals...
        // a contradicts b in this case if exactly one attribute matches in a and b and the others are exclusive
        if (Attribute1 == aec.Attribute1)
        {
            return Attribute2.IsExclusiveWith(aec.Attribute2);
        }

        if (Attribute1 == aec.Attribute2)
        {
            return Attribute2.IsExclusiveWith(aec.Attribute1);
        }

        if (Attribute2 == aec.Attribute1)
        {
            return Attribute1.IsExclusiveWith(aec.Attribute2);
        }

        if (Attribute2 == aec.Attribute2)
        {
            return Attribute1.IsExclusiveWith(aec.Attribute1);
        }

        // else there isn't a matching attribute
        return false;
    }
}