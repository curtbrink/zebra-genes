namespace Generator.Zebra.Clue;

public abstract record BaseClue<TSelf> where TSelf : BaseClue<TSelf>
{
    // does this clue encode exactly the same information as another
    public abstract bool IsEquivalentTo(TSelf? other);

    // does this clue intrinsically contradict another clue
    public abstract bool Contradicts(TSelf? other);

    // does this clue intrinsically imply another clue (should also include equivalency)
    public virtual bool Implies(TSelf? other) => IsEquivalentTo(other);
}