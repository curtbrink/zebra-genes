namespace Generator.Zebra.Clues.Abstract;

public abstract record Clue
{
    // does this clue encode exactly the same information as another
    public abstract bool IsEquivalentTo(Clue? other);
    
    // does this clue intrinsically contradict another clue
    public abstract bool Contradicts(Clue? other);
    
    // does this clue intrinsically imply another clue (should also include equivalency)
    public virtual bool Implies(Clue? other) => IsEquivalentTo(other);

    protected static bool AttributesAreEqual((string, string) attr1, (string, string) attr2) =>
        attr1.Item1 == attr2.Item1 && attr1.Item2 == attr2.Item2;

    protected static bool AttributesAreDifferentValueInSameCategory((string, string) attr1, (string, string) attr2) =>
        attr1.Item1 == attr2.Item1 && attr1.Item1 != attr2.Item2;
}
