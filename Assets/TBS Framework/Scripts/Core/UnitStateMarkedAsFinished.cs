public class UnitStateMarkedAsFinished : UnitState
{
    public UnitStateMarkedAsFinished(Unit unit) : base(unit)
    {      
    }
    public UnitStateMarkedAsFinished(UnitNet unit)
        : base(unit)
    {
    }

    public override void Apply()
    {
        if (_unit != null) _unit.MarkAsFinished();
        if (_unitNet != null) _unitNet.MarkAsFinished();
    }

    public override void MakeTransition(UnitState state)
    {
        if(state is UnitStateNormal)
        {
            state.Apply();
            if (_unit != null) _unit.UnitState = state;
            if (_unitNet != null) _unitNet.UnitState = state;
        }
    }
}

