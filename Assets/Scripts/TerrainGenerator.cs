using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private DensityGenerator densityGenerator;
    [SerializeField] private MarchingCubes marchingCubes;
    [SerializeField] private GameObject meshGameObject;
    [SerializeField] private VoxelsOctreeGenerator voxelsOctree;
    [SerializeField] private CameraMover cameraMover;
    [SerializeField] private SettingsController settingsUI;

    [Header("Terrain settings")]
    [SerializeField] private Vector3 startPoint;
    [SerializeField] private int sizeMultiplier;

    [Header("Density settings")]
    [SerializeField] private Vector3 densitySize;
    [SerializeField] private int pointsSpace = 1;
    [SerializeField] [Range(1, 10)] private int octavesCount = 1;
    [SerializeField] private float noiseScale;
    [SerializeField] private float noiseWeight;
    [SerializeField] private float amplitudeMultiplier;
    [SerializeField] private float frequencyMultiplier;
    [SerializeField] private int seed;

    [Header("Debug settings")]
    [SerializeField] private bool enableGizmosDensityDrawer;
    [SerializeField] private DensityGizmosDrawer densityGizmosDrawer;

    [Header("Big terrain settings")]
    [SerializeField] private int xCount = 1;
    [SerializeField] private int zCount = 1;
    [SerializeField] private bool bigTerrainEnabled = false;

    public static bool wasUpdate = false;

    public void CreateTerrain(int size, int noiseScale, int noiseWeight)
    {
        this.noiseScale = noiseScale;
        this.noiseWeight = noiseWeight;
        xCount = size;
        zCount = size;
        bigTerrainEnabled = true;
        this.enabled = true;
    }

    public void OnEnable()
    {
        if (bigTerrainEnabled)
        {
            DestroyEarlierMesh();
            StartCoroutine(GenerateBigTerrainCoroutine());
        }
    }

    private IEnumerator GenerateBigTerrainCoroutine()
    {
        yield return new WaitForEndOfFrame();

        float startPointY = startPoint.y;
        startPoint = new Vector3(0, startPointY, 0);
        Vector3 fullSize = new Vector3((densitySize.x - 1) * xCount, densitySize.y, (densitySize.z - 1) * zCount);
        voxelsOctree.SetDensitySize(startPoint, fullSize);
        cameraMover.OnTerrainGeneration(startPoint, fullSize);

        int iteration = 0;
        int lastIteration = xCount * zCount;

        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < zCount; j++)
            {
                startPoint = new Vector3((densitySize.x - 1) * i, startPointY, (densitySize.z - 1) * j);
                GenerateTerrain();
                settingsUI.OnTerrainPartGenerated((float)(iteration-1) / (float)lastIteration);
                iteration++;
                yield return null;
            }
        }
       
        startPoint = new Vector3(0, startPointY, 0);
        GenerateTerrain();

        yield return new WaitForEndOfFrame();
        bigTerrainEnabled = false;
        settingsUI.OnTerrainGenerated();
        this.enabled = false;
    }

    private void Update()
    {
        if (wasUpdate)
        {
            wasUpdate = false;
        }
    }

    private void OnValidate()
    {
        wasUpdate = true;
    }

    private void GenerateTerrain()
    {
        if (densitySize.x > 0 && densitySize.y > 0 && densitySize.z > 0)
        {
            Stopwatch s = new Stopwatch();
            s.Restart();
            s.Start();

            startPoint = new Vector3((int)startPoint.x, (int)startPoint.y, (int)startPoint.z);
            densitySize = new Vector3((int)densitySize.x, (int)densitySize.y, (int)densitySize.z);

            Vector4[] density = densityGenerator.GetDensity(
                densitySize, pointsSpace, startPoint, noiseScale, noiseWeight, octavesCount,
                amplitudeMultiplier, frequencyMultiplier, seed, sizeMultiplier);

            Vector4[,,] convertedDensity = ConvertArrayOfVector4To3DimensionalArray(density, densitySize * pointsSpace);

            DrawGizmosIfEnable(density);

            OctreeElement root = voxelsOctree.GetOctreeVoxel(convertedDensity);

            if (root != null)
            {
                marchingCubes.GenerateMesh(root, meshGameObject);
            }
            //marchingCubes.GenerateMesh(convertedDensity, meshGameObject);

            s.Stop();
            //Debug.Log("ooo " + s.Elapsed.TotalMilliseconds);
        }
    }

    private void DrawGizmosIfEnable(Vector4[] density)
    {
        if (enableGizmosDensityDrawer)
        {
            densityGizmosDrawer.SetPoints(density);
        }
        else
        {
            densityGizmosDrawer.ClearPoints();
        }
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
