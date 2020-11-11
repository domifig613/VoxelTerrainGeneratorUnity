using System;
using UnityEngine;

public class SimpleDensityFunction : IDensityFunction
{
    (Vector3, float)[,,] IDensityFunction.GenerateTerrainDensity(Vector3 gridSize, bool reverse = false, int count = 0, int generateCase = 0)
    {
        (Vector3, float)[,,] terrainDensity = new (Vector3, float)[(int)gridSize.x + 1, (int)gridSize.y + 1, (int)gridSize.z + 1];

        for (int x = 0; x <= gridSize.x; x++)
        {
            for (int y = 0; y <= gridSize.y; y++)
            {
                for (int z = 0; z <= gridSize.z; z++)
                {
                    float value;
                    value = -y;
                    terrainDensity[x, y, z] = (new Vector3(x, y, z), value);
                }
            }
        }

        return terrainDensity;
    }
}
