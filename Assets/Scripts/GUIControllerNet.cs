using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class GUIControllerNet : Singleton<GUIControllerNet>
{
    public CellGridNet CellGrid;
    public GameObject UnitsParent;
    public GameObject PlayersParent;
    public GameObject ObstaclesParent;
    public Camera CarrierCamera;    
    public Camera CarrierCamera1;
    public Camera OverheadCamera;
    public GameObject RuntimeInstantiated;

    bool init = false;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Init();
        }

    }

    public void Init()
    {
        Debug.Log("GUI CONTROLLER STARTED ON: " + CellGrid);
        CellGrid.GameStarted += OnGameStarted;
        CellGrid.TurnEnded += OnTurnEnded;
        CellGrid.GameEnded += OnGameEnded;

        CarrierCamera1.enabled = true;
        OverheadCamera.enabled = false;

        init = true;
    }

    private void OnGameStarted(object sender, EventArgs e)
    {
        foreach (Transform unit in UnitsParent.transform)
        {
            unit.GetComponent<UnitNet>().UnitHighlighted += OnUnitHighlighted;
            unit.GetComponent<UnitNet>().UnitDehighlighted += OnUnitDehighlighted;
            unit.GetComponent<UnitNet>().UnitDestroyed += OnUnitDestroyed;
            unit.GetComponent<UnitNet>().UnitAttacked += OnUnitAttacked;
        }
    }

    private void OnTurnEnded(object sender, EventArgs e)
    {
        GameUnitNet[] units = UnitsParent.gameObject.transform.GetComponentsInChildren<GameUnitNet>();
        foreach (GameUnitNet unit in units)
        {
            unit.UnSelect();
            unit.SetHasMoved(false);
        }
        Debug.Log("Turn Ended");
    }
    private void OnGameEnded(object sender, EventArgs e)
    {
        GameObject.FindObjectOfType<HexGCANetwork>().ClearGrid();
        StatManager.Instance.IsNewCave = true;
        GameManager.Instance.LoadGameEndedScreen();

    }

    private void OnUnitAttacked(object sender, AttackEventArgsNet e)
    {
        if (!(CellGrid.CurrentPlayer is HumanPlayerNet)) return;

        OnUnitDehighlighted(sender, e);

        if ((sender as UnitNet).HitPoints <= 0) return;

        OnUnitHighlighted(sender, e);
    }
    private void OnUnitDestroyed(object sender, AttackEventArgsNet e)
    {
        Debug.Log("Unit Destroyed");
    }
    private void OnUnitDehighlighted(object sender, EventArgs e)
    {
    }
    private void OnUnitHighlighted(object sender, EventArgs e)
    {
        // Debug.Log("Unit Highlighted");
        var unit = sender as GameUnitNet;
        /* _infoPanel = Instantiate(InfoPanel);

         float hpScale = (float)((float)(unit).HitPoints / (float)(unit).TotalHitPoints);

         _infoPanel.transform.Find("Name").GetComponent<Text>().text = unit.UnitName;
         _infoPanel.transform.Find("HitPoints").Find("Image").transform.localScale = new Vector3(hpScale, 1, 1);
         _infoPanel.transform.Find("Attack").Find("Image").transform.localScale = new Vector3((float)unit.AttackFactor / 10.0f, 1, 1);
         _infoPanel.transform.Find("Defence").Find("Image").transform.localScale = new Vector3((float)unit.DefenceFactor / 10.0f, 1, 1);

         _infoPanel.GetComponent<RectTransform>().SetParent(Canvas.GetComponent<RectTransform>(), false);*/
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
        GameUnitNet[] gameUnits = UnitsParent.GetComponentsInChildren<GameUnitNet>();
        bool allUnitsMoved = true;
        bool exitReached = true;
        CarrierNet carrier = null;
        foreach (GameUnitNet gu in gameUnits)
        {
            if (gu.enabled) exitReached = false;
            if (gu is CarrierNet)
                carrier = gu as CarrierNet;
            if (!gu.HasMoved())
            {
                allUnitsMoved = false;
                //break;
            }
        }
        if (allUnitsMoved) CellGrid.EndTurn();
        /*if (carrier == null)
            Destroy(PlayersParent.gameObject.transform.GetChild(carrier.PlayerNumber));*/

        if (exitReached)
            CellGrid.EndGame();
    }

    public void SaveGame()
    {
        StatManager.Instance.IsNewCave = false;
        ObstacleGeneratorNet.Instance.SaveSpawns();
        PlayersParent.GetComponentInChildren<HumanPlayerNet>().SaveStats();
        GameManager.Instance.SaveGame();
    }

    public void LoadGame()
    {
        //GameManager.Instance.LoadGame();
        StatManager.Instance.LoadData();
    }

    void Update()
    {
        if (init)
        {
            // switching cameras
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SwitchCameras();
                return;
            }
            if (CellGrid != null)
            {
                // ending turn
                if (Input.GetKeyDown(KeyCode.N))
                    CellGrid.EndTurn(); //User ends his turn by pressing "n" on keyboard.
                CheckForEndingConditions();
            }
        }
    }

    public void SwitchCameras()
    {
        if (CarrierCamera.enabled)
        {
            CarrierCamera.enabled = false;
            OverheadCamera.enabled = false;
            CarrierCamera1.enabled = true;
        }
        else if (CarrierCamera1.enabled)
        {
            CarrierCamera.enabled = false;
            CarrierCamera1.enabled = false;
            OverheadCamera.enabled = true;        
        }
        else
        {
            CarrierCamera.enabled = true;
            OverheadCamera.enabled = false;
            CarrierCamera1.enabled = false;
        }
    }
}
