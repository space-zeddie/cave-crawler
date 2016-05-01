using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class ICellGridGeneratorNet : NetworkBehaviour
{
    public Transform CellsParent;
    public abstract List<CellNet> GenerateGrid();
}

