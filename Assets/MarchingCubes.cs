using System.Diagnostics;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    [SerializeField] private VertexesCaseProvider vertexesCaseProvider;
    [SerializeField] private Material material;
    [SerializeField] float borderValue = 0f;

    public void GenerateMesh(Vector4[,,] terrainDensity, GameObject terrain)
    {
        Stopwatch d = new Stopwatch();
        d.Start();

        Vector4[,,] temporaryDensityArray = new Vector4[2, 2, 2];

        if (vertexesCaseProvider.ReadVerticesIndexesFromFile())
        {
            for (int x = 0; x < terrainDensity.GetLength(0) - 1; x++)
            {
                for (int y = 0; y < terrainDensity.GetLength(1) - 1; y++)
                {
                    for (int z = 0; z < terrainDensity.GetLength(2) - 1; z++)
                    {
                        FillTemporaryArray(terrainDensity, temporaryDensityArray, x, y, z);
                        int[,] trianglesEdges = vertexesCaseProvider.GetVerticesEdgesIndexes(vertexesCaseProvider.ReadCaseForCube(temporaryDensityArray, borderValue));
                        DrawMesh(temporaryDensityArray, trianglesEdges, terrain, borderValue);
                    }
                }
            }
        }

        d.Stop();
        UnityEngine.Debug.Log(d.ElapsedMilliseconds);
    }

    private void FillTemporaryArray(Vector4[,,] terrainDensity, Vector4[,,] temporaryDensityArray, int x, int y, int z)
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

    private void DrawMesh(Vector4[,,] terrainDensity, int[,] trianglesEdges, GameObject terrain, float borderValue)
    {
        for (int i = 0; i < trianglesEdges.GetLength(0); i++)
        {
            if (trianglesEdges[i, 0] != -1)
            {
                Vector3[] vertices = new Vector3[3];

                for (int j = 0; j < trianglesEdges.GetLength(1); j++)
                {
                    int[,] points = vertexesCaseProvider.GetVertexesIndexInArrayByEdge(trianglesEdges[i, j]);
                    Vector3 firstPoint = new Vector3(
                        terrainDensity[points[0, 0], points[0, 1], points[0, 2]].x,
                        terrainDensity[points[0, 0], points[0, 1], points[0, 2]].y, 
                        terrainDensity[points[0, 0], points[0, 1], points[0, 2]].z);
                    Vector3 secondPoint = new Vector3(
                        terrainDensity[points[1, 0], points[1, 1], points[1, 2]].x,
                        terrainDensity[points[1, 0], points[1, 1], points[1, 2]].y,
                        terrainDensity[points[1, 0], points[1, 1], points[1, 2]].z);
                    float valueToInterpolate;

                    valueToInterpolate = (borderValue - terrainDensity[points[0, 0], points[0, 1], points[0, 2]].w) /
                    (terrainDensity[points[1, 0], points[1, 1], points[1, 2]].w - terrainDensity[points[0, 0], points[0, 1], points[0, 2]].w);

                    vertices[j] = Vector3.Lerp(firstPoint, secondPoint, valueToInterpolate);
                }

                GameObject terrainPart = new GameObject();
                terrainPart.name = "Mesh";

                MeshRenderer meshRenderer = terrainPart.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = material;
                MeshFilter meshFilter = terrainPart.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();

                mesh.vertices = vertices;

                int[] tris = new int[3]
                {
                    0, 1, 2
                };

                mesh.triangles = tris;

                Vector3 normal = TerrainGeneratorHelper.GetNormalFromTriangle(vertices);

                Vector3[] normals = new Vector3[3]
                {
                     normal,
                     normal,
                     normal,
                };

                mesh.normals = normals;

                Vector2[] uv = new Vector2[3]
                {
                        new Vector2(0, 0),
                        new Vector2(1, 0),
                        new Vector2(1, 1),
                };

                mesh.uv = uv;

                Color[] colors = new Color[3]
                {
                        Color.red,
                        Color.green,
                        Color.blue,
                };

                mesh.colors = colors;

                meshFilter.mesh = mesh;
                terrainPart.transform.SetParent(terrain.transform);
            }
        }
    }
}
