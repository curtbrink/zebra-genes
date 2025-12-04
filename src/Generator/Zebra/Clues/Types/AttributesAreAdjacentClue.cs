using Generator.Zebra.Clues.Abstract;
using Generator.Zebra.Types;

namespace Generator.Zebra.Clues.Types;

public record AttributesAreAdjacentClue(ZebraAttribute Attribute1, ZebraAttribute Attribute2)
    : BinaryAttributeClue(Attribute1, Attribute2)
{
    public override bool IsEquivalentTo(ZebraClue? other) =>
        other is AttributesAreAdjacentClue aaac && InvolvesSameAttributes(aaac);

    public override bool Contradicts(ZebraClue? other) =>
        other is AttributesAreEqualClue aec && InvolvesSameAttributes(aec);
}