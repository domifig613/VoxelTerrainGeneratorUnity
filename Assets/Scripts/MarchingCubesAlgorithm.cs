using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarchingCubesAlgorithm : MonoBehaviour
{
    [SerializeField] private Material material;

    public void GenerateMesh((Vector3, float)[,,] pointsWithDensity)
    {
        (Vector3, float)[,,] oneCube = new (Vector3, float)[2, 2, 2];

        for (int x = 0; x < pointsWithDensity.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < pointsWithDensity.GetLength(1) - 1; y++)
            {
                for (int z = 0; z < pointsWithDensity.GetLength(2) - 1; z++)
                {
                    oneCube[0, 0, 0] = (new Vector3(x, y, z), pointsWithDensity[x, y, z].Item2);
                    oneCube[0, 0, 1] = (new Vector3(x, y, z + 1), pointsWithDensity[x, y, z + 1].Item2);
                    oneCube[0, 1, 0] = (new Vector3(x, y + 1, z), pointsWithDensity[x, y + 1, z].Item2);
                    oneCube[0, 1, 1] = (new Vector3(x, y + 1, z + 1), pointsWithDensity[x, y + 1, z + 1].Item2);
                    oneCube[1, 0, 0] = (new Vector3(x + 1, y, z), pointsWithDensity[x + 1, y, z].Item2);
                    oneCube[1, 0, 1] = (new Vector3(x + 1, y, z + 1), pointsWithDensity[x + 1, y, z + 1].Item2);
                    oneCube[1, 1, 0] = (new Vector3(x + 1, y + 1, z), pointsWithDensity[x + 1, y + 1, z].Item2);
                    oneCube[1, 1, 1] = (new Vector3(x + 1, y + 1, z + 1), pointsWithDensity[x + 1, y + 1, z + 1].Item2);

                    GenerateMeshForOneCube(oneCube);
                }
            }
        }
    }

    private void GenerateMeshForOneCube((Vector3, float)[,,] densityInOneCube)
    {
        int pointsInsideTerrainCount = 0;

        for (int x = 0; x < densityInOneCube.GetLength(0); x++)
        {
            for (int y = 0; y < densityInOneCube.GetLength(1); y++)
            {
                for (int z = 0; z < densityInOneCube.GetLength(2); z++)
                {
                    if (densityInOneCube[x, y, z].Item2 > 0)
                    {
                        pointsInsideTerrainCount++;
                    }
                }
            }
        }

        switch (pointsInsideTerrainCount)
        {
            case 1:
                GenerateMeshForOnePoinsInside(densityInOneCube, false);
                break;
            case 7:
                GenerateMeshForOnePoinsInside(densityInOneCube, true);
                break;
            case 2:
                GenerateMeshForTwoPoinsInside(densityInOneCube, false);
                break;
            case 6:
                GenerateMeshForTwoPoinsInside(densityInOneCube, true);
                break;
            case 3:
                GenerateMeshForThreePoinsInside(densityInOneCube, false);
                break;
            case 5:
                GenerateMeshForThreePoinsInside(densityInOneCube, true);
                break;
            case 4:
                GenerateMeshForFourPoinsInside(densityInOneCube, false);
                break;
            default:
                //Debug.LogError($"{nameof(MarchingCubesAlgorithm)}. {nameof(GenerateMesh)}. unexpected points inside terrain count: {pointsInsideTerrainCount}");
                break;

        }
    }

    #region OnePoint
    private void GenerateMeshForOnePoinsInside((Vector3, float)[,,] pointsWithDensity, bool reverse = false)
    {
        Vector3 center = Vector3.Lerp(pointsWithDensity[0, 0, 0].Item1, pointsWithDensity[1, 1, 1].Item1, 0.5f);
        Vector3 pointInside = new Vector3();

        for (int x = 0; x < pointsWithDensity.GetLength(0); x++)
        {
            for (int y = 0; y < pointsWithDensity.GetLength(1); y++)
            {
                for (int z = 0; z < pointsWithDensity.GetLength(2); z++)
                {
                    if (pointsWithDensity[x, y, z].Item2 > 0)
                    {
                        pointInside = pointsWithDensity[x, y, z].Item1;
                    }
                }
            }
        }

        List<Vector3> vertices = new List<Vector3>();

        for (int x = 0; x < pointsWithDensity.GetLength(0); x++)
        {
            for (int y = 0; y < pointsWithDensity.GetLength(1); y++)
            {
                for (int z = 0; z < pointsWithDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsWithDensity[x, y, z].Item1, pointInside) == 2)
                    {
                        vertices.Add(Vector3.Lerp(pointsWithDensity[x, y, z].Item1, pointInside, 0.5f));
                    }
                }
            }
        }


        Vector3 normal = (reverse ? center - pointInside : pointInside - center);

        GameObject terrainPart = new GameObject();
        terrainPart.name = "Mesh";

        MeshRenderer meshRenderer = terrainPart.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        MeshFilter meshFilter = terrainPart.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        if (Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[1]).normalized != -normal.normalized)
        {
            vertices.Reverse();
        }

        mesh.vertices = vertices.ToArray();

        int[] tris = new int[3]
        {
            0, 1, 2
        };

        mesh.triangles = tris;

        Vector3[] normals = new Vector3[3]
        {
            -normal,
            -normal,
            -normal
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
        terrainPart.transform.SetParent(gameObject.transform);
    }
    #endregion

    #region TwoPoint
    private void GenerateMeshForTwoPoinsInside((Vector3, float)[,,] pointsWithDensity, bool reverse = false)
    {
        List<Vector3> pointsInside = new List<Vector3>();

        for (int x = 0; x < pointsWithDensity.GetLength(0); x++)
        {
            for (int y = 0; y < pointsWithDensity.GetLength(1); y++)
            {
                for (int z = 0; z < pointsWithDensity.GetLength(2); z++)
                {
                    if ((pointsWithDensity[x, y, z].Item2 > 0f && !reverse) || (!(pointsWithDensity[x, y, z].Item2 > 0f) && reverse))
                    {
                        pointsInside.Add(pointsWithDensity[x, y, z].Item1);
                    }
                }
            }
        }

        DrawSeparatePoints(pointsWithDensity, reverse, pointsInside);

        if(pointsInside.Count == 2)
        {
            GenerateMeshForTwoPointsNextToEachOter(pointsWithDensity, reverse, pointsInside);
        }
    }

    private void GenerateMeshForTwoPointsNextToEachOter((Vector3, float)[,,] pointsWithDensity, bool reverse, List<Vector3> pointsInside)
    {
        Vector3 center = Vector3.Lerp(pointsWithDensity[0, 0, 0].Item1, pointsWithDensity[1, 1, 1].Item1, 0.5f);
        List<Vector3> meshPoints = new List<Vector3>();
        List<Vector3> meshPointsCopy = new List<Vector3>();

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    if (pointsInside.Exists(t => TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsWithDensity[x, y, z].Item1, t) == 2)
                   && !pointsInside.Exists(t => TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsWithDensity[x, y, z].Item1, t) == 3))
                    {
                        meshPointsCopy.Add(pointsWithDensity[x, y, z].Item1);
                    }
                }
            }
        }

        meshPoints.Add(meshPointsCopy[0]);
        meshPoints.Add(meshPointsCopy.Find(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(meshPoints[0], x) == 1));
        meshPoints.Add(meshPointsCopy.Find(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(meshPoints[1], x) == 2));
        meshPoints.Add(meshPointsCopy.Find(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(meshPoints[2], x) == 1));

        for (int i = 0; i < meshPoints.Count; i++)
        {
            meshPoints[i] = Vector3.Lerp(meshPoints[i], pointsInside.Find(t => TerrainGeneratorHelper.CalculateCommonPlanesCount(meshPoints[i], t) == 2), 0.5f);
        }

        Vector3 normal = (reverse ? center - (Vector3.Lerp(pointsInside[0], pointsInside[1], 0.5f)) : (Vector3.Lerp(pointsInside[0], pointsInside[1], 0.5f)) - center);

        GameObject terrainPart = new GameObject();
        terrainPart.name = "Mesh";

        MeshRenderer meshRenderer = terrainPart.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        MeshFilter meshFilter = terrainPart.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        if (Vector3.Cross(meshPoints[0] - meshPoints[1], meshPoints[2] - meshPoints[1]).normalized == -normal.normalized)
        {
            meshPoints.Reverse();
        }

        mesh.vertices = meshPoints.ToArray();

        int[] tris = new int[6]
        {
            0, 1, 2,
            2, 3, 0
        };

        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
         {
            -normal,
            -normal,
            -normal,
            -normal,
        };

        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        mesh.uv = uv;


        Color[] colors = new Color[4]
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.blue,
        };

        mesh.colors = colors;

        meshFilter.mesh = mesh;
        terrainPart.transform.SetParent(gameObject.transform);
    }
    #endregion

    #region ThreePoint
    private void GenerateMeshForThreePoinsInside((Vector3, float)[,,] pointsWithDensity, bool reverse = false)
    {
        List<Vector3> pointsInside = new List<Vector3>();

        for (int x = 0; x < pointsWithDensity.GetLength(0); x++)
        {
            for (int y = 0; y < pointsWithDensity.GetLength(1); y++)
            {
                for (int z = 0; z < pointsWithDensity.GetLength(2); z++)
                {
                    if ((pointsWithDensity[x, y, z].Item2 > 0f && !reverse) || (!(pointsWithDensity[x, y, z].Item2 > 0f) && reverse))
                    {
                        pointsInside.Add(pointsWithDensity[x, y, z].Item1);
                    }
                }
            }
        }

        DrawSeparatePoints(pointsWithDensity, reverse, pointsInside);

        if (pointsInside.Count == 2)
        {
            GenerateMeshForTwoPointsNextToEachOter(pointsWithDensity, reverse, pointsInside);
        }
        else
        {
            GenerateMeshForThreePointsNextToEachOther(pointsWithDensity, pointsInside, reverse);
        }
    }

    private void GenerateMeshForThreePointsNextToEachOther((Vector3, float)[,,] pointsWithDensity, List<Vector3> pointsInside, bool reverse = false)
    {
        Vector3 center = Vector3.Lerp(pointsWithDensity[0, 0, 0].Item1, pointsWithDensity[1, 1, 1].Item1, 0.5f);
        Vector3[] pointsNeededToDetectPointsOnTerrain = new Vector3[4];
        Vector3 pointForCalculateNormal = new Vector3();
        bool firstEdgeFilled = false;

        for (int x = 0; x < pointsWithDensity.GetLength(0); x++)
        {
            for (int y = 0; y < pointsWithDensity.GetLength(1); y++)
            {
                for (int z = 0; z < pointsWithDensity.GetLength(2); z++)
                {
                    int planesCount = 0;

                    foreach (var pointInside in pointsInside)
                    {
                        int commonPlanes = TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsWithDensity[x, y, z].Item1, pointInside);

                        if (commonPlanes == 3)
                        {
                            planesCount = 0;
                            break;
                        }
                        else
                        {
                            planesCount += commonPlanes;
                        }
                    }

                    if (planesCount == 5)
                    {
                        pointsNeededToDetectPointsOnTerrain[0] = pointsWithDensity[x, y, z].Item1;
                    }
                    else if (planesCount == 4)
                    {
                        pointsNeededToDetectPointsOnTerrain[1] = pointsWithDensity[x, y, z].Item1;
                    }
                    else if (planesCount == 3)
                    {
                        if (!firstEdgeFilled)
                        {
                            firstEdgeFilled = true;
                            pointsNeededToDetectPointsOnTerrain[2] = pointsWithDensity[x, y, z].Item1;
                        }
                        else
                        {
                            pointsNeededToDetectPointsOnTerrain[3] = pointsWithDensity[x, y, z].Item1;
                        }
                    }
                }
            }
        }

        Vector3[] verticesToGenerateTerrain = new Vector3[5];

        foreach (var pointInside in pointsInside)
        {
            int commonPlanes = TerrainGeneratorHelper.CalculateCommonPlanesCount(pointInside, pointsNeededToDetectPointsOnTerrain[0]);

            if (commonPlanes == 2)
            {
                if (verticesToGenerateTerrain[0] == Vector3.zero)
                {
                    verticesToGenerateTerrain[0] = Vector3.Lerp(pointInside, pointsNeededToDetectPointsOnTerrain[0], 0.5f);

                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(pointInside, pointsNeededToDetectPointsOnTerrain[2]) == 2)
                    {
                        verticesToGenerateTerrain[1] = Vector3.Lerp(pointInside, pointsNeededToDetectPointsOnTerrain[2], 0.5f);
                    }
                    else
                    {
                        verticesToGenerateTerrain[1] = Vector3.Lerp(pointInside, pointsNeededToDetectPointsOnTerrain[3], 0.5f);
                    }
                }
                else
                {
                    verticesToGenerateTerrain[4] = Vector3.Lerp(pointInside, pointsNeededToDetectPointsOnTerrain[0], 0.5f);

                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(pointInside, pointsNeededToDetectPointsOnTerrain[2]) == 2)
                    {
                        verticesToGenerateTerrain[3] = Vector3.Lerp(pointInside, pointsNeededToDetectPointsOnTerrain[2], 0.5f);
                    }
                    else
                    {
                        verticesToGenerateTerrain[3] = Vector3.Lerp(pointInside, pointsNeededToDetectPointsOnTerrain[3], 0.5f);
                    }
                }
            }
            else if (commonPlanes == 1)
            {
                verticesToGenerateTerrain[2] = Vector3.Lerp(pointInside, pointsNeededToDetectPointsOnTerrain[1], 0.5f);
                pointForCalculateNormal = Vector3.Lerp(pointsNeededToDetectPointsOnTerrain[0], pointInside, 0.25f);
            }
        }

        Vector3 normal = (reverse ? center - pointForCalculateNormal : pointForCalculateNormal - center);

        GameObject terrainPart = new GameObject();
        terrainPart.name = "Mesh";

        MeshRenderer meshRenderer = terrainPart.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        MeshFilter meshFilter = terrainPart.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        mesh.vertices = verticesToGenerateTerrain;

        if (Vector3.Dot(-normal.normalized, Vector3.Cross(verticesToGenerateTerrain[1] - verticesToGenerateTerrain[0], verticesToGenerateTerrain[2] - verticesToGenerateTerrain[1]).normalized) < 0f)
        {
            verticesToGenerateTerrain = verticesToGenerateTerrain.Reverse().ToArray();
            mesh.vertices = verticesToGenerateTerrain;
        }


        int[] tris = new int[9]
        {
            0, 1, 3,
            1, 2, 3,
            3, 4, 0,
        };

        mesh.triangles = tris;

        Vector3[] normals = new Vector3[5]
        {
            -normal,
            -normal,
            -normal,
            -normal,
            -normal
        };

        mesh.normals = normals;

        Vector2[] uv = new Vector2[5]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
        };

        mesh.uv = uv;


        Color[] colors = new Color[5]
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.blue,
            Color.blue,
        };

        mesh.colors = colors;

        meshFilter.mesh = mesh;
        terrainPart.transform.SetParent(gameObject.transform);
    }
    #endregion

    #region FourPoint
    private void GenerateMeshForFourPoinsInside((Vector3, float)[,,] pointsWithDensity, bool reverse = false)
    {
        List<Vector3> pointsInside = new List<Vector3>();

        for (int x = 0; x < pointsWithDensity.GetLength(0); x++)
        {
            for (int y = 0; y < pointsWithDensity.GetLength(1); y++)
            {
                for (int z = 0; z < pointsWithDensity.GetLength(2); z++)
                {
                    if ((pointsWithDensity[x, y, z].Item2 > 0f && !reverse) || (!(pointsWithDensity[x, y, z].Item2 > 0f) && reverse))
                    {
                        pointsInside.Add(pointsWithDensity[x, y, z].Item1);
                    }
                }
            }
        }

        DrawSeparatePoints(pointsWithDensity, reverse, pointsInside);

        if (pointsInside.Count == 2)
        {
            GenerateMeshForTwoPointsNextToEachOter(pointsWithDensity, reverse, pointsInside);
        }
        else if (pointsInside.Count == 3)
        {
            GenerateMeshForThreePointsNextToEachOther(pointsWithDensity, pointsInside, reverse);
        }
        else if (pointsInside.Count == 4)
        {
            bool itsfourPointsCase3 = true;

            foreach (var point in pointsInside)
            {
                if (!pointsInside.Exists(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(point, x) == 0)
                 || !pointsInside.Exists(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(point, x) == 1)
                 || !pointsInside.Exists(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(point, x) == 2)
                 || !pointsInside.Exists(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(point, x) == 3))
                {
                    itsfourPointsCase3 = false;
                }
            }

            if (itsfourPointsCase3)
            {
                GenerateMeshForFourPointsCaseThree(pointsWithDensity, reverse, pointsInside);
            }
            else
            {
                FourConnectedPoints(pointsWithDensity, reverse, pointsInside);
            }
        }
    }

    private void FourConnectedPoints((Vector3, float)[,,] pointsWithDensity, bool reverse, List<Vector3> pointsInside)
    {
        bool itsFourPointsCase1 = true;

        foreach (var point in pointsInside)
        {
            if (pointsInside.Exists(t => t.x != pointsInside[0].x)
            && pointsInside.Exists(t => t.y != pointsInside[0].y)
            && pointsInside.Exists(t => t.z != pointsInside[0].z))
            {
                itsFourPointsCase1 = false;
            }
        }

        Vector3 thirdPoint = pointsInside.Find(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(x, pointsInside[0]) == 1);
        pointsInside.Remove(thirdPoint);
        pointsInside.Insert(2, thirdPoint);

        if (itsFourPointsCase1)
        {
            GenerateMeshForFourPointsCase1(pointsWithDensity, reverse, pointsInside);
        }
        else if (pointsInside.Exists(x => !pointsInside.Exists(y => TerrainGeneratorHelper.CalculateCommonPlanesCount(x, y) < 2)))
        {
            GenerateMeshForFourPointsCase2(pointsWithDensity, reverse, pointsInside);
        }
        else
        {
            GenerateMeshForFourPointsCase4(pointsWithDensity, reverse, pointsInside);
        }
    }

    private void GenerateMeshForFourPointsCase4((Vector3, float)[,,] pointsWithDensity, bool reverse, List<Vector3> pointsInside)
    {
        List<Vector3> pointsNeedsForVertices = new List<Vector3>();

        for (int x = 0; x < pointsWithDensity.GetLength(0); x++)
        {
            for (int y = 0; y < pointsWithDensity.GetLength(1); y++)
            {
                for (int z = 0; z < pointsWithDensity.GetLength(2); z++)
                {
                    if ((!(pointsWithDensity[x, y, z].Item2 > 0)))
                    {
                        int countWithPointsInside = 0;

                        foreach (var point in pointsInside)
                        {
                            countWithPointsInside += TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsWithDensity[x, y, z].Item1, point);
                        }

                        if (countWithPointsInside == 6)
                        {
                            pointsNeedsForVertices.Add(pointsWithDensity[x, y, z].Item1);
                        }
                    }
                }
            }
        }

        pointsNeedsForVertices.Add(new Vector3());
        pointsNeedsForVertices.Add(new Vector3());
        Debug.Log("case 4 4 count: " + pointsNeedsForVertices.Count);

        for (int x = 0; x < pointsWithDensity.GetLength(0); x++)
        {
            for (int y = 0; y < pointsWithDensity.GetLength(1); y++)
            {
                for (int z = 0; z < pointsWithDensity.GetLength(2); z++)
                {
                    if ((!(pointsWithDensity[x, y, z].Item2 > 0)) && !pointsNeedsForVertices.Contains(pointsWithDensity[x, y, z].Item1))
                    {
                        if (TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsWithDensity[x, y, z].Item1, pointsNeedsForVertices[0]) == 2)
                        {
                            pointsNeedsForVertices[2] = pointsWithDensity[x, y, z].Item1;
                        }
                        else
                        {
                            pointsNeedsForVertices[3] = pointsWithDensity[x, y, z].Item1;
                        }
                    }
                }
            }
        }

        List<Vector3> vertices = new List<Vector3>();

        Vector3 temporaryPoint = pointsInside.Find(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsNeedsForVertices[0], x) == 2
                                   && !pointsInside.Exists(y => TerrainGeneratorHelper.CalculateCommonPlanesCount(x, y) == 0));
        vertices.Add(Vector3.Lerp(temporaryPoint, pointsNeedsForVertices[0], 0.5f));
        temporaryPoint = pointsInside.Find(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsNeedsForVertices[2], x) == 2);
        vertices.Add(Vector3.Lerp(temporaryPoint, pointsNeedsForVertices[2], 0.5f));
        vertices.Add(Vector3.Lerp(temporaryPoint, pointsNeedsForVertices[1], 0.5f));
        temporaryPoint = pointsInside.Find(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsNeedsForVertices[2], x) == 0);
        Vector3 pointForNormal = temporaryPoint;
        Vector3 center = Vector3.Lerp(temporaryPoint, pointsNeedsForVertices[1], 0.5f);
        vertices.Add(Vector3.Lerp(temporaryPoint, pointsNeedsForVertices[1], 0.5f));
        temporaryPoint = pointsInside.Find(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsNeedsForVertices[3], x) == 2);

        vertices.Add(Vector3.Lerp(temporaryPoint, pointsNeedsForVertices[3], 0.5f));
        vertices.Add(Vector3.Lerp(temporaryPoint, pointsNeedsForVertices[0], 0.5f));

        Vector3 normal = (reverse ? center - pointForNormal : pointForNormal - center);

        if (Vector3.Dot(-normal.normalized, Vector3.Cross(vertices[5] - vertices[1], vertices[3] - vertices[5]).normalized) > 0f)
        {
            vertices.Reverse();
        }

        GameObject terrainPart = new GameObject();
        terrainPart.name = "Mesh";

        MeshRenderer meshRenderer = terrainPart.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        MeshFilter meshFilter = terrainPart.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();

        int[] tris = new int[12]
        {
                0, 1, 5,
                1, 2, 3,
                5, 1, 3,
                3, 4, 5,
        };

        mesh.triangles = tris;

        Vector3[] normals = new Vector3[6]
        {
            -normal,
            -normal,
            -normal,
            -normal,
            -normal,
            -normal,
        };

        mesh.normals = normals;

        Vector2[] uv = new Vector2[6]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        mesh.uv = uv;

        Color[] colors = new Color[6]
        {
            Color.red,
            Color.green,
            Color.green,
            Color.green,
            Color.blue,
            Color.blue,
        };

        mesh.colors = colors;

        meshFilter.mesh = mesh;
        terrainPart.transform.SetParent(gameObject.transform);
    }

    private void GenerateMeshForFourPointsCase2((Vector3, float)[,,] pointsWithDensity, bool reverse, List<Vector3> pointsInside)
    {
        List<Vector3> pointsNeededForVertices = new List<Vector3>();
        Vector3 center = new Vector3();

        for (int x = 0; x < pointsWithDensity.GetLength(0); x++)
        {
            for (int y = 0; y < pointsWithDensity.GetLength(1); y++)
            {
                for (int z = 0; z < pointsWithDensity.GetLength(2); z++)
                {
                    if (!(pointsWithDensity[x, y, z].Item2 > 0f) && pointsInside.Exists(t => TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsWithDensity[x, y, z].Item1, t) == 2))
                    {
                        pointsNeededForVertices.Add(pointsWithDensity[x, y, z].Item1);
                    }

                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsWithDensity[x, y, z].Item1, pointsInside[0]) == 0)
                    {
                        center = Vector3.Lerp(pointsWithDensity[x, y, z].Item1, pointsInside[0], 0.5f);
                    }
                }
            }
        }

        List<Vector3> vertices = new List<Vector3>();

        Vector3 currentPoint = pointsInside.Find(t => pointsInside.Exists(y => TerrainGeneratorHelper.CalculateCommonPlanesCount(t, y) == 1));
        Vector3 oldPoint = pointsNeededForVertices.Find(t => TerrainGeneratorHelper.CalculateCommonPlanesCount(t, currentPoint) == 2);

        for (int i = 0; i < 3; i++)
        {
            Vector3 pointToFind = pointsNeededForVertices.Find(t => TerrainGeneratorHelper.CalculateCommonPlanesCount(t, currentPoint) == 2 && t != oldPoint);
            vertices.Add(Vector3.Lerp(currentPoint, pointToFind, 0.5f));
            oldPoint = currentPoint;
            currentPoint = pointToFind;
            pointToFind = pointsInside.Find(t => TerrainGeneratorHelper.CalculateCommonPlanesCount(t, currentPoint) == 2 && t != oldPoint);
            vertices.Add(Vector3.Lerp(currentPoint, pointToFind, 0.5f));
            oldPoint = currentPoint;
            currentPoint = pointToFind;
        }

        Vector3 pointForNormal = pointsInside.Find(x => !pointsInside.Exists(y => TerrainGeneratorHelper.CalculateCommonPlanesCount(x, y) < 2));
        Vector3 normal = (reverse ? center - pointForNormal : pointForNormal - center);

        if (Vector3.Dot(-normal.normalized, Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[1]).normalized) < 0f)
        {
            vertices.Reverse();
        }

        GameObject terrainPart = new GameObject();
        terrainPart.name = "Mesh";

        MeshRenderer meshRenderer = terrainPart.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        MeshFilter meshFilter = terrainPart.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();

        int[] tris = new int[12]
        {
                0, 1, 2,
                2, 3, 5,
                3, 4, 5,
                5, 0, 2,
        };

        mesh.triangles = tris;

        Vector3[] normals = new Vector3[6]
        {
            -normal,
            -normal,
            -normal,
            -normal,
            -normal,
            -normal,
        };

        mesh.normals = normals;

        Vector2[] uv = new Vector2[6]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        mesh.uv = uv;

        Color[] colors = new Color[6]
        {
            Color.red,
            Color.green,
            Color.green,
            Color.green,
            Color.blue,
            Color.blue,
        };

        mesh.colors = colors;

        meshFilter.mesh = mesh;
        terrainPart.transform.SetParent(gameObject.transform);
    }

    private void GenerateMeshForFourPointsCase1((Vector3, float)[,,] pointsWithDensity, bool reverse, List<Vector3> pointsInside)
    {
        Vector3[] vertices = new Vector3[4];

        for (int x = 0; x < pointsWithDensity.GetLength(0); x++)
        {
            for (int y = 0; y < pointsWithDensity.GetLength(1); y++)
            {
                for (int z = 0; z < pointsWithDensity.GetLength(2); z++)
                {
                    if (!pointsInside.Contains(pointsWithDensity[x, y, z].Item1))
                    {
                        int indexInVertices = pointsInside.FindIndex(t => TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsWithDensity[x, y, z].Item1, t) == 2);
                        vertices[indexInVertices] = Vector3.Lerp(pointsWithDensity[x, y, z].Item1, pointsInside[indexInVertices], 0.5f);
                    }
                }
            }
        }

        Vector3 center = Vector3.Lerp(vertices[0], vertices[2], 0.5f);
        Vector3 normal = (reverse ? center - Vector3.Lerp(pointsInside[0], pointsInside[2], 0.5f) : Vector3.Lerp(pointsInside[0], pointsInside[2], 0.5f) - center);

        if (Vector3.Dot(-normal.normalized, Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[1]).normalized) < 0f)
        {
            vertices = vertices.Reverse().ToArray();
        }

        GameObject terrainPart = new GameObject();
        terrainPart.name = "Mesh";

        MeshRenderer meshRenderer = terrainPart.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        MeshFilter meshFilter = terrainPart.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
                0, 1, 2,
                0, 2, 3
        };

        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -normal,
            -normal,
            -normal,
            -normal,
        };

        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        mesh.uv = uv;


        Color[] colors = new Color[4]
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.blue,
        };

        mesh.colors = colors;

        meshFilter.mesh = mesh;
        terrainPart.transform.SetParent(gameObject.transform);
    }

    private void GenerateMeshForFourPointsCaseThree((Vector3, float)[,,] pointsWithDensity, bool reverse, List<Vector3> pointsInside)
    {
        Vector3 pointNextToFirstPoint = pointsInside.Find(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(pointsInside[0], x) == 2);

        GenerateMeshForTwoPointsNextToEachOter(pointsWithDensity, false, new List<Vector3> { pointsInside[0], pointNextToFirstPoint });
        pointsInside.Remove(pointsInside[0]);
        pointsInside.Remove(pointNextToFirstPoint);
        GenerateMeshForTwoPointsNextToEachOter(pointsWithDensity, false, pointsInside);
    }

    #endregion
    private void DrawSeparatePoints((Vector3, float)[,,] pointsWithDensity, bool reverse, List<Vector3> pointsInside)
    {
        List<Vector3> separatedPointsInside = new List<Vector3>();

        for (int i = pointsInside.Count - 1; i >= 0; i--)
        {
            if (!pointsInside.Exists(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(x, pointsInside[i]) > 1 && TerrainGeneratorHelper.CalculateCommonPlanesCount(x, pointsInside[i]) != 3))
            {
                separatedPointsInside.Add(pointsInside[i]);
                pointsInside.RemoveAt(i);
            }
        }

        foreach (var point in separatedPointsInside)
        {
            (Vector3, float)[,,] separatedPointsWithDensity = new (Vector3, float)[2, 2, 2];

            for (int x = 0; x < pointsWithDensity.GetLength(0); x++)
            {
                for (int y = 0; y < pointsWithDensity.GetLength(1); y++)
                {
                    for (int z = 0; z < pointsWithDensity.GetLength(2); z++)
                    {
                        bool pointInside = point.x == pointsWithDensity[x, y, z].Item1.x && point.y == pointsWithDensity[x, y, z].Item1.y && point.z == pointsWithDensity[x, y, z].Item1.z;
                        separatedPointsWithDensity[x, y, z] = (pointsWithDensity[x, y, z].Item1, Convert.ToInt32(pointInside));
                    }
                }
            }

            GenerateMeshForOnePoinsInside(separatedPointsWithDensity, reverse);
        }
    }
}
