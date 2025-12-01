using Generator.Zebra.Clues.Abstract;

namespace Generator.Zebra.Clues.Types;

public record AttributesAreEqualClue(string Category1, string Value1, string Category2, string Value2)
    : BinaryAttributeClue(Category1, Value1, Category2, Value2)
{
    public override bool IsEquivalentTo(Clue? other) =>
        other is AttributesAreEqualClue aec && InvolvesSameAttributes(aec);

    public override bool Contradicts(Clue? other)
    {
        if (other is null || IsEquivalentTo(other)) return false;

        return other switch
        {
            AttributesAreEqualClue aec => ContradictsAttributeEquals(aec),
            AttributeIsBeforeClue aibc => InvolvesSameAttributes(aibc),
            AttributesAreAdjacentClue aiac => InvolvesSameAttributes(aiac),
            AttributeIsImmediatelyBeforeClue aiibc => InvolvesSameAttributes(aiibc),
            _ => false
        };
    }

    private bool ContradictsAttributeEquals(AttributesAreEqualClue aec)
    {
        // known different attribute equals...
        // one attribute must be the same
        if (AttributesAreEqual(A, aec.A)) // a = otherA
        {
            // contradiction if b.category == otherB.category && b.value != otherB.value
            return AttributesAreDifferentValueInSameCategory(B, aec.B);
        }
        if (AttributesAreEqual(A, aec.B)) // a = otherB
        {
            // contradiction if b.category == otherA.category && b.value != otherA.value
            return AttributesAreDifferentValueInSameCategory(B, aec.A);
        }
        if (AttributesAreEqual(B, aec.A)) // b = otherA
        {
            // contradiction if a.category == otherB.category && a.value != otherB.value
            return AttributesAreDifferentValueInSameCategory(A, aec.B);
        }
        if (AttributesAreEqual(B, aec.B)) // b = otherB
        {
            // contradiction if a.category == otherA.category && a.value != otherA.value
            return AttributesAreDifferentValueInSameCategory(A, aec.A);
        }
        // else there isn't a matching attribute
        return false;
    }
}