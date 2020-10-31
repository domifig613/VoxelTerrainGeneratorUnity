using System;
using UnityEngine;

public class SimpleDensityFunction : IDensityFunction
{
    (Vector3, float)[,,] IDensityFunction.GenerateTerrainDensity(Vector3 gridSize, bool reverse = false, int count = 0, int generateCase = 0)
    {
        Texture2D noise = Resources.Load<Texture2D>("noise2");
        Texture2D noise2 = Resources.Load<Texture2D>("noise3");
        Texture2D noise3 = Resources.Load<Texture2D>("noise4");
        (Vector3, float)[,,] terrainDensity = new (Vector3, float)[(int)gridSize.x + 1, (int)gridSize.y + 1, (int)gridSize.z + 1];

        for (int x = 0; x <= gridSize.x; x++)
        {
            for (int y = 0; y <= gridSize.y; y++)
            {
                for (int z = 0; z <= gridSize.z; z++)
                {
                    float value;
                    value = -y;
                    //value += noise.GetPixel(x*2, z*2).r*2;
                    //value += noise2.GetPixel(x, z).r*4f;
                    value += noise2.GetPixel(Convert.ToInt32(x *3.87f), Convert.ToInt32(z *3.87f)).r * 0.97f;
                    value += noise2.GetPixel(Convert.ToInt32(x *1.87f), Convert.ToInt32(z *1.87f)).r * 2.03f;
                    value += noise2.GetPixel(Convert.ToInt32(x *1.02f), Convert.ToInt32(z *1.02f)).r * 4.07f;
                    //value += noise2.GetPixel(x * 2, z * 2).r;
                    //value += noise2.GetPixel(x, z).r * 2f;
                    //value += noise2.GetPixel(x, z).r*10f;
                    //Debug.Log(noise.GetPixel(x, z).r);
                    terrainDensity[x, y, z] = (new Vector3(x, y, z), value);
                }
            }
        }

        return terrainDensity;
    }
}
