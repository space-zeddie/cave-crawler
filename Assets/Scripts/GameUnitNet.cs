using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameUnitNet : UnitNet 
{
    static int totalUnits = 0;

    public Color LeadingColor;
    public bool canShoot;
    public int CellNumber; // the number of the cell as a child of HexGrid
    bool hasMoved;
    public bool isDeployed;
    bool isSelected;
    string id = "";

    List<CellNet> available;

    void Start()
    {
        InitID();
    }

    void OnClientStart()
    {
        if (!NetworkServer.active)
        {
            transform.SetParent(GameObject.FindObjectOfType<UnitParentScript>().transform);
            this.PlayerNumber = 1;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        ++totalUnits;
        available = new List<CellNet>();
        transform.position += new Vector3(0, 0, -1);
        GetComponent<Renderer>().material.color = LeadingColor;
        isDeployed = true;
        hasMoved = false;
        isSelected = false;
        if (id.Length == 0) InitID();
    }

    protected override IEnumerator MovementAnimation(List<CellNet> path)
    {
        /* NetworkGameUnit net = this.gameObject.GetComponent<NetworkGameUnit>();
         if (net != null)
             return net.CmdMove(path, this.MovementSpeed);
         else */
        return base.MovementAnimation(path);
    }

    void InitID()
    {
        id = this.GetType().ToString() + "," + this.TotalHitPoints + "," + this.AttackRange + "," + this.MovementPoints + "," + this.PlayerNumber;
    }

    /* public static GameUnit ParseGameUnitFromID(string id)
     {
         if (id.Length == 0 || id.IndexOf(',') == -1) return null;
         string[] tags = id.Split(',');
         if (tags.GetLength(0) == 0) return null;
         switch(tags[0])
         {
             case "Carrier"
         }
     }*/

    public override bool Equals(object o)
    {
        var t1 = this.GetType();
        var t2 = o.GetType();

        if (t1.IsAssignableFrom(t2) || t2.IsAssignableFrom(t1))
        {
            GameUnitNet gu = o as GameUnitNet;
            if (this.ID().Equals(gu.ID()))
                return true;
        }
        return false;
    }

    public override string ToString()
    {
        return base.ToString() + " #" + id;
    }

    public string ID()
    {
        return id;
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
        GameObject unit_go = UnitParentScript.Instance.SelectedUnit();
        if (unit_go != null) unit_go.GetComponent<GameUnitNet>().UnSelect();
        UnitParentScript.Instance.SetSelectedUnit(this);
        //this.gameObject.transform.parent.gameObject.GetComponent<UnitParentScript>().SetSelectedUnit(this);
        Debug.Log(available);
        if (available.Count == 0) PopulateAvailableCells();
        foreach (CellNet cell in available)
        {
            (cell as HexagonNet).MarkAsReachable();
            if (cell is FloorCellNet)
                (cell as FloorCellNet).moveable = this;
            else if (cell is WallCellNet && this.canShoot)
                (cell as WallCellNet).moveable = this;
        }
    }

    public void PopulateAvailableCells(CellNet cell = null, int limit = 0)
    {
        // if (limit == 0) Debug.Log("Populating available cells for " + this);
        // default values
        if (cell == null) cell = this.Cell;
        if (available == null) available = new List<CellNet>();

        if (limit == this.MovementPoints) return;
        available.AddRange((cell as HexagonNet).GetNeighbours(new List<CellNet>(cell.gameObject.transform.parent.GetComponentsInChildren<CellNet>())));
        List<CellNet> cells = new List<CellNet>(available);
        foreach (CellNet c in cells)
        {
            PopulateAvailableCells(c, limit + c.MovementCost);
        }
    }

    public List<CellNet> GetAvailableCells()
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
    public override void MarkAsDefending(UnitNet other)
    {
        GetComponent<Renderer>().material.color = LeadingColor + Color.yellow;
    }
    /// <summary>
    /// Gives visual indication that the unit is attacking.
    /// </summary>
    /// <param name="other"></param>
    public override void MarkAsAttacking(UnitNet other)
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
        UnitParentScript.Instance.ClearSelectedUnit();
        if (available.Count != 0)
        {
            foreach (CellNet cell in available)
            {
                if (cell is FloorCellNet) (cell as FloorCellNet).UnReach();
                else if (cell is WallCellNet) (cell as WallCellNet).UnReach();
            }

            available = new List<CellNet>();
        }
    }
}
