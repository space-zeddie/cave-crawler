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
            if (InstatiatedObjects.transform.GetComponentInChildren<GameManager>() == null)
            {
                GameManager gm = Instantiate(GMPrefab);
                gm.transform.parent = InstatiatedObjects.transform;
                AssignButtonHandlers(gm);
            }
        }
        else Debug.LogError("The scene must contain one designated game object for storing runtime-generated objects");
    }

    void AssignButtonHandlers(GameManager gm)
    {
        SingleplayerButton sb = canvas.gameObject.transform.GetComponentInChildren<SingleplayerButton>();
        MultiplayerButton mb = canvas.gameObject.transform.GetComponentInChildren<MultiplayerButton>();
        ReturnToMenuButton rmb = canvas.gameObject.transform.GetComponentInChildren<ReturnToMenuButton>();
        if (sb != null)
            (sb as Button).onClick.AddListener(delegate { gm.LoadSingleplayerCave(); });
        if (mb != null)
            (mb as Button).onClick.AddListener(delegate { gm.LoadMultiplayerCave(); });
        if (rmb != null)
            (rmb as Button).onClick.AddListener(delegate { gm.LoadMainMenu(); });
    }


    public void LoadMainMenu()
    {
        Application.LoadLevel(0);
    }

    public void LoadSingplayerScene()
    {
        Application.LoadLevel(2);
    }
    public void LoadMultiplayerScene()
    {
        //Application.LoadLevel();
    }
}
