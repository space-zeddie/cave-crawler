using UnityEngine;
using System.Collections;

public class PlayerState : Singleton<PlayerState> 
{
    public PlayerStatistics LocalPlayerData = new PlayerStatistics();

    public void LoadFromGlobal()
    {
        LocalPlayerData = StatManager.Instance.LocalCopyOfPlayerData;
    }

    public void SaveToGlobal()
    {
        StatManager.Instance.LocalCopyOfPlayerData = LocalPlayerData;
    }
	
}
