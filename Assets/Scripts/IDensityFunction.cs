using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDensityFunction
{
    (Vector3, float)[,,] GenerateTerrainDensity(Vector3 gridSize, bool reverse = false, int count = 0, int generateCase = 0);
}
