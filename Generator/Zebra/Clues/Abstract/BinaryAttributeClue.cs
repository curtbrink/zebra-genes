namespace Generator.Zebra.Clues.Abstract;

public abstract record BinaryAttributeClue(string Category1, string Value1, string Category2, string Value2) : Clue
{
    public (string, string) A => (Category1, Value1);
    public (string, string) B => (Category2, Value2);
    
    protected bool InvolvesSameAttributes(Clue? clue2)
    {
        if (clue2 is not BinaryAttributeClue bac2) return false;

        // two cases they involve the same attributes:
        // (a1 = a2, b1 = b2) || (a1 = b2, b1 = a2)
        return (AttributesAreEqual(A, bac2.A) && AttributesAreEqual(B, bac2.B)) ||
               (AttributesAreEqual(A, bac2.B) && AttributesAreEqual(B, bac2.A));
    }
}