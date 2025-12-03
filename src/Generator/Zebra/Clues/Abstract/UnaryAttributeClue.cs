namespace Generator.Zebra.Clues.Abstract;

public abstract record UnaryAttributeClue(string Category, string Value) : Clue
{
    public (string, string) A => (Category, Value);

    protected bool InvolvesSameAttribute(Clue? clue2) =>
        clue2 is UnaryAttributeClue uac2 && AttributesAreEqual(A, uac2.A);

    protected bool InvolvesDifferentAttributeInSameCategory(Clue? clue2) =>
        clue2 is UnaryAttributeClue uac2 && AttributesAreDifferentValueInSameCategory(A, uac2.A);
}