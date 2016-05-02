public class UnitStateMarkedAsFriendly : UnitState
{
    public UnitStateMarkedAsFriendly(Unit unit) : base(unit)
    {
        
    }

    public UnitStateMarkedAsFriendly(UnitNet unit)
        : base(unit)
    {

    }


    public override void Apply()
    {
        if (_unit != null) _unit.MarkAsFriendly();
        if (_unitNet != null) _unitNet.MarkAsFriendly();
    }

    public override void MakeTransition(UnitState state)
    {
        state.Apply();
        if (_unit != null) _unit.UnitState = state;
        if (_unitNet != null) _unitNet.UnitState = state;
    }
}

