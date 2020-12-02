using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    [SerializeField] private VertexesCaseProvider vertexesCaseProvider;
    [SerializeField] private Material material;
    [SerializeField] float borderValue = 0f;

    private List<Vector3> meshVertices;

    public void GenerateMesh(Vector4[,,] terrainDensity, GameObject terrain)
    {
        meshVertices = new List<Vector3>();
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
                        Debug.Log("yy " + trianglesEdges[0, 0]);
                        CalclulateVertices(temporaryDensityArray, trianglesEdges, borderValue);
                    }
                }
            }
        }

        GenerateMesh(terrain);
    }

    public void GenerateMesh(OctreeElement root, GameObject terrain)
    {
        meshVertices = new List<Vector3>();

        if (vertexesCaseProvider.ReadVerticesIndexesFromFile())
        {
            List<OctreeElement> octreeBranches = root.NextElements.ToList();

            while (octreeBranches.Count != 0)
            {
                int count = octreeBranches.Count;

                for (int i = count - 1; i >= 0; i--)
                {
                    if (octreeBranches[i] != null)
                    {
                        OctreeElement[] nextElements = octreeBranches[i].NextElements;

                        if (nextElements == null)
                        {
                            Voxel voxel = octreeBranches[i] as Voxel;
                            int[,] trianglesEdges = vertexesCaseProvider.GetVerticesEdgesIndexes(vertexesCaseProvider.ReadCaseForCube(voxel.Density, borderValue));
                            Debug.Log("yyy " + trianglesEdges[0, 0]);
                            CalclulateVertices(voxel, trianglesEdges, borderValue);
                        }
                        else
                        {
                            octreeBranches.AddRange(nextElements);
                        }
                    }

                    octreeBranches.RemoveAt(i);
                }
            }
        }
        GenerateMesh(terrain);
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

    private void CalclulateVertices(Vector4[,,] terrainDensity, int[,] trianglesEdges, float borderValue)
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
                //Debug.Log("xx " + vertices[0]);
                meshVertices.AddRange(vertices);
            }
        }
    }

    private void CalclulateVertices(Voxel voxel, int[,] trianglesEdges, float borderValue)
    {
        for (int i = 0; i < trianglesEdges.GetLength(0); i++)
        {
            if (trianglesEdges[i, 0] != -1)
            {
                Vector3[] vertices = new Vector3[3];

                for (int j = 0; j < trianglesEdges.GetLength(1); j++)
                {
                    int[,] points = vertexesCaseProvider.GetVertexesIndexInArrayByEdge(trianglesEdges[i, j]);
                    Vector3 firstPoint = voxel.GetRange(points[0, 0], points[0, 1], points[0, 2]);
                    Vector3 secondPoint = voxel.GetRange(points[1, 0], points[1, 1], points[1, 2]);
                    float valueToInterpolate;

                    valueToInterpolate = (borderValue - voxel.Density[points[0, 0], points[0, 1], points[0, 2]]) /
                    (voxel.Density[points[1, 0], points[1, 1], points[1, 2]] - voxel.Density[points[0, 0], points[0, 1], points[0, 2]]);

                    vertices[j] = Vector3.Lerp(firstPoint, secondPoint, valueToInterpolate);
                }
               // Debug.Log("xxx " + vertices[0]);

                meshVertices.AddRange(vertices);
            }
        }
    }

    private void GenerateMesh(GameObject terrain)
    {
        if (meshVertices.Count < 64000)
        {

            GameObject terrainPart = new GameObject();
            terrainPart.name = "Mesh";
            MeshRenderer meshRenderer = terrainPart.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;
            MeshFilter meshFilter = terrainPart.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();

            mesh.vertices = meshVertices.ToArray();
            Debug.Log("ccc " + meshVertices.Count);
            int[] tris = new int[meshVertices.Count * 3];

            for (int i = 0; i < meshVertices.Count; i++)
            {
                tris[i] = i;
            }

            mesh.triangles = tris;
            meshFilter.sharedMesh = mesh;
            mesh.RecalculateNormals();
            terrainPart.transform.SetParent(terrain.transform);
        }
        else
        {
            Debug.Log("terrain to big " + meshVertices.Count);
        }
    }
}
