public class UnitStateNormal : UnitState
{
    public UnitStateNormal(Unit unit) : base(unit)
    {       
    }
    public UnitStateNormal(UnitNet unit)
        : base(unit)
    {
    }

    public override void Apply()
    {
        if (_unit != null) _unit.UnMark();
        if (_unitNet != null) _unitNet.UnMark();
    }

    public override void MakeTransition(UnitState state)
    {
        state.Apply();
        if (_unit != null) _unit.UnitState = state;
        if (_unitNet != null) _unitNet.UnitState = state;
    }
}

