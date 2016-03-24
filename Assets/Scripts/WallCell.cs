using UnityEngine;
using System.Collections;

public class WallCell : Hexagon
{
    bool isReachable = false;

    public override Vector3 GetCellDimensions()
    {
        return GetComponent<Renderer>().bounds.size;
    }
    public override void MarkAsHighlighted()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }
    public override void MarkAsPath()
    {
        GetComponent<Renderer>().material.color = Color.clear;
    }
    public override void MarkAsReachable()
    {
        if (!isReachable) isReachable = true;
        GetComponent<Renderer>().material.color = Color.red;
    }
    public override void UnMark()
    {
        if (isReachable) isReachable = false;
        GetComponent<Renderer>().material.color = Color.black;
    }

    public void UnReach()
    {
        if (isReachable) isReachable = false;
    }
		
}
