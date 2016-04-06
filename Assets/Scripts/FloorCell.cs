using UnityEngine;
using System.Collections;

public class FloorCell : Hexagon 
{
    bool isReachable = false;
    public GameUnit moveable = null;
    public Collectable spawn = null;

    void OnMouseDown()
    {
        //Debug.Log("Moving to cell");
        if (moveable == null) return;
        if (spawn != null)
        {
            if (!(spawn is Exit)) { Destroy(spawn); }
            IsTaken = false;
        }
        int priorMovementPoints = moveable.MovementPoints;
        if (!(spawn is Exit)) moveable.Move(this, moveable.FindPath(moveable.GetAvailableCells(), this));
        int deltaPoints = priorMovementPoints - moveable.MovementPoints;
        PlayersParent.Instance.GetComponentInChildren<HumanPlayer>().Score -= deltaPoints;

        moveable.SetHasMoved(true);

        if (spawn is Exit)
        {
            UnitParentScript.Instance.GetComponentInChildren<HumanPlayer>().Score += Exit.BUFF_POINTS;
            (spawn as Exit).VanishUnit(moveable);
        }
        moveable.UnSelect();
        this.UnReach();
        //Debug.Log("done moving to cell");
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
