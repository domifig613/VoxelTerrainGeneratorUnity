using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour
{
    [SerializeField] private Vector3 gridSize;
    [SerializeField] private bool testing = false;
    [SerializeField] private bool testingReverse = true;
    [SerializeField] private int testingCount = 1;
    [SerializeField] private int testingGenerateCase = 0;


    [SerializeField] private DensityGenerator densityFunction;
    [SerializeField] private MarchingCubesAlgorithm marchingCubesAlgorithm;
    [SerializeField] private MarchingCubes marchingCubes;

    private (Vector3, float)[,,] terrainGrid;

    private void Start()
    {
        densityFunction.RegisterToTerrainDensityFunction(OnTerrainDensityGenerated);
        RunDensityGenerator();
    }

    public void RunDensityGenerator()
    {
        if (!testing)
        {
            densityFunction.StartGenerateTerrainDensity(gridSize);
        }
        else
        {
            densityFunction.StartGenerateTerrainDensity(testingReverse, testingCount, testingGenerateCase);
        }
    }

    private void OnTerrainDensityGenerated((Vector3, float)[,,] terrainDensity)
    {
        float[,,] terrainDensityWithoutPositions = new float[terrainDensity.GetLength(0), terrainDensity.GetLength(1), terrainDensity.GetLength(2)];

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    terrainDensityWithoutPositions[x, y, z] = terrainDensity[x, y, z].Item2;
                }
            }
        }

        marchingCubes.GenerateMesh(terrainDensityWithoutPositions, Vector3.zero);
       // marchingCubesAlgorithm.GenerateMesh(terrainDensity);
    }
}
