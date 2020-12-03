using UnityEngine;

public class Voxel : OctreeElement
{
    private float[,,] density;
    private VoxelType type;
    private bool drawable = false;

    public float[,,] Density => density;
    public bool Drawable => drawable;
    public VoxelType Type => type;

    public Voxel((int, int) xRange, (int, int) yRange, (int, int) zRange) : base(true)
    {
        type = VoxelType.Undefined;

        base.xRange = xRange;
        base.yRange = yRange;
        base.zRange = zRange;
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
        bool densityPlus = false;
        bool densityMinus = false;

        for (int i = 0; i < density.GetLength(0); i++)
        {
            for (int j = 0; j < density.GetLength(1); j++)
            {
                for (int k = 0; k < density.GetLength(2); k++)
                {
                    if (density[i, j, k] >= 0)
                    {
                        densityPlus = true;
                    }
                    else if (density[i, j, k] < 0)
                    {
                        densityMinus = true;
                    }
                }
            }
        }

        if (!densityPlus && densityMinus)
        {
            type = VoxelType.Air;
        }
        else
        {
            PreviousElement.GetTypeNextTo(this, xRange, yRange, zRange);
            type = densityPlus && densityMinus ? VoxelType.Rock : VoxelType.Air;
        }

        if (densityPlus && densityMinus)
        {
            drawable = true;
        }
    }

    public Vector3 GetRange(int x, int y, int z)
    {
        return new Vector3(x == 1 ? xRange.Item2 : xRange.Item1,
                           y == 1 ? yRange.Item2 : yRange.Item1,
                           z == 1 ? zRange.Item2 : zRange.Item1);
    }
}
