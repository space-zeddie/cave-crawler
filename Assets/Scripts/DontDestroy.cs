using UnityEngine;
using System.Collections;

public class DontDestroy : Singleton<DontDestroy>
{

    // Use this for initialization
    void Awake()
    {

        DontDestroyOnLoad(gameObject);

    }
}
