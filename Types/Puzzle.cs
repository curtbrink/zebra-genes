namespace ZebraGenes.Types;

public class Puzzle(Dictionary<string, List<string>> categories, List<PuzzleHint> hints)
{
    public bool IsSolvable { get; private set; }
    public bool HasContradiction { get; private set; }

    public CspMatrix Matrix { get; } = new(categories);

    
}