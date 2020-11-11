using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] DensityGeneratorFromComputeShader densityGenerator;
    [SerializeField] MarchingCubes marchingCubes;
    [SerializeField] GameObject meshGameObject;

    [Header("Terrain settings")]
    [SerializeField] private Vector3 startPoint;

    [Header("Density settings")]
    [SerializeField] private Vector3 densitySize;
    [SerializeField] private int pointsSpace = 1;
    [SerializeField] [Range(1, 10)] private int octavesCount = 1;
    [SerializeField] private float noiseScale;
    [SerializeField] private float noiseWeight;

    [Header("Debug settings")]
    [SerializeField] bool enableGizmosDensityDrawer;
    [SerializeField] DensityGizmosDrawer densityGizmosDrawer;

    public static bool wasUpdate = false;

    public void Start()
    {
        GenerateTerrain();
    }

    private void Update()
    {
        if (wasUpdate)
        {
            wasUpdate = false;
            GenerateTerrain();
        }
    }

    private void OnValidate()
    {
        wasUpdate = true;
    }

    private void GenerateTerrain()
    {
        //Stopwatch s = new Stopwatch();
        //s.Start();

        startPoint = new Vector3((int)startPoint.x, (int)startPoint.y, (int)startPoint.z);
        densitySize = new Vector3((int)densitySize.x, (int)densitySize.y, (int)densitySize.z); 
        Vector4[] density = densityGenerator.GetDensity(
            densitySize, pointsSpace, startPoint, noiseScale, noiseWeight, octavesCount);
        Vector4[,,] density1 = ConvertArrayOfVector4To3DimensionalArray(density, densitySize);

        //Print3DimensionalArray(density1);

        if (enableGizmosDensityDrawer)
        {
            densityGizmosDrawer.SetPoints(density);
        }
        else
        {
            densityGizmosDrawer.ClearPoints();
        }

        //s.Stop();
        //UnityEngine.Debug.Log(s.ElapsedMilliseconds);
        //s.Reset();
        //s.Start();

        DestroyEarlierMesh();
        marchingCubes.GenerateMesh
            (density1, meshGameObject);

        //s.Stop();
        //UnityEngine.Debug.Log(s.ElapsedMilliseconds);
        //s.Reset();
    }

    private static void Print3DimensionalArray(Vector4[,,] density1)
    {
        for (int x = 0; x < density1.GetLength(0); x++)
        {
            for (int y = 0; y < density1.GetLength(1); y++)
            {
                for (int z = 0; z < density1.GetLength(2); z++)
                {
                    UnityEngine.Debug.Log(density1[x, y, z]);
                }
            }
        }
    }

    private void DestroyEarlierMesh()
    {
        for (int i = meshGameObject.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(meshGameObject.transform.GetChild(i).gameObject);
        }
    }

    private Vector4[,,] ConvertArrayOfVector4To3DimensionalArray(Vector4[] array, Vector3 arraySize)
    {
        Vector4[,,] threeDimensionalArray = new Vector4[(int)arraySize.x, (int)arraySize.y, (int)arraySize.z];

        int arrayElement = 0;

        for (int z = 0; z < (int)arraySize.z; z++)
        {
            for (int y = 0; y < (int)arraySize.y; y++)
            {
                for (int x = 0; x < (int)arraySize.x; x++)
                {
                    threeDimensionalArray[x, y, z] = array[arrayElement];
                    arrayElement++;
                }
            }
        }

        return threeDimensionalArray;
    }
}
