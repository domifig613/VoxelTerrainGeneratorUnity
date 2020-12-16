using System.Collections.Generic;
using UnityEngine;

public class Voxel : OctreeElement
{
    private float[,,] density;
    private VoxelType type;
    private VoxelPositionType positionType;
    private bool drawable = false;
    private int stage = 0;
    private List<Vector3[]> vertices; //only if drawable

    public bool Edge = false;
    private Vector2 positionInWorld;

    public float[,,] Density => density;
    public bool Drawable => drawable;
    public VoxelType Type => type;
    public List<Vector3[]> Vertices => vertices;


    private static Vector2[,,] uvPosition = new Vector2[,,]
    {
        {
          {new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.6f), new Vector2(0.6f, 0.6f)}, //rock
          {new Vector2(0.5f, 0.5f), new Vector2(0.6f, 0.5f), new Vector2(0.6f, 0.6f)}
        },
        {
          {new Vector2(0.0f, 0.0f), new Vector2(0.4f, 0.0f), new Vector2(0.4f, 0.45f)}, //water
          {new Vector2(0.0f, 0.0f), new Vector2(0.4f, 0.0f), new Vector2(0.4f, 0.45f)}
        },
        {
          {new Vector2(0.0f, 0.6f), new Vector2(0.1f, 0.6f), new Vector2(0.1f, 0.7f)}, //grass
          {new Vector2(0.0f, 0.6f), new Vector2(0.0f, 0.7f), new Vector2(0.1f, 0.7f)}
        },
        {
          {new Vector2(0.5f, 0.0f), new Vector2(0.5f, 0.1f), new Vector2(0.6f, 0.1f)}, //sand
          {new Vector2(0.5f, 0.0f), new Vector2(0.6f, 0.0f), new Vector2(0.6f, 0.1f)}
        },
    };


    public Vector2[] GetUVPositionForVoxel()
    {
        Vector2[] uv = new Vector2[3];

        int c = ((int)type != 7) ? (int)type - 2 : (int)type - 6;

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = uvPosition[c, UnityEngine.Random.Range(0, 2), i];
        }

        return uv;
    }

    public Voxel((int, int) xRange, (int, int) yRange, (int, int) zRange, Vector2 positionInWorld) : base(true)
    {
        type = VoxelType.Undefined;

        base.xRange = xRange;
        base.yRange = yRange;
        base.zRange = zRange;

        this.positionInWorld = positionInWorld;
    }

    public void SetDensity(float[,,] density)
    {
        this.density = density;
    }

    public float GetSumDensity()
    {
        float sumDensity = 0;

        for (int i = 0; i < density.GetLength(0); i++)
        {
            for (int j = 0; j < density.GetLength(1); j++)
            {
                for (int k = 0; k < density.GetLength(2); k++)
                {
                    sumDensity += density[i, j, k];
                }
            }
        }

        return sumDensity;
    }

    public void SetType()
    {
        if (type == VoxelType.Undefined)
        {
            DefineVoxelType();
        }
        else
        {
            stage++;
            ChangeVoxelType();
        }
    }

    private void ChangeVoxelType()
    {

        if (stage == 1)
        {
            if (positionType == VoxelPositionType.Horizontal && GetTypesNextTo().FindAll(x => x == VoxelType.Soil).Count > 6)
            {
                var rand = Random.Range(0f, 1f);

                if (rand < 0.0025f)
                {
                    type = VoxelType.Water;
                }
            }
        }
        else if (stage == 2)
        {
            if (type == VoxelType.Soil)
            {
                if (NoiseHelper.GetColor1(positionInWorld.x, positionInWorld.y).r < 0.2f)
                {
                    type = VoxelType.Water;
                }
            }
        }
        else if (stage == 3)
        {
            if (type == VoxelType.Soil)
            {
                List<VoxelType> typesNextTo = GetTypesNextTo();

                if (typesNextTo.Contains(VoxelType.Water) && NoiseHelper.GetColor2(positionInWorld.x, positionInWorld.y).r < 0.3f)
                {
                    type = VoxelType.Water;
                }
            }
        }
        else if (stage == 4)
        {
            if (type == VoxelType.Soil && GetTypesNextTo().FindAll(x => x == VoxelType.Water).Count > 1)
            {
                type = VoxelType.Sand;
            }
        }
        else if (stage == 6)
        {
            if (type == VoxelType.Soil && GetTypesNextTo().Contains(VoxelType.Sand) 
                && NoiseHelper.GetColor2(positionInWorld.x, positionInWorld.y).r < 0.5f)
            {
                type = VoxelType.Sand;
            }
        }
    }

    private List<VoxelType> GetTypesNextTo()
    {
        return PreviousElement.GetTypeNextTo(this, xRange, yRange, zRange);

    }

    private void DefineVoxelType()
    {
        (int densityMinusCount, int densityPlusCount) = CalculateDensityCountPlusAndMinus();

        if (densityPlusCount == 0 && densityMinusCount > 0)
        {
            type = VoxelType.Air;
        }
        else
        {
            if (densityPlusCount > 0 && densityMinusCount > 0)
            {
                drawable = true;
                SetTrianglesAndPositionType();

                type = positionType == VoxelPositionType.Horizontal ? VoxelType.Soil : VoxelType.Rock;
            }
        }
    }

    private void SetTrianglesAndPositionType()
    {
        vertices = MarchingCubes.GetVertices(this,
           VertexesCaseProvider.GetVerticesEdgesIndexes(VertexesCaseProvider.ReadCaseForCube(density, 0f)), 0f);

        List<float> normalsDirection = new List<float>();

        foreach (var triangle in vertices)
        {
            normalsDirection.Add(Vector3.Dot((Vector3.Cross(triangle[1] - triangle[0], triangle[2] - triangle[0])).normalized, Vector3.up));
        }

        positionType = VoxelPositionType.Unspecified;

        foreach (var normalDiretion in normalsDirection)
        {
            if (normalDiretion > 0.93f || normalDiretion < -0.93f)
            {
                positionType = VoxelPositionType.Horizontal;
            }
            else if (normalDiretion < 0.2 && normalDiretion > -0.2)
            {
                positionType = VoxelPositionType.Vertical;
            }
        }
    }

    private (int, int) CalculateDensityCountPlusAndMinus()
    {
        int densityPlusCount = 0;
        int densityMinusCount = 0;

        for (int i = 0; i < density.GetLength(0); i++)
        {
            for (int j = 0; j < density.GetLength(1); j++)
            {
                for (int k = 0; k < density.GetLength(2); k++)
                {
                    if (density[i, j, k] >= 0)
                    {
                        densityPlusCount++;
                    }
                    else if (density[i, j, k] < 0)
                    {
                        densityMinusCount++;
                    }
                }
            }
        }

        return (densityMinusCount, densityPlusCount);
    }

    public Vector3 GetRange(int x, int y, int z)
    {
        return new Vector3(x == 1 ? xRange.Item2 : xRange.Item1,
                           y == 1 ? yRange.Item2 : yRange.Item1,
                           z == 1 ? zRange.Item2 : zRange.Item1);
    }
}
