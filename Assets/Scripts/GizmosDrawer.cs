using System;
using System.Collections.Generic;
using UnityEngine;

public class GizmosDrawer : MonoBehaviour
{
    [SerializeField] private Vector3 gridSize;
    [SerializeField] private DensityGenerator densityFunction;

    private (Vector3, float)[,,] terrainDensity;

    private void Start()
    {
        densityFunction.RegisterToTerrainDensityFunction(OnTerrainDensityGenerated);
    }

    private void OnDestroy()
    {
        densityFunction.UnregisterToTerrainDensityFunction(OnTerrainDensityGenerated);
    }

    private void OnDrawGizmos()
    {
        DrawGrid();
        DrawTerrainPoints();
    }

    private void OnTerrainDensityGenerated((Vector3, float)[,,] terrainDensity)
    {
        this.terrainDensity = terrainDensity;
    }

    private void DrawGrid()
    {
        Gizmos.color = Color.blue;

        for (int x = 0; x <= (int)gridSize.x; x++)
        {
            for (int y = 0; y <= (int)gridSize.y; y++)
            {
                for (int z = 0; z <= (int)gridSize.z; z++)
                {
                    if (x != (int)gridSize.x)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x + 1, y, z));
                    }

                    if (y != (int)gridSize.y)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x, y + 1, z));
                    }

                    if (z != (int)gridSize.z)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x, y, z + 1));
                    }
                }
            }
        }
    }

    private void DrawTerrainPoints()
    {
        if (terrainDensity != null)
        {
            (Vector3, float)[,,] terrainDensityCopy = terrainDensity; 

            Gizmos.color = Color.red;

            foreach (var point in terrainDensityCopy)
            {
                if (point.Item2 > 0)
                {
                    Gizmos.DrawWireSphere(point.Item1, 0.1f);
                }
            }
        }

    }
}
