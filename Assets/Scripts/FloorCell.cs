using UnityEngine;
using System.Collections;

public class FloorCell : Hexagon 
{
    bool isReachable = false;
    public GameUnit moveable = null;

    void OnMouseDown()
    {
        if (moveable == null) return;
        moveable.Move(this, moveable.FindPath(moveable.GetAvailableCells(), this));
        moveable.SwitchHasMoved();
        moveable.UnSelect();
        this.UnReach();
    }

    protected override void OnMouseExit()
    {
        base.OnMouseExit();
        if (isReachable) MarkAsReachable();
    }

    public override Vector3 GetCellDimensions() 
    {
        return GetComponent<Renderer>().bounds.size; 
    }
    public override void MarkAsHighlighted()
    {
        GetComponent<Renderer>().material.color = new Color(0.75f, 0.75f, 0.75f); 
    }
    public override void MarkAsPath()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }
    public override void MarkAsReachable() 
    {
        if (!isReachable) isReachable = true;
        GetComponent<Renderer>().material.color = Color.yellow; 
    }
    public override void UnMark() 
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    public void UnReach()
    {
        if (isReachable) isReachable = false;
     //   base.OnMouseExit();
        UnMark();
        moveable = null;
    }
	
}
