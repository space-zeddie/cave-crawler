class CellGridStateWaitingForInput : CellGridState
{
    public CellGridStateWaitingForInput(CellGrid cellGrid) : base(cellGrid)
    {
    }
    public CellGridStateWaitingForInput(CellGridNet cellGrid)
        : base(cellGrid)
    {
    }

    public override void OnUnitClicked(Unit unit)
    {
        if(unit.PlayerNumber.Equals(_cellGrid.CurrentPlayerNumber))
            _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, unit); 
    }
    public override void OnUnitClicked(UnitNet unit)
    {
        if (unit.PlayerNumber.Equals(_cellGridNet.CurrentPlayerNumber))
            _cellGridNet.CellGridState = new CellGridStateUnitSelected(_cellGridNet, unit);
    }
}
