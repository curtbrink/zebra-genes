namespace Csp.Puzzles.Polyomino.Models;

public record Placement(int X, int Y, int Variation, Polyomino P)
{
    public int Compare(Placement other)
    {
        if (X < other.X) return -1;
        if (X > other.X) return 1;

        if (Y < other.Y) return -1;
        if (Y > other.Y) return 1;

        return Variation - other.Variation;
    }
}