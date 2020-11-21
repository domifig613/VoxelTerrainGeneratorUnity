using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityFunction : MonoBehaviour
{
    [SerializeField] NoiseProvider noiseProvider;

    public (Vector3, float)[,,] GenerateTerrainDensity(Vector3 gridSize, bool reverse = false, int count = 0, int generateCase = 0)
    {
        (Vector3, float)[,,] terrainDensity = new (Vector3, float)[(int)gridSize.x + 1, (int)gridSize.y + 1, (int)gridSize.z + 1];
        List<float[,,]> noiseTextures3D = new List<float[,,]>();
        noiseProvider.GetTexturesNoiseNames().ForEach(x => noiseTextures3D.Add(noiseProvider.ReadNoise(x)));
        float[,] texturesMultipliers = new float[,] { 
            { 1.6f, 0.32f }, 
            { 0.8f, 0.64f }, 
            { 0.4f, 1.28f }, 
            { 0.2f, 2.56f },
            { 0.1f, 5f },
            { 0.05f, 10f },
            { 0.025f, 20f },
            { 0.012f, 40f },
            { 0.2499f, 2f },
        };

        for (int x = 0; x <= gridSize.x; x++)
        {
            for (int y = 0; y <= gridSize.y; y++)
            {
                for (int z = 0; z <= gridSize.z; z++)
                {
                    float value;
                    value = -y;

                    //for (int i = 0; i < 8; i++)
                    //{
                    //    value += noiseTextures3D[i][Convert.ToInt32(x * texturesMultipliers[i,0]) % noiseTextures3D[i].GetLength(0),
                    //        Convert.ToInt32(y * texturesMultipliers[i,0]) % noiseTextures3D[i].GetLength(1), Convert.ToInt32(z * texturesMultipliers[i,0]) % noiseTextures3D[i].GetLength(2)] * texturesMultipliers[i,1];
                    //}

                    value += noiseTextures3D[0][x % noiseTextures3D[0].GetLength(0), y % noiseTextures3D[0].GetLength(1), z % noiseTextures3D[0].GetLength(2)] * 2;

                   // value = (float)Math.Round((double)value, 1);
                    terrainDensity[x, y, z] = (new Vector3(x, y, z), value);
                }
            }
        }

        return terrainDensity;
    }
}
