using UnityEngine;
using System.Collections;

public class Exit : Collectable 
{

    public void VanishUnit(GameUnit unit)
    {
        unit.enabled = false;
        unit.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
	
}
