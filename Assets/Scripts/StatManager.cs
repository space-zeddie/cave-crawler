using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class StatManager : Singleton<StatManager>
{
    public PlayerStatistics LocalCopyOfPlayerData = new PlayerStatistics();
    public bool IsSceneBeingLoaded = false;

   /* void Awake()
    {
        // STUB
        LocalCopyOfPlayerData.Score = 1;
        LocalCopyOfPlayerData.DeployedUnits = new GameObject[2];
        LocalCopyOfPlayerData.DeployedUnits[0] = CarrierPrefab;
        LocalCopyOfPlayerData.DeployedUnits[1] = SentinelPrefab;
    }*/

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

            saveFile.Close();
        }
        else
        {
            LocalCopyOfPlayerData.Score = 0;
            LocalCopyOfPlayerData.DeployedUnits = null;
           // LocalCopyOfPlayerData.DeployedUnits[0] = CarrierPrefab;
           // LocalCopyOfPlayerData.DeployedUnits[1] = SentinelPrefab;
        }
    }

}
