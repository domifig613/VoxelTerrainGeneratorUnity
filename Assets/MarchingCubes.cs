using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    [SerializeField] private VertexesCaseProvider vertexesCaseProvider;

    public void GenerateMesh(float[,,] terrainDensity, Vector3 startPoint)
    {
        float[,,] temporaryDensityArray = new float[2, 2, 2];

        if (vertexesCaseProvider.ReadVerticesIndexesFromFile())
        {
            for (int x = 0; x < terrainDensity.GetLength(0) -1; x++)
            {
                for (int y = 0; y < terrainDensity.GetLength(1) -1; y++)
                {
                    for (int z = 0; z < terrainDensity.GetLength(2) -1; z++)
                    {
                        FillTemporaryArray(terrainDensity, temporaryDensityArray, x, y, z);
                        Debug.Log(x);
                    }
                }
            }
        }
    }

    private static void FillTemporaryArray(float[,,] terrainDensity, float[,,] temporaryDensityArray, int x, int y, int z)
    {
        temporaryDensityArray[0, 0, 0] = terrainDensity[x, y, z];
        temporaryDensityArray[0, 0, 1] = terrainDensity[x, y, z + 1];
        temporaryDensityArray[0, 1, 0] = terrainDensity[x, y + 1, z];
        temporaryDensityArray[0, 1, 1] = terrainDensity[x, y + 1, z + 1];
        temporaryDensityArray[1, 0, 0] = terrainDensity[x + 1, y, z];
        temporaryDensityArray[1, 0, 1] = terrainDensity[x + 1, y, z + 1];
        temporaryDensityArray[1, 1, 0] = terrainDensity[x + 1, y + 1, z];
        temporaryDensityArray[1, 1, 1] = terrainDensity[x + 1, y + 1, z + 1];
    }
}
