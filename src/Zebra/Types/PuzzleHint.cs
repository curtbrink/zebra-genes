namespace ZebraGenes.Types;

public enum HintType
{
    // "Primary is secondary."
    Equal,
    
    // "Primary is next to secondary."
    Adjacent,
    
    // "Primary is somewhere to the left of secondary."
    // Sets of these can also be combined when translating to natural language
    Before,
    
    // "Primary is immediately to the left of secondary."
    AdjacentBefore,
    
    // "Primary is in one of the positions noted in PossiblePositions."
    AbsolutePosition
}

public record PuzzleHint
{
    // for example, "Alice is somewhere to the left of the person who owns a tiger."
    // or, for position, "Alice is either in the first or fourth position."
    
    // The type of hint. (e.g. Before)
    public required HintType Type { get; init; }
    
    // The primary operating category. (e.g. Name|Alice)
    public required CategoryValue PrimaryValue { get; init; }
    
    // The secondary operating category. Null for absolute position clues. (e.g. Pet|Tiger)
    public CategoryValue? SecondaryValue { get; init; }
    
    // Possible absolute positions for "AbsolutePosition" hint types. (e.g. 1,4)
    public List<int>? PossiblePositions { get; init; }

    public bool IsValid =>
        (Type == HintType.AbsolutePosition && SecondaryValue is null && PossiblePositions is { Count: > 0 }) ||
        (Type != HintType.AbsolutePosition && PossiblePositions is null && SecondaryValue is not null);
}