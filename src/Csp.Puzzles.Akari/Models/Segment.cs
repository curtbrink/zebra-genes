using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Akari.Models;

public abstract class Segment
{
    public string Name => $"{descriptor} seg at {anchorAxis}={_anchor},{segAxis}={_minIdx}-{_maxIdx}";

    public IList<IGridCellVariable> Cells { get; init; }
    
    private readonly bool _horizontal;
    private readonly int _anchor;
    private readonly int _minIdx;
    private readonly int _maxIdx;

    private string descriptor => _horizontal ? "Row" : "Col";
    private string anchorAxis => _horizontal ? "Y" : "X";
    private string segAxis => _horizontal ? "X" : "Y";

    protected Segment(IList<IGridCellVariable> cells, bool horizontal)
    {
        Cells = cells;
        _horizontal = horizontal;
        
        var yCoords = cells.Select(c => c.Y).ToList();
        var xCoords = cells.Select(c => c.X).ToList();

        if (horizontal)
        {
            // anchor should be Y coord
            if (yCoords.Distinct().Count() > 1)
                throw new ArgumentOutOfRangeException(nameof(cells), "Not a row of cells");
            if (xCoords.Distinct().Count() != xCoords.Count)
                throw new ArgumentOutOfRangeException(nameof(cells), "Cells are not unique");
            _anchor = yCoords.First();
            _minIdx = xCoords.Min();
            _maxIdx = xCoords.Max();
        }
        else
        {
            // anchor should be X coord
            if (xCoords.Distinct().Count() > 1)
                throw new ArgumentOutOfRangeException(nameof(cells), "Not a column of cells");
            if (yCoords.Distinct().Count() != xCoords.Count)
                throw new ArgumentOutOfRangeException(nameof(cells), "Cells are not unique");
            _anchor = xCoords.First();
            _minIdx = yCoords.Min();
            _maxIdx = yCoords.Max();
        }
    }

    public bool Contains(IGridCellVariable cell) => Cells.Contains(cell);
}

public class RowSegment(IList<IGridCellVariable> cells) : Segment(cells, true);

public class ColumnSegment(IList<IGridCellVariable> cells) : Segment(cells, false);