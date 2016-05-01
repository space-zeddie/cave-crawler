using System.Collections.Generic;

public interface IUnitGenerator
{
     List<Unit> SpawnUnits(List<Cell> cells);
     List<UnitNet> SpawnUnits(List<CellNet> cells);
}

