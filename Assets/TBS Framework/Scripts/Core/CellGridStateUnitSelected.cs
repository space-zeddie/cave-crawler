using System.Collections.Generic;
using System.Linq;

class CellGridStateUnitSelected : CellGridState
{
    private bool network;

    private Unit _unit;
    private List<Cell> _pathsInRange;
    private List<Unit> _unitsInRange;

    private Cell _unitCell;

    private UnitNet _unitNet;
    private List<CellNet> _pathsInRangeNet;
    private List<UnitNet> _unitsInRangeNet;

    private CellNet _unitCellNet;

    public CellGridStateUnitSelected(CellGrid cellGrid, Unit unit) : base(cellGrid)
    {
        _unit = unit;
        _pathsInRange = new List<Cell>();
        _unitsInRange = new List<Unit>();
        network = false;
    }
    public CellGridStateUnitSelected(CellGridNet cellGrid, UnitNet unit) : base(cellGrid)
    {
        _unitNet = unit;
        _pathsInRangeNet = new List<CellNet>();
        _unitsInRangeNet = new List<UnitNet>();
        network = true;
    }

    public override void OnCellClicked(Cell cell)
    {
        if (_unit.isMoving)
            return;
        if(cell.IsTaken)
        {
            _cellGrid.CellGridState = new CellGridStateWaitingForInput(_cellGrid);
            return;
        }
            
        if(!_pathsInRange.Contains(cell))
        {
            _cellGrid.CellGridState = new CellGridStateWaitingForInput(_cellGrid);
        }
        else
        {
            var path = _unit.FindPath(_cellGrid.Cells, cell);
            _unit.Move(cell,path);
            _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, _unit);
        }
    }
    public override void OnCellClicked(CellNet cell)
    {
        if (_unitNet.isMoving)
            return;
        if (cell.IsTaken)
        {
            _cellGridNet.CellGridState = new CellGridStateWaitingForInput(_cellGridNet);
            return;
        }

        if (!_pathsInRangeNet.Contains(cell))
        {
            _cellGrid.CellGridState = new CellGridStateWaitingForInput(_cellGrid);
        }
        else
        {
            var path = _unitNet.FindPath(_cellGridNet.Cells, cell);
            _unitNet.Move(cell, path);
            _cellGridNet.CellGridState = new CellGridStateUnitSelected(_cellGridNet, _unitNet);
        }
    }
    public override void OnUnitClicked(Unit unit)
    {
        if (unit.Equals(_unit) || unit.isMoving)
            return;

        if (_unitsInRange.Contains(unit) && _unit.ActionPoints > 0)
        {
            _unit.DealDamage(unit);
            _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, _unit);
        }

        if (unit.PlayerNumber.Equals(_unit.PlayerNumber))
        {
            _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, unit);
        }
            
    }
    public override void OnUnitClicked(UnitNet unit)
    {
        if (unit.Equals(_unitNet) || unit.isMoving)
            return;

        if (_unitsInRangeNet.Contains(unit) && _unitNet.ActionPoints > 0)
        {
            _unitNet.DealDamage(unit);
            _cellGridNet.CellGridState = new CellGridStateUnitSelected(_cellGridNet, _unitNet);
        }

        if (unit.PlayerNumber.Equals(_unitNet.PlayerNumber))
        {
            _cellGridNet.CellGridState = new CellGridStateUnitSelected(_cellGridNet, unit);
        }

    }
    public override void OnCellDeselected(CellNet cell)
    {
        base.OnCellDeselected(cell);

        foreach (var _cell in _pathsInRangeNet)
        {
            _cell.MarkAsReachable();
        }
        foreach (var _cell in _cellGridNet.Cells.Except(_pathsInRangeNet))
        {
            _cell.UnMark();
        }
    }
    public override void OnCellDeselected(Cell cell)
    {
        base.OnCellDeselected(cell);

        foreach (var _cell in _pathsInRange)
        {
            _cell.MarkAsReachable();
        }
        foreach (var _cell in _cellGrid.Cells.Except(_pathsInRange))
        {
            _cell.UnMark();
        }
    }
    public override void OnCellSelected(Cell cell)
    {
        base.OnCellSelected(cell);
        if (!_pathsInRange.Contains(cell)) return;
        var path = _unit.FindPath(_cellGrid.Cells, cell);
        foreach (var _cell in path)
        {
            _cell.MarkAsPath();
        }
    }
    public override void OnCellSelected(CellNet cell)
    {
        base.OnCellSelected(cell);
        if (!_pathsInRangeNet.Contains(cell)) return;
        var path = _unitNet.FindPath(_cellGridNet.Cells, cell);
        foreach (var _cell in path)
        {
            _cell.MarkAsPath();
        }
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        if (network) stateEnterNetwork();
        else stateEnter();

    }
    public override void OnStateExit()
    {
        if (network) stateEnterNetwork();
        else stateEnter();
    }

    void stateExit()
    {
        _unit.OnUnitDeselected();
        foreach (var unit in _unitsInRange)
        {
            if (unit == null) continue;
            unit.SetState(new UnitStateNormal(unit));
        }
        foreach (var cell in _cellGrid.Cells)
        {
            cell.UnMark();
        } 
    }
    void stateExitNetwork()
    {
        _unitNet.OnUnitDeselected();
        foreach (var unit in _unitsInRangeNet)
        {
            if (unit == null) continue;
            unit.SetState(new UnitStateNormal(unit));
        }
        foreach (var cell in _cellGridNet.Cells)
        {
            cell.UnMark();
        }
    }

    void stateEnter()
    {
        _unit.OnUnitSelected();
        _unitCell = _unit.Cell;

        _pathsInRange = _unit.GetAvailableDestinations(_cellGrid.Cells);
        var cellsNotInRange = _cellGrid.Cells.Except(_pathsInRange);

        foreach (var cell in cellsNotInRange)
        {
            cell.UnMark();
        }
        foreach (var cell in _pathsInRange)
        {
            cell.MarkAsReachable();
        }

        if (_unit.ActionPoints <= 0) return;

        foreach (var currentUnit in _cellGrid.Units)
        {
            if (currentUnit.PlayerNumber.Equals(_unit.PlayerNumber))
                continue;

            if (_unit.IsUnitAttackable(currentUnit, _unit.Cell))
            {
                currentUnit.SetState(new UnitStateMarkedAsReachableEnemy(currentUnit));
                _unitsInRange.Add(currentUnit);
            }
        }

        if (_unitCell.GetNeighbours(_cellGrid.Cells).FindAll(c => c.MovementCost <= _unit.MovementPoints).Count == 0
            && _unitsInRange.Count == 0)
            _unit.SetState(new UnitStateMarkedAsFinished(_unit));
    }

    void stateEnterNetwork()
    {
        _unitNet.OnUnitSelected();
        _unitCellNet = _unitNet.Cell;

        _pathsInRangeNet = _unitNet.GetAvailableDestinations(_cellGridNet.Cells);
        var cellsNotInRange = _cellGridNet.Cells.Except(_pathsInRangeNet);

        foreach (var cell in cellsNotInRange)
        {
            cell.UnMark();
        }
        foreach (var cell in _pathsInRangeNet)
        {
            cell.MarkAsReachable();
        }

        if (_unitNet.ActionPoints <= 0) return;

        foreach (var currentUnit in _cellGridNet.Units)
        {
            if (currentUnit.PlayerNumber.Equals(_unitNet.PlayerNumber))
                continue;

            if (_unitNet.IsUnitAttackable(currentUnit, _unitNet.Cell))
            {
                currentUnit.SetState(new UnitStateMarkedAsReachableEnemy(currentUnit));
                _unitsInRangeNet.Add(currentUnit);
            }
        }

        if (_unitCellNet.GetNeighbours(_cellGridNet.Cells).FindAll(c => c.MovementCost <= _unitNet.MovementPoints).Count == 0
            && _unitsInRangeNet.Count == 0)
            _unitNet.SetState(new UnitStateMarkedAsFinished(_unitNet));
  
    }
}

