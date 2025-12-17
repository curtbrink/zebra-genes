using Generator.Zebra.Zebra.Types;

namespace Generator.Zebra.Zebra.Clues.Abstract;

public abstract record BinaryAttributeClue(ZebraAttribute Attribute1, ZebraAttribute Attribute2) : ZebraClue
{
    protected bool InvolvesSameAttributes(ZebraClue? clue2)
    {
        if (clue2 is not BinaryAttributeClue bac2) return false;

        // two cases they involve the same attributes:
        // (a1 = a2, b1 = b2) || (a1 = b2, b1 = a2)
        return (Attribute1 == bac2.Attribute1 && Attribute2 == bac2.Attribute2) ||
               (Attribute1 == bac2.Attribute2 && Attribute2 == bac2.Attribute1);
    }
}