using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Loader : Singleton<Loader>
{
    public GameObject InstatiatedObjects;
    public GameManager GMPrefab;
    public Canvas canvas;

    void Awake()
    {
        if (InstatiatedObjects != null)
        {
            if (GameManager.instance == null)
            {
                GameManager gm = Instantiate(GMPrefab);
                gm.transform.parent = InstatiatedObjects.transform;
                AssignButtonHandlers();
            }
        }
        else Debug.LogError("The scene must contain one designated game object for storing runtime-generated objects");
    }

    void AssignButtonHandlers()
    {
        SingleplayerButton sb = canvas.gameObject.transform.GetComponentInChildren<SingleplayerButton>();
        MultiplayerButton mb = canvas.gameObject.transform.GetComponentInChildren<MultiplayerButton>();
        ReturnToMenuButton rmb = canvas.gameObject.transform.GetComponentInChildren<ReturnToMenuButton>();
        if (sb != null)
            (sb as Button).onClick.AddListener(delegate { GameManager.Instance.LoadGame(); });
        if (mb != null)
            (mb as Button).onClick.AddListener(delegate { GameManager.Instance.LoadMultiplayerCave(); });
        if (rmb != null)
            (rmb as Button).onClick.AddListener(delegate { GameManager.Instance.LoadMainMenu(); });
    }


    public void LoadMainMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }

    public void LoadSingplayerScene()
    {
        GameManager.Instance.LoadGame();
        
    }
    public void LoadMultiplayerScene()
    {
        //Application.LoadLevel();
    }
}
