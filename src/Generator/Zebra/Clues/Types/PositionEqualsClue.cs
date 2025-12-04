using Generator.Zebra.Clues.Abstract;
using Generator.Zebra.Types;

namespace Generator.Zebra.Clues.Types;

public record PositionEqualsClue(ZebraAttribute Attribute, int Position) : UnaryAttributeClue(Attribute)
{
    public override bool IsEquivalentTo(ZebraClue? other) => this == other;

    public override bool Contradicts(ZebraClue? other)
    {
        if (other is not PositionEqualsClue pec) return false;
        if (IsEquivalentTo(other)) return false;

        // one attribute can't be in multiple positions
        // two attributes of the same category can't be in the same position
        return (Attribute == pec.Attribute && Position != pec.Position) ||
               (Attribute.IsExclusiveWith(pec.Attribute) && Position == pec.Position);
    }
}