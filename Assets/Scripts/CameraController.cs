using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
    public Transform UnitsParent;
    GameObject player;
    private Vector3 offset;

	// Use this for initialization
	void Start ()
    {
        
	}

    public void RelocateToPlayer(int playerNumber = 0)
    {
        if (player == null)
            FindPlayer(playerNumber);
        if (player != null)
        {
            this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y);
            offset = transform.position - player.transform.position;
        }
        else Debug.Log("No carrier found");
    }

 //   public IEnumerator 

    void FindPlayer(int playerNumber)
    {
        Carrier[] carriers = UnitsParent.GetComponentsInChildren<Carrier>();
        if (carriers.GetLength(0) < 1)
        {
            FindPlayerNetwork(playerNumber);
            return;
        }
        foreach (Carrier carrier in carriers)
        {
            if (carrier.PlayerNumber == playerNumber)
            {
                player = carrier.gameObject;
                break;
            }
        }
    }

    void FindPlayerNetwork(int playerNumber)
    {
        CarrierNet[] carriers = UnitsParent.GetComponentsInChildren<CarrierNet>();
        foreach (CarrierNet carrier in carriers)
        {
            if (carrier.PlayerNumber == playerNumber)
            {
                player = carrier.gameObject;
                break;
            }
        }
    }
	
	
	// Update is called once per frame
	void LateUpdate () 
    {
        if (player != null) transform.position = player.transform.position + offset;
	}
}
