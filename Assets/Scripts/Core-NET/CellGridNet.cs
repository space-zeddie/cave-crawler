using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// CellGrid class keeps track of the game, stores cells, units and players objects. It starts the game and makes turn transitions. 
/// It reacts to user interacting with units or cells, and raises events related to game progress. 
/// </summary>
public class CellGridNet : NetworkBehaviour
{
    public event EventHandler GameStarted;
    public event EventHandler GameEnded;
    public event EventHandler TurnEnded;
    
    private CellGridState _cellGridState;//The grid delegates some of its behaviours to cellGridState object.

    public CellGridState CellGridState
    {
        private get
        {
            return _cellGridState;
        }
        set
        {
            if(_cellGridState != null)
                _cellGridState.OnStateExit();
            _cellGridState = value;
            _cellGridState.OnStateEnter();
        }
    }


    public int NumberOfPlayers { get; private set; }
    
    public PlayerNet CurrentPlayer
    {
        get { return Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
    }
    
    public int CurrentPlayerNumber { get; private set; }

    public Transform PlayersParent;

    
    public List<PlayerNet> Players { get; private set; }
    
    public List<CellNet> Cells { get; private set; }
    
    public List<UnitNet> Units { get; private set; }

    void Start()
    {
        CalculatePlayers();

        Cells = new List<CellNet>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var cell = transform.GetChild(i).gameObject.GetComponent<CellNet>();
            if (cell != null)
                Cells.Add(cell);
            else
                Debug.LogError("Invalid object in cells paretn game object");
        }
      
        foreach (var cell in Cells)
        {
            cell.CellClicked += OnCellClicked;
            cell.CellHighlighted += OnCellHighlighted;
            cell.CellDehighlighted += OnCellDehighlighted;
        }
             
        if (NetworkServer.active)
        {
            var unitGenerator = GetComponent<IUnitGeneratorNet>();
            if (unitGenerator != null)
            {
                Units = unitGenerator.SpawnUnits(Cells);
                foreach (var unit in Units)
                {
                    unit.UnitClicked += OnUnitClicked;
                    unit.UnitDestroyed += OnUnitDestroyed;
                }
            }
            else
                Debug.LogError("No IUnitGenerator script attached to cell grid");
            StartGame();
        }
    }

    public PlayerNet CalculatePlayers()
    {
        Players = new List<PlayerNet>();
        PlayerNet pl = null;
        for (int i = 0; i < PlayersParent.childCount; i++)
        {
            var player = PlayersParent.GetChild(i).GetComponent<PlayerNet>();
            if (player != null)
            {
                Players.Add(player);
                if (player.PlayerNumber == 1) pl = player;
            }
            
            else
                Debug.LogError("Invalid object in Players Parent game object");
        }
        NumberOfPlayers = Players.Count;
        CurrentPlayerNumber = Players.Min(p => p.PlayerNumber);
        return pl;
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player connected");
    }

    public override void OnStartClient()
    {
        Debug.Log("Started client");
        if (!NetworkServer.active)
        {
            Cells = new List<CellNet>();
            foreach (CellNet cell in GameObject.FindObjectsOfType<CellNet>())
                Cells.Add(cell);
           
        }
    }
   
    public void CreateUnits()
    {
        InitUnits();
        StartGame();
    }

    void InitUnits()
    {
        var unitGenerator = GetComponent<IUnitGeneratorNet>();
        HumanPlayerNet p = CalculatePlayers().GetComponent<HumanPlayerNet>();
        if (unitGenerator != null)
        {
            Debug.Log("call to init units");
            p.Cmd_SpawnUnits();
           // Units = unitGenerator.SpawnUnits(Cells);
            Units = new List<UnitNet>(GameObject.FindObjectOfType<UnitParentScript>().gameObject.transform.GetComponentsInChildren<UnitNet>());
            Debug.Log(Units.Count);
            for (int i = 0; i < Units.Count; ++i)
            {
                var unit = Units[i];
               // Debug.Log();
                unit.UnitClicked += OnUnitClicked;
                unit.UnitDestroyed += OnUnitDestroyed;
            }
        }
        else
            Debug.LogError("No IUnitGenerator script attached to cell grid");
    }


    private void OnCellDehighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellDeselected(sender as CellNet);
    }
    private void OnCellHighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellSelected(sender as CellNet);
    } 
    private void OnCellClicked(object sender, EventArgs e)
    {
        CellGridState.OnCellClicked(sender as CellNet);
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {
        CellGridState.OnUnitClicked(sender as UnitNet);
    }
    private void OnUnitDestroyed(object sender, AttackEventArgsNet e)
    {
        Units.Remove(sender as UnitNet);
        var totalPlayersAlive = Units.Select(u => u.PlayerNumber).Distinct().ToList(); //Checking if the game is over
        if (totalPlayersAlive.Count == 1)
        {
            if(GameEnded != null)
                GameEnded.Invoke(this, new EventArgs());
        }
    }
    
    /// <summary>
    /// Method is called once, at the beggining of the game.
    /// </summary>
    public void StartGame()
    {
        if(GameStarted != null)
            GameStarted.Invoke(this, new EventArgs());

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
    }

    public void EndGame()
    {
        if (GameEnded != null)
            GameEnded.Invoke(this, new EventArgs());
    }


    /// <summary>
    /// Method makes turn transitions. It is called by player at the end of his turn.
    /// </summary> 
    public void EndTurn()
    {
        if (Units.Select(u => u.PlayerNumber).Distinct().Count() == 0)
        {
            return;
        }
        CellGridState = new CellGridStateTurnChanging(this);

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnEnd(); });

        CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
        while (Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).Count == 0)
        {
            CurrentPlayerNumber = (CurrentPlayerNumber + 1)%NumberOfPlayers;
        }//Skipping players that are defeated.

        if (TurnEnded != null)
            TurnEnded.Invoke(this, new EventArgs());

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);     
    }
}
