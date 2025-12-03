using Generator.Zebra.Clues.Types;

namespace Generator.Zebra.Clues.Abstract;

public abstract record OrderedBinaryAttributeClue<T>(string Category1, string Value1, string Category2, string Value2)
    : BinaryAttributeClue(Category1, Value1, Category2, Value2)
    where T : OrderedBinaryAttributeClue<T>
{
    public override bool IsEquivalentTo(Clue? other) =>
        other is T otherT && AttributesAreEqual(A, otherT.A) && AttributesAreEqual(B, otherT.B);
    
    // order matters for the before clues
    public override bool Contradicts(Clue? other)
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

    private bool ContradictsOrder<TOther>(OrderedBinaryAttributeClue<TOther> other)
        where TOther : OrderedBinaryAttributeClue<TOther> =>
        AttributesAreEqual(A, other.B) && AttributesAreEqual(B, other.A);
}