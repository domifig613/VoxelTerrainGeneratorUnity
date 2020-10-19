using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SimpleBlocksSpawner : MonoBehaviour
{
    [SerializeField] private int xTerrainSize = 0;
    [SerializeField] private int yTerrainSize = 0;
    [SerializeField] private int zTerrainSize = 0;

    [SerializeField] private int maxSpawnProbabilityYLev = 5;
    [SerializeField] private int removeSpawnProbabilityPerYLev = 10;
    [SerializeField] private GameObject cube;

    [SerializeField] private Texture2D noise1;

    private int[,,] terrain;
    private int[,] terrain2;

    void OnEnable()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        //Terrain1();

        terrain2 = new int[64, 64];

        for (int  x = 0;  x < terrain2.GetLength(0);  x++)
        {
            for (int z = 0; z < terrain2.GetLength(1); z++)
            {
                Instantiate(cube, new Vector3(x, noise1.GetPixel(x, z).r*2, z), new Quaternion(), this.gameObject.transform);
            }
        }
        


        stopwatch.Stop();

        UnityEngine.Debug.Log("Total terrain create time: " + stopwatch.Elapsed.TotalSeconds);
    }

    private void Terrain1()
    {
        terrain = new int[xTerrainSize, yTerrainSize, zTerrainSize];

        for (int x = 0; x < xTerrainSize; x++)
        {
            for (int y = 0; y < yTerrainSize; y++)
            {
                int minusValue = -100 - (Mathf.Abs(y - maxSpawnProbabilityYLev) * removeSpawnProbabilityPerYLev);

                for (int z = 0; z < zTerrainSize; z++)
                {
                    short visible = (short)Random.Range(minusValue, 101);
                    terrain[x, y, z] = ((y == 0 || terrain[x, y - 1, z] > 0) ? visible : -1);
                    terrain[x, y, z] = (maxSpawnProbabilityYLev == y ? 1 : terrain[x, y, z]);

                    if (terrain[x, y, z] > 0)
                    {
                        Instantiate(cube, new Vector3(x, y, z), new Quaternion(), this.gameObject.transform);
                    }
                }
            }
        }
    }
}
