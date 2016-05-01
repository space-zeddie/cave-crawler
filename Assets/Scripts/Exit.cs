using UnityEngine;
using System.Collections;

public class Exit : Collectable 
{
    public static readonly int BUFF_POINTS = 30;

    public void VanishUnit(GameUnit unit)
    {
        unit.enabled = false;
        unit.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void VanishUnit(GameUnitNet unit)
    {
        unit.enabled = false;
        unit.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
	
}
