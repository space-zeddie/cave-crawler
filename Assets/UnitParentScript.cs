using UnityEngine;
using System.Collections;

public class UnitParentScript : MonoBehaviour
{
    public GameObject hexGrid;
    bool hasSelectedUnits = false;
    GameUnit selectedUnit = null;

    public void SetSelectedUnit(GameUnit unit)
    {
        hasSelectedUnits = true;
        selectedUnit = unit;
    }

    public void ClearSelectedUnit()
    {
        hasSelectedUnits = false;
        selectedUnit = null;
    }

    public bool HasSelectedUnits()
    {
        return hasSelectedUnits;
    }

    public GameUnit SelectedUnit()
    {
        return selectedUnit;
    }
}