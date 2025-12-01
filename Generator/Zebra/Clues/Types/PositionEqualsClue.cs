using Generator.Zebra.Clues.Abstract;

namespace Generator.Zebra.Clues.Types;

public record PositionEqualsClue(string Category, string Value, int Position) : UnaryAttributeClue(Category, Value)
{
    public override bool IsEquivalentTo(Clue? other) =>
        other is PositionEqualsClue pec && InvolvesSameAttribute(pec) && pec.Position == Position;

    public override bool Contradicts(Clue? other)
    {
        if (other is not PositionEqualsClue pec) return false;
        if (IsEquivalentTo(other)) return false;
        
        return (InvolvesSameAttribute(pec) && Position != pec.Position) ||
               (InvolvesDifferentAttributeInSameCategory(pec) && Position == pec.Position);
    }
}