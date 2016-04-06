using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class StatManager : Singleton<StatManager>
{
    public PlayerStatistics LocalCopyOfPlayerData = new PlayerStatistics();
    public bool IsSceneBeingLoaded = false;
    public bool IsNewCave;
    
    public void SaveData()
    {
        if (!Directory.Exists("Saves"))
            Directory.CreateDirectory("Saves");

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Create("Saves/save.binary");

        LocalCopyOfPlayerData = PlayerState.Instance.LocalPlayerData;

        formatter.Serialize(saveFile, LocalCopyOfPlayerData);

        saveFile.Close();
    }

    public void LoadData()
    {
        if (Directory.Exists("Saves") && File.Exists("Saves/save.binary"))
        {
            Debug.Log("Loading Data");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream saveFile = File.Open("Saves/save.binary", FileMode.Open);

            LocalCopyOfPlayerData = (PlayerStatistics)formatter.Deserialize(saveFile);
            IsSceneBeingLoaded = true;
           // IsNewCave = false;

            saveFile.Close();
        }
        else
        {
            IsNewCave = true;
            LocalCopyOfPlayerData.Score = 0;
            LocalCopyOfPlayerData.DeployedUnits = null;
           // LocalCopyOfPlayerData.DeployedUnits[0] = CarrierPrefab;
           // LocalCopyOfPlayerData.DeployedUnits[1] = SentinelPrefab;
        }
    }

}
