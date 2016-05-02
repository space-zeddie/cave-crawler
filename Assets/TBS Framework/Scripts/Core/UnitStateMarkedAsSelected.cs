public class UnitStateMarkedAsSelected : UnitState
{
    public UnitStateMarkedAsSelected(Unit unit) : base(unit)
    {      
    }
    public UnitStateMarkedAsSelected(UnitNet unit)
        : base(unit)
    {
    }


    public override void Apply()
    {
        if (_unit != null) _unit.MarkAsSelected();
        if (_unitNet != null) _unitNet.MarkAsSelected();
    }

    public override void MakeTransition(UnitState state)
    {
        state.Apply();
        if (_unit != null) _unit.UnitState = state;
        if (_unitNet != null) _unitNet.UnitState = state;
    }
}

