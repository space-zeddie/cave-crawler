using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class NetworkGameUnit : NetworkBehaviour 
{
    [SyncVar]
    double x, y, z;

    void Start()
    {
        x = this.gameObject.transform.position.x;
        y = this.gameObject.transform.position.y;
        z = this.gameObject.transform.position.z;
    }

    [Command]
    public static void CmdSpawn(GameObject prefab, Vector3 position)
    {
        if (prefab.GetComponent<NetworkIdentity>() == null) return;
        var go = (GameObject)Instantiate(prefab, position, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(go, LocalPlayer());

    }

    public static GameObject LocalPlayer()
    {
        var players = GameObject.FindObjectOfType<PlayersParent>().gameObject.transform;
        GameObject player = null;
        for (int i = 0; i < players.childCount; ++i)
            if (players.GetChild(i).GetComponent<NetworkIdentity>().isLocalPlayer)
                player = players.GetChild(i).gameObject;
        return player;

    }

    /*[Command]
    public IEnumerator CmdMove(List<Cell> path, float MovementSpeed)
    {
        if (isLocalPlayer)
        {
            this.gameObject.GetComponent<GameUnit>().isMoving = true;
            path.Reverse();
            foreach (var cell in path)
            {
                while (new Vector2(transform.position.x, transform.position.y) != new Vector2(cell.transform.position.x, cell.transform.position.y))
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(cell.transform.position.x, cell.transform.position.y, transform.position.z), Time.deltaTime * MovementSpeed);
                    yield return 0;
                }
            }
            this.gameObject.GetComponent<GameUnit>().isMoving = true;
            x = this.gameObject.transform.position.x;
            y = this.gameObject.transform.position.y;
            z = this.gameObject.transform.position.z;
        }
    }*/

    public double X() { return x; }
    public double Y() { return y; }
    public double Z() { return z; }
}
