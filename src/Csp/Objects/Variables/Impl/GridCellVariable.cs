using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Variables.Impl;

public class GridCellVariable(int x, int y) : BaseVariable($"{x},{y}"), IGridCellVariable
{
    public int X { get; } = x;
    public int Y { get; } = y;
}