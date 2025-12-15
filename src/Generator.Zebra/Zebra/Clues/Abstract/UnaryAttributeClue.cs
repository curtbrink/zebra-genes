using Generator.Zebra.Zebra.Types;

namespace Generator.Zebra.Zebra.Clues.Abstract;

public abstract record UnaryAttributeClue(ZebraAttribute Attribute) : ZebraClue
{
    protected bool InvolvesSameAttribute(ZebraClue? clue2) =>
        clue2 is UnaryAttributeClue uac2 && Attribute == uac2.Attribute;

    protected bool InvolvesExclusiveAttribute(ZebraClue? clue2) =>
        clue2 is UnaryAttributeClue uac2 && Attribute.IsExclusiveWith(uac2.Attribute);
}