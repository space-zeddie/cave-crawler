using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LinkToMainMenu : Singleton<LinkToMainMenu>
{
    public GameObject InstatiatedObjects;
    public GameObject GMPrefab;

    void Awake()
    {
        if (InstatiatedObjects != null)
        {
            if (InstatiatedObjects.transform.GetComponentInChildren<GameManager>() == null)
            {

                GameObject gm = Instantiate(GMPrefab);
                gm.transform.parent = InstatiatedObjects.transform;
            }
        }
        else Debug.LogError("The scene must contain one designated game object for storing runtime-generated objects");
    }


    public void LoadMainMenu()
    {
        Application.LoadLevel(0);
    }
}
