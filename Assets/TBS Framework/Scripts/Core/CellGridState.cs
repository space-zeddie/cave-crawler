using System.Linq;

public abstract class CellGridState
{
    protected CellGrid _cellGrid;
    protected CellGridNet _cellGridNet;
    private bool network;
    
    protected CellGridState(CellGrid cellGrid)
    {
        _cellGrid = cellGrid;
        network = false;
    }
    protected CellGridState(CellGridNet cellGrid)
    {
        _cellGridNet = cellGrid;
        network = true;
    }

    public virtual void OnUnitClicked(Unit unit)
    { }
    public virtual void OnUnitClicked(UnitNet unit)
    { }
    
    public virtual void OnCellDeselected(Cell cell)
    {
        cell.UnMark();
    }
    public virtual void OnCellDeselected(CellNet cell)
    {
        cell.UnMark();
    }
    public virtual void OnCellSelected(Cell cell)
    {
        cell.MarkAsHighlighted();
    }
    public virtual void OnCellSelected(CellNet cell)
    {
        cell.MarkAsHighlighted();
    }
    public virtual void OnCellClicked(Cell cell)
    { }
    public virtual void OnCellClicked(CellNet cell)
    { }

    public virtual void OnStateEnter()
    {
        if (!network)
        {
            if (_cellGrid.Units.Select(u => u.PlayerNumber).Distinct().ToList().Count == 1)
            {
                _cellGrid.CellGridState = new CellGridStateGameOver(_cellGrid);
            }
        }
        else
        {
            if (_cellGridNet.Units.Select(u => u.PlayerNumber).Distinct().ToList().Count == 1)
            {
                _cellGridNet.CellGridState = new CellGridStateGameOver(_cellGridNet);
            }
        }
    }
    public virtual void OnStateExit()
    {
    }
}