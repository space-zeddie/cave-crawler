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
        if (player != null) offset = transform.position - player.transform.position;
	}

    void FindPlayer()
    {
        for (int i = 0; i < UnitsParent.childCount; i++)
        {
            var unit = UnitsParent.GetChild(i).GetComponent<GameUnit>();
            if (unit != null)
            {
                if (unit is Carrier && unit.PlayerNumber == 0)
                {
                    player = unit.gameObject;
                    return;
                }
            }
        }
    }
	
	// Update is called once per frame
	void LateUpdate () 
    {
        if (player != null) transform.position = player.transform.position + offset;
	}
}
