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
        if (player == null)
        {
            FindPlayer();
        }
        if (player != null)
        {
            this.transform.position = new Vector3(transform.position.x, transform.position.y, this.transform.position.z); 
            offset = transform.position - player.transform.position;
        }
	}

    void FindPlayer()
    {
        Carrier[] carriers = UnitsParent.GetComponentsInChildren<Carrier>();
        foreach (Carrier carrier in carriers)
        {
            if (carrier.PlayerNumber == 0)
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
