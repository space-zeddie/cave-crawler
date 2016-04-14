using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour 
{
    /* only for learning networking */
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Translate(x, y, 0);
    }
}
