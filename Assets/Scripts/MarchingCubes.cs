using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] float borderValue = 0f;

    private List<Vector3> meshVertices;
    private List<Vector2> uv;

    public void GenerateMesh(OctreeElement root, GameObject terrain)
    {
        meshVertices = new List<Vector3>();
        uv = new List<Vector2>();

        if (VertexesCaseProvider.ReadVerticesIndexesFromFile())
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

                            if (voxel.Drawable)
                            {
                                foreach (var vertices in voxel.Vertices)
                                {
                                    meshVertices.AddRange(vertices);
                                    uv.AddRange(voxel.GetUVPositionForVoxel());
                                }
                            }
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

    public static List<Vector3[]> GetVertices(Voxel voxel, int[,] trianglesEdges, float borderValue)
    {
        List<Vector3[]> allVertices = new List<Vector3[]>();

        for (int i = 0; i < trianglesEdges.GetLength(0); i++)
        {
            if (trianglesEdges[i, 0] != -1)
            {
                Vector3[] vertices = new Vector3[3];

                for (int j = 0; j < trianglesEdges.GetLength(1); j++)
                {
                    int[,] points = VertexesCaseProvider.GetVertexesIndexInArrayByEdge(trianglesEdges[i, j]);
                    Vector3 firstPoint = voxel.GetRange(points[0, 0], points[0, 1], points[0, 2]);
                    Vector3 secondPoint = voxel.GetRange(points[1, 0], points[1, 1], points[1, 2]);
                    float valueToInterpolate;

                    valueToInterpolate = (borderValue - voxel.Density[points[0, 0], points[0, 1], points[0, 2]]) /
                    (voxel.Density[points[1, 0], points[1, 1], points[1, 2]] - voxel.Density[points[0, 0], points[0, 1], points[0, 2]]);

                    vertices[j] = Vector3.Lerp(firstPoint, secondPoint, valueToInterpolate);
                }

                allVertices.Add(vertices);
            }
        }

        return allVertices;
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
            int[] tris = new int[meshVertices.Count * 3];

            for (int i = 0; i < meshVertices.Count; i++)
            {
                tris[i] = i;
            }


            mesh.triangles = tris;
            mesh.uv = uv.ToArray();
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
