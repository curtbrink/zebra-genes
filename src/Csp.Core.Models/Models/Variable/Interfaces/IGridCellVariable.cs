namespace Csp.Core.Models.Models.Variable.Interfaces;

public interface IGridCellVariable : IVariable
{
    public int X { get; }
    public int Y { get; }
}