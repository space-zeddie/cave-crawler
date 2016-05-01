﻿using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

/// <summary>
/// Base class for all units in the game.
/// </summary>
[Serializable]
public abstract class UnitNet : NetworkBehaviour
{
    /// <summary>
    /// UnitClicked event is invoked when user clicks the unit. It requires a collider on the unit game object to work.
    /// </summary>
    [SerializeField]
    public event EventHandler UnitClicked;
    /// <summary>
    /// UnitSelected event is invoked when user clicks on unit that belongs to him. It requires a collider on the unit game object to work.
    /// </summary>
    [SerializeField]
    public event EventHandler UnitSelected;
    [SerializeField]
    public event EventHandler UnitDeselected;
    /// <summary>
    /// UnitHighlighted event is invoked when user moves cursor over the unit. It requires a collider on the unit game object to work.
    /// </summary>
    [SerializeField]
    public event EventHandler UnitHighlighted;
    [SerializeField]
    public event EventHandler UnitDehighlighted;
    [SerializeField]
    public event EventHandler<AttackEventArgsNet> UnitAttacked;
    [SerializeField]
    public event EventHandler<AttackEventArgsNet> UnitDestroyed;
    [SerializeField]
    public event EventHandler<MovementEventArgsNet> UnitMoved;

    [SerializeField]
    public UnitState UnitState { get; set; }

    public void SetState(UnitState state)
    {
        UnitState.MakeTransition(state);
    }

    [SerializeField]
    public List<Buff> Buffs { get; private set; }

    [SerializeField]
    public int TotalHitPoints { get; private set; }
    [SerializeField]
    [SyncVar]
    protected int TotalMovementPoints;
    [SerializeField]
    [SyncVar]
    protected int TotalActionPoints;

    /// <summary>
    /// Cell that the unit is currently occupying.
    /// </summary>
    [SerializeField]
    public CellNet Cell { get; set; }

    [SerializeField]
    [SyncVar]
    public int HitPoints;
    [SerializeField]
    [SyncVar]
    public int AttackRange;
    [SerializeField]
    [SyncVar]
    public int AttackFactor;
    [SerializeField]
    [SyncVar]
    public int DefenceFactor;
    /// <summary>
    /// Determines how far on the grid the unit can move.
    /// </summary>
    [SerializeField]
    [SyncVar]
    public int MovementPoints;
    /// <summary>
    /// Determines speed of movement animation.
    /// </summary>
   [SerializeField]
   [SyncVar]
    public float MovementSpeed;
    /// <summary>
    /// Determines how many attacks unit can perform in one turn.
    /// </summary>
    [SerializeField]
    [SyncVar]
    public int ActionPoints;

    /// <summary>
    /// Indicates the player that the unit belongs to. Should correspoond with PlayerNumber variable on Player script.
    /// </summary>
    [SerializeField]
    [SyncVar]
    public int PlayerNumber;

    /// <summary>
    /// Indicates if movement animation is playing.
    /// </summary>
   [SerializeField]
    public bool isMoving { get; set; }

    private static IPathfinding _pathfinder = new AStarPathfinding();

    /// <summary>
    /// Method called after object instantiation to initialize fields etc. 
    /// </summary>
    public virtual void Initialize()
    {
        Buffs = new List<Buff>();

        UnitState = new UnitStateNormal(this);

        TotalHitPoints = HitPoints;
        TotalMovementPoints = MovementPoints;
        TotalActionPoints = ActionPoints;
    }

    protected virtual void OnMouseDown()
    {
        if (UnitClicked != null)
            UnitClicked.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseEnter()
    {
        if (UnitHighlighted != null)
            UnitHighlighted.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseExit()
    {
        if (UnitDehighlighted != null)
            UnitDehighlighted.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Method is called at the start of each turn.
    /// </summary>
    public virtual void OnTurnStart()
    {
        MovementPoints = TotalMovementPoints;
        ActionPoints = TotalActionPoints;

        SetState(new UnitStateMarkedAsFriendly(this));
    }
    /// <summary>
    /// Method is called at the end of each turn.
    /// </summary>
    public virtual void OnTurnEnd()
    {
        Buffs.FindAll(b => b.Duration == 0).ForEach(b => { b.Undo(this); });
        Buffs.RemoveAll(b => b.Duration == 0);
        Buffs.ForEach(b => { b.Duration--; });

        SetState(new UnitStateNormal(this));
    }
    /// <summary>
    /// Method is called when units HP drops below 1.
    /// </summary>
    protected virtual void OnDestroyed()
    {
        Cell.IsTaken = false;
        MarkAsDestroyed();
        Destroy(gameObject);
    }

    /// <summary>
    /// Method is called when unit is selected.
    /// </summary>
    public virtual void OnUnitSelected()
    {
        SetState(new UnitStateMarkedAsSelected(this));
        if (UnitSelected != null)
            UnitSelected.Invoke(this, new EventArgs());
    }
    /// <summary>
    /// Method is called when unit is deselected.
    /// </summary>
    public virtual void OnUnitDeselected()
    {
        SetState(new UnitStateMarkedAsFriendly(this));
        if (UnitDeselected != null)
            UnitDeselected.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Method indicates if it is possible to attack unit given as parameter, from cell given as second parameter.
    /// </summary>
    public virtual bool IsUnitAttackable(UnitNet other, CellNet sourceCell)
    {
        if (sourceCell.GetDistance(other.Cell) <= AttackRange)
            return true;

        return false;
    }
    /// <summary>
    /// Method deals damage to unit given as parameter.
    /// </summary>
    public virtual void DealDamage(UnitNet other)
    {
        if (isMoving)
            return;
        if (ActionPoints == 0)
            return;
        if (!IsUnitAttackable(other, Cell))
            return;

        MarkAsAttacking(other);
        ActionPoints--;
        other.Defend(this, AttackFactor);

        if (ActionPoints == 0)
        {
            SetState(new UnitStateMarkedAsFinished(this));
            MovementPoints = 0;
        }  
    }
    /// <summary>
    /// Attacking unit calls Defend method on defending unit. 
    /// </summary>
    protected virtual void Defend(UnitNet other, int damage)
    {
        MarkAsDefending(other);
        HitPoints -= Mathf.Clamp(damage - DefenceFactor, 1, damage);  //Damage is calculated by subtracting attack factor of attacker and defence factor of defender. If result is below 1, it is set to 1.
                                                                      //This behaviour can be overridden in derived classes.
        if (UnitAttacked != null)
            UnitAttacked.Invoke(this, new AttackEventArgsNet(other, this, damage));

        if (HitPoints <= 0)
        {
            if (UnitDestroyed != null)
                UnitDestroyed.Invoke(this, new AttackEventArgsNet(other, this, damage));
            OnDestroyed();
        }
    }

    public virtual void Move(CellNet destinationCell, List<CellNet> path)
    {
        if (isMoving)
            return;

        var totalMovementCost = path.Sum(h => h.MovementCost);
        if (MovementPoints < totalMovementCost)
            return;

        MovementPoints -= totalMovementCost;

        Cell.IsTaken = false;
        Cell = destinationCell;
        destinationCell.IsTaken = true;

        if (MovementSpeed > 0)
            StartCoroutine(MovementAnimation(path));
        else
            transform.position = Cell.transform.position;

        if (UnitMoved != null)
            UnitMoved.Invoke(this, new MovementEventArgsNet(Cell, destinationCell, path));    
    }
    protected virtual IEnumerator MovementAnimation(List<CellNet> path)
    {
        isMoving = true;

        path.Reverse();
        foreach (var cell in path)
        {
            while (new Vector2(transform.position.x,transform.position.y) != new Vector2(cell.transform.position.x,cell.transform.position.y))
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(cell.transform.position.x,cell.transform.position.y,transform.position.z), Time.deltaTime * MovementSpeed);
                yield return 0;
            }
        }

        isMoving = false;
    }

    ///<summary>
    /// Method indicates if unit is capable of moving to cell given as parameter.
    /// </summary>
    public virtual bool IsCellMovableTo(CellNet cell)
    {
        return !cell.IsTaken;
    }
    /// <summary>
    /// Method indicates if unit is capable of moving through cell given as parameter.
    /// </summary>
    public virtual bool IsCellTraversable(CellNet cell)
    {
        return !cell.IsTaken;
    }
    /// <summary>
    /// Method returns all cells that the unit is capable of moving to.
    /// </summary>
    public List<CellNet> GetAvailableDestinations(List<CellNet> cells)
    {
        var ret = new List<CellNet>();
        var cellsInMovementRange = cells.FindAll(c => IsCellMovableTo(c) && c.GetDistance(Cell) <= MovementPoints);

        var traversableCells = cells.FindAll(c => IsCellTraversable(c) && c.GetDistance(Cell) <= MovementPoints);
        traversableCells.Add(Cell);

        foreach (var cellInRange in cellsInMovementRange)
        {
            if (cellInRange.Equals(Cell)) continue;

            var path = FindPath(traversableCells, cellInRange);
            var pathCost = path.Sum(c => c.MovementCost);
            if (pathCost > 0 && pathCost <= MovementPoints)
                ret.AddRange(path);
        }
        return ret.FindAll(IsCellMovableTo).Distinct().ToList();
    }

    public List<CellNet> FindPath(List<CellNet> cells, CellNet destination)
    {
        return _pathfinder.FindPath(GetGraphEdges(cells), this.Cell, destination);
    }
    /// <summary>
    /// Method returns graph representation of cell grid for pathfinding.
    /// </summary>
    protected virtual Dictionary<CellNet, Dictionary<CellNet, int>> GetGraphEdges(List<CellNet> cells)
    {
        Dictionary<CellNet, Dictionary<CellNet, int>> ret = new Dictionary<CellNet, Dictionary<CellNet, int>>();
        foreach (var cell in cells)
        {
            if (IsCellTraversable(cell) || cell.Equals(Cell))
            {
                ret[cell] = new Dictionary<CellNet, int>();
                foreach (var neighbour in cell.GetNeighbours(cells).FindAll(IsCellTraversable))
                {
                    ret[cell][neighbour] = neighbour.MovementCost;
                }
            }
        }
        return ret;
    }

    /// <summary>
    /// Gives visual indication that the unit is under attack.
    /// </summary>
    /// <param name="other"></param>
    public abstract void MarkAsDefending(UnitNet other);
    /// <summary>
    /// Gives visual indication that the unit is attacking.
    /// </summary>
    /// <param name="other"></param>
    public abstract void MarkAsAttacking(UnitNet other);
    /// <summary>
    /// Gives visual indication that the unit is destroyed. It gets called right before the unit game object is
    /// destroyed, so either instantiate some new object to indicate destruction or redesign Defend method. 
    /// </summary>
    public abstract void MarkAsDestroyed();

    /// <summary>
    /// Method marks unit as current players unit.
    /// </summary>
    public abstract void MarkAsFriendly();
    /// <summary>
    /// Method mark units to indicate user that the unit is in range and can be attacked.
    /// </summary>
    public abstract void MarkAsReachableEnemy();
    /// <summary>
    /// Method marks unit as currently selected, to distinguish it from other units.
    /// </summary>
    public abstract void MarkAsSelected();
    /// <summary>
    /// Method marks unit to indicate user that he can't do anything more with it this turn.
    /// </summary>
    public abstract void MarkAsFinished();
    /// <summary>
    /// Method returns the unit to its base appearance
    /// </summary>
    public abstract void UnMark();
}

[Serializable]
public class MovementEventArgsNet : EventArgs
{
    public CellNet OriginCell;
    public CellNet DestinationCell;
    public List<CellNet> Path;

    public MovementEventArgsNet(CellNet sourceCell, CellNet destinationCell, List<CellNet> path)
    {
        OriginCell = sourceCell;
        DestinationCell = destinationCell;
        Path = path;
    }
}
[Serializable]
public class AttackEventArgsNet : EventArgs
{
    public UnitNet Attacker;
    public UnitNet Defender;

    public int Damage;

    public AttackEventArgsNet(UnitNet attacker, UnitNet defender, int damage)
    {
        Attacker = attacker;
        Defender = defender;

        Damage = damage;
    }
}