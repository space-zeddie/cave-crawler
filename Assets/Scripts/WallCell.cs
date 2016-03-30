using UnityEngine;
using System.Collections;

public class WallCell : Hexagon
{
    public GameUnit moveable = null;
    bool isReachable = false;

    void OnMouseDown()
    {
        if (moveable == null) { return; }
        if (!moveable.canShoot || moveable.Cell.GetDistance(this as Cell) > moveable.AttackRange || moveable.HitPoints <= 0) return;
        moveable.SetHasMoved(true);
        isReachable = true;
        moveable.UnSelect();
        this.transform.parent.gameObject.GetComponent<HexGridCellularAutomata>().DemolishWallOnCell(this);
    }

    public override Vector3 GetCellDimensions()
    {
        return GetComponent<Renderer>().bounds.size;
    }
    public override void MarkAsHighlighted()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }
    public override void MarkAsPath()
    {
        GetComponent<Renderer>().material.color = Color.clear;
    }
    public override void MarkAsReachable()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }
    public override void UnMark()
    {
       // if (isReachable) isReachable = false;
        GetComponent<Renderer>().material.color = Color.black;
    }

    public void UnReach()
    {
        //if (isReachable) isReachable = false;
        moveable = null;
        UnMark();
    }
		
}
