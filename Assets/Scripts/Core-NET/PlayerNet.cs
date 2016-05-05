using UnityEngine;
using UnityEngine.Networking;

public abstract class PlayerNet : NetworkBehaviour
{
    public int PlayerNumber;
    /// <summary>
    /// Method is called every turn. Allows player to interact with his units.
    /// </summary>         
    public abstract void Play(CellGridNet cellGrid);
}