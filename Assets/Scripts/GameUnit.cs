using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameUnit : Unit
{
    public Color LeadingColor;
    public bool canShoot;
    bool hasMoved;
    public bool isDeployed;
    List<Cell> available;
    bool isSelected;

    public override void Initialize()
    {
        base.Initialize();
        available = new List<Cell>();
        transform.position += new Vector3(0, 0, -1);
        GetComponent<Renderer>().material.color = LeadingColor;
        isDeployed = true;
        hasMoved = false;
        isSelected = false;
    }

    public bool HasMoved()
    {
        return hasMoved;
    }

    public void SetHasMoved(bool val)
    {
        hasMoved = val;
    }

    void OnMouseDown()
    {
        // simply blocks any other player; should be removed if
        // hotseat on the same device is needed
        if (this.PlayerNumber != 0) return;

        if (!this.isSelected) this.MarkAsSelected();
        else
        {
            this.UnSelect();
            return;
        }
        GameUnit unit = this.gameObject.transform.parent.gameObject.GetComponent<UnitParentScript>().SelectedUnit();
        if (unit != null) unit.UnSelect();
        this.gameObject.transform.parent.gameObject.GetComponent<UnitParentScript>().SetSelectedUnit(this);
        PopulateAvailableCells();
        foreach (Cell cell in available)
        {
            (cell as Hexagon).MarkAsReachable();
            if (cell is FloorCell)
                (cell as FloorCell).moveable = this;
            else if (cell is WallCell && this.canShoot)
                (cell as WallCell).moveable = this;
        }
    }

    void PopulateAvailableCells(Cell cell = null, int limit = 0)
    {
      //  if (available.Count != 0) return;

        // default values
        if (cell == null) cell = this.Cell;
        if (available == null) available = new List<Cell>();

        if (limit == this.MovementPoints) return;
        available.AddRange((cell as Hexagon).GetNeighbours(new List<Cell>(cell.gameObject.transform.parent.GetComponentsInChildren<Cell>())));
        List<Cell> cells = new List<Cell>(available);
      //  Debug.Log(available.Count + ", limit: " + limit);
        foreach (Cell c in cells)
        {
           // Debug.Log("In cycle");
            PopulateAvailableCells(c, limit + c.MovementCost);
        }
    }

    public List<Cell> GetAvailableCells()
    {
        return available;
    }

    public override void MarkAsFriendly()
    {
        GetComponent<Renderer>().material.color = LeadingColor + new Color(0.8f, 1, 0.8f);
    }

    public override void MarkAsReachableEnemy()
    {
        GetComponent<Renderer>().material.color = LeadingColor + Color.red;
    }

    public override void MarkAsSelected()
    {
        isSelected = true;
        GetComponent<Renderer>().material.color = LeadingColor + Color.green;
    }


    /// <summary>
    /// Gives visual indication that the unit is under attack.
    /// </summary>
    /// <param name="other"></param>
    public override void MarkAsDefending(Unit other)
    {
        GetComponent<Renderer>().material.color = LeadingColor + Color.yellow;
    }
    /// <summary>
    /// Gives visual indication that the unit is attacking.
    /// </summary>
    /// <param name="other"></param>
    public override void MarkAsAttacking(Unit other)
    {
        GetComponent<Renderer>().material.color = LeadingColor + Color.white;
    }
    /// <summary>
    /// Gives visual indication that the unit is destroyed. It gets called right before the unit game object is
    /// destroyed, so either instantiate some new object to indicate destruction or redesign Defend method. 
    /// </summary>
    public override void MarkAsDestroyed()
    {
        GetComponent<Renderer>().material.color = LeadingColor + Color.grey;
    }
    /// <summary>
    /// Method marks unit to indicate user that he can't do anything more with it this turn.
    /// </summary>
    public override void MarkAsFinished()
    {
        GetComponent<Renderer>().material.color = LeadingColor + Color.blue;
    }
    /// <summary>
    /// Method returns the unit to its base appearance
    /// </summary>
    public override void UnMark()
    {
        UnSelect();
        GetComponent<Renderer>().material.color = LeadingColor;
    }

    public void UnSelect()
    {
        Debug.Log("Unselected");
        if (isSelected) isSelected = false;
        this.gameObject.transform.parent.GetComponent<UnitParentScript>().ClearSelectedUnit();
        if (available.Count != 0)
        {
            foreach (Cell cell in available)
            {
                if (cell is FloorCell) (cell as FloorCell).UnReach();
                else if (cell is WallCell) (cell as WallCell).UnReach();
            }

            available = new List<Cell>();
        }
    }
}
