using Generator.Zebra.Clues.Abstract;

namespace Generator.Zebra.Clues.Types;

public record AttributesAreAdjacentClue(string Category1, string Value1, string Category2, string Value2)
    : BinaryAttributeClue(Category1, Value1, Category2, Value2)
{
    public override bool IsEquivalentTo(Clue? other) =>
        other is AttributesAreAdjacentClue aaac && InvolvesSameAttributes(aaac);

    public override bool Contradicts(Clue? other) => other is AttributesAreEqualClue aec && InvolvesSameAttributes(aec);
}