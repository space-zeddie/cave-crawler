using UnityEngine;
using System.Collections;

public class PlayerState : Singleton<PlayerState> 
{
    public PlayerStatistics LocalPlayerData = new PlayerStatistics();
    public bool Loaded = false;

    public void LoadFromGlobal()
    {
        Loaded = true;
        LocalPlayerData = StatManager.Instance.LocalCopyOfPlayerData;
    }

    public void SaveToGlobal()
    {
        Loaded = false;
        StatManager.Instance.LocalCopyOfPlayerData = LocalPlayerData;
    }
	
}
