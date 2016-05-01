using UnityEngine;
using System.Collections;

public class UnitParentScript : Singleton<UnitParentScript>
{
    bool hasSelectedUnits = false;
    GameUnit selectedUnit = null;
    GameUnitNet selectedUnitNet = null;

    public void SetSelectedUnit(GameUnit unit)
    {
        hasSelectedUnits = true;
        selectedUnit = unit;
    }
    public void SetSelectedUnit(GameUnitNet unit)
    {
        hasSelectedUnits = true;
        selectedUnitNet = unit;
    }

    public void ClearSelectedUnit()
    {
        hasSelectedUnits = false;
        selectedUnit = null;
        selectedUnitNet = null;
    }

    public bool HasSelectedUnits()
    {
        return hasSelectedUnits;
    }

    public GameObject SelectedUnit()
    {
        if (selectedUnit != null) return selectedUnit.gameObject;
        else return selectedUnitNet.gameObject;
    }
}