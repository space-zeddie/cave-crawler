using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GUIControllerScript : Singleton<GUIControllerScript> 
{
	public CellGrid CellGrid;
    public GameObject UnitsParent;
    public GameObject PlayersParent;
    public GameObject ObstaclesParent;
    public Camera CarrierCamera;
    public Camera OverheadCamera;
    public GameObject RuntimeInstantiated;

    //public Button NextTurnButton;

   // public Image UnitImage;
    //public Text InfoText;
    //public Text StatsText;

    void Start()
    {
        CellGrid.GameStarted += OnGameStarted;
        CellGrid.TurnEnded += OnTurnEnded;
        CellGrid.GameEnded += OnGameEnded;

        CarrierCamera.enabled = true;
        OverheadCamera.enabled = false;
    }

    private void OnGameStarted(object sender, EventArgs e)
    {
        foreach (Transform unit in UnitsParent.transform)
        {
            unit.GetComponent<Unit>().UnitHighlighted += OnUnitHighlighted;
            unit.GetComponent<Unit>().UnitDehighlighted += OnUnitDehighlighted;
            unit.GetComponent<Unit>().UnitDestroyed += OnUnitDestroyed;
            unit.GetComponent<Unit>().UnitAttacked += OnUnitAttacked;
        }
    }

    private void OnTurnEnded(object sender, EventArgs e)
    {
        //NextTurnButton.interactable = ((sender as CellGrid).CurrentPlayer is HumanPlayer);
        GameUnit[] units = UnitsParent.gameObject.transform.GetComponentsInChildren<GameUnit>();
        foreach (GameUnit unit in units)
        {
            unit.UnSelect();
            unit.SetHasMoved(false);
        }
        Debug.Log("Turn Ended");
    }
    private void OnGameEnded(object sender, EventArgs e)
    {
      //  if (PlayersParent.gameObject.transform.GetChildCount() > 1) 
        //    InfoText.text = "Cave completed";
        //else
          //  InfoText.text = "Cave completed. Player " + ((sender as CellGrid).CurrentPlayerNumber + 1) + " wins!";
       /* _gameOverPanel = Instantiate(GameOverPanel);
        _gameOverPanel.transform.Find("InfoText").GetComponent<Text>().text = "Player " + ((sender as CellGrid).CurrentPlayerNumber + 1) + "\nwins!";

        _gameOverPanel.transform.Find("DismissButton").GetComponent<Button>().onClick.AddListener(DismissPanel);

        _gameOverPanel.GetComponent<RectTransform>().SetParent(Canvas.GetComponent<RectTransform>(), false);*/
      //  Debug.Log("Game Ended");
        RuntimeInstantiated.gameObject.transform.GetComponentInChildren<GameManager>().LoadGameEndedScreen();
        
    }

    private void OnUnitAttacked(object sender, AttackEventArgs e)
    {
        if (!(CellGrid.CurrentPlayer is HumanPlayer)) return;

        OnUnitDehighlighted(sender, e);

        if ((sender as Unit).HitPoints <= 0) return;

        OnUnitHighlighted(sender, e);
    }
    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        Debug.Log("Unit Destroyed");
      //  Destroy(_infoPanel);
    }
    private void OnUnitDehighlighted(object sender, EventArgs e)
    {
      //  Destroy(_infoPanel);
    }
    private void OnUnitHighlighted(object sender, EventArgs e)
    {
       // Debug.Log("Unit Highlighted");
        var unit = sender as GameUnit;
       /* _infoPanel = Instantiate(InfoPanel);

        float hpScale = (float)((float)(unit).HitPoints / (float)(unit).TotalHitPoints);

        _infoPanel.transform.Find("Name").GetComponent<Text>().text = unit.UnitName;
        _infoPanel.transform.Find("HitPoints").Find("Image").transform.localScale = new Vector3(hpScale, 1, 1);
        _infoPanel.transform.Find("Attack").Find("Image").transform.localScale = new Vector3((float)unit.AttackFactor / 10.0f, 1, 1);
        _infoPanel.transform.Find("Defence").Find("Image").transform.localScale = new Vector3((float)unit.DefenceFactor / 10.0f, 1, 1);

        _infoPanel.GetComponent<RectTransform>().SetParent(Canvas.GetComponent<RectTransform>(), false);*/
    }

    public void DismissPanel()
    {
       /// Destroy(_gameOverPanel);
    }
    
    public void RestartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    
    // checks if all units have moved
    // and if a player's carrier was destroyed
    void CheckForEndingConditions()
    {
        //Debug.Log("Check for ending condition");
        GameUnit[] gameUnits = UnitsParent.GetComponentsInChildren<GameUnit>();
        bool allUnitsMoved = true;
        bool exitReached = true;
        Carrier carrier = null;
        foreach (GameUnit gu in gameUnits)
        {
            if (gu.enabled) exitReached = false;
            if (gu is Carrier)
                carrier = gu as Carrier;
            if (!gu.HasMoved())
            {
                allUnitsMoved = false;
                //break;
            }
        }
        if (allUnitsMoved) CellGrid.EndTurn();
        if (carrier == null)
            Destroy(PlayersParent.gameObject.transform.GetChild(carrier.PlayerNumber));

        if (exitReached)
            CellGrid.EndGame();
    }

    void Update () 
    {
        // switching cameras
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SwitchCameras();
            return;
        }

        // ending turn
        if(Input.GetKeyDown(KeyCode.N))
            CellGrid.EndTurn(); //User ends his turn by pressing "n" on keyboard.
        CheckForEndingConditions();       
    }

   public void SwitchCameras()
   {
       CarrierCamera.enabled = !CarrierCamera.enabled;
       OverheadCamera.enabled = !OverheadCamera.enabled;
   }
}
