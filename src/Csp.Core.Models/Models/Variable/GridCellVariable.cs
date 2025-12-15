using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Models.Models.Variable;

public class GridCellVariable(int x, int y) : Variable($"{x},{y}"), IGridCellVariable
{
    public int X { get; } = x;
    public int Y { get; } = y;
}