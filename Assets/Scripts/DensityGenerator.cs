using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityGenerator : MonoBehaviour
{
    [SerializeField] DensityFunction densityFunctionMono;
    private event Action<(Vector3, float)[,,]> onTerrainDenistyGenerated = delegate { };
    private IDensityFunction densityFunction;
    private (Vector3, float)[,,] terrainDensity;
    private bool listWasGenerated = false;

    public void StartGenerateTerrainDensity(Vector3 gridSize)
    {
        densityFunction = new SimpleDensityFunction();
        terrainDensity = densityFunctionMono.GenerateTerrainDensity(gridSize);
        listWasGenerated = true;
        onTerrainDenistyGenerated(terrainDensity);
    }

    public void StartGenerateTerrainDensity(bool reverse, int count, int generateCase)
    {
        densityFunction = new DebugDensityFunction();
        terrainDensity = densityFunction.GenerateTerrainDensity(new Vector3(1,1,1), reverse, count, generateCase);
        listWasGenerated = true;
        onTerrainDenistyGenerated(terrainDensity);
    }

    public void RegisterToTerrainDensityFunction(Action<(Vector3, float)[,,]> onGenerated)
    {
        onTerrainDenistyGenerated += onGenerated;
        
        if(listWasGenerated)
        {
            onGenerated(terrainDensity);
        }
    }

    public void UnregisterToTerrainDensityFunction(Action<(Vector3, float)[,,]> onGenerated)
    {
        onTerrainDenistyGenerated -= onGenerated;
    }
}
