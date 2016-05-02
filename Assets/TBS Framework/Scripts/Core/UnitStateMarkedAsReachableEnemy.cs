public class UnitStateMarkedAsReachableEnemy : UnitState
{
    public UnitStateMarkedAsReachableEnemy(Unit unit) : base(unit)
    {        
    }

    public UnitStateMarkedAsReachableEnemy(UnitNet unit)
        : base(unit)
    {
    }


    public override void Apply()
    {
        if (_unit != null) _unit.MarkAsReachableEnemy();
        if (_unitNet != null) _unitNet.MarkAsReachableEnemy();
    }

    public override void MakeTransition(UnitState state)
    {
        state.Apply();
        if (_unit != null) _unit.UnitState = state;
        if (_unitNet != null) _unitNet.UnitState = state;
    }
}

