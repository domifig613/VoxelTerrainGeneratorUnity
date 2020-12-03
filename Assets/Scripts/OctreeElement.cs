using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OctreeElement
{
    private OctreeElement previousElement;
    protected OctreeElement[] nextElements;
    protected (int, int) xRange; //first min, second max
    protected (int, int) yRange;
    protected (int, int) zRange;

    private bool leaf;

    public OctreeElement PreviousElement => previousElement;
    public OctreeElement[] NextElements => nextElements;

    public (int, int) XRange => xRange;
    public (int, int) YRange => yRange;
    public (int, int) ZRange => zRange;

    public OctreeElement(bool leaf)
    {
        this.leaf = leaf;
    }

    public OctreeElement(OctreeElement previousElement, bool leaf)
    {
        this.previousElement = previousElement;
        this.leaf = leaf;
    }

    public OctreeElement(OctreeElement[] nextElements)
    {
        this.leaf = false;
        this.nextElements = nextElements;
    }

    public bool IsPointInFocusPoint(Vector3 point, Vector3 different)
    {
        return    ((xRange.Item1 >= point.x - different.x && xRange.Item1 <= point.x + different.x)
                && (xRange.Item2 >= point.x - different.x && xRange.Item2 <= point.x + different.x)
                && (yRange.Item1 >= point.y - different.y && yRange.Item1 <= point.y + different.y)
                && (yRange.Item2 >= point.y - different.y && yRange.Item2 <= point.y + different.y)
                && (zRange.Item1 >= point.z - different.z && zRange.Item1 <= point.z + different.z)
                && (zRange.Item2 >= point.z - different.z && zRange.Item2 <= point.z + different.z));
    }

    public List<VoxelType> GetTypeNextTo(OctreeElement seekersElement, (int,int) rangeX, (int,int) rangeY, (int,int) rangeZ)
    {
        List<VoxelType> voxelsTypes = new List<VoxelType>();

        int f = 0;

        for (int x = rangeX.Item1 - 1; x < rangeX.Item2 +1 ; x++)
        {
            for (int y = rangeY.Item1 - 1; y < rangeY.Item2 + 1; y++)
            {
                for (int z = rangeZ.Item1 - 1; z < rangeZ.Item2 + 1; z++)
                {
                    bool elementFilled = false;

                    for (int i = 0; i < nextElements.Length; i++)
                    {
                        if (nextElements[i] != null && nextElements[i] != seekersElement)
                        {
                            if(nextElements[i].ElementInRange((x,x+1),(y,y+1),(z,z+1)))
                            {
                                voxelsTypes.Add((nextElements[i] as Voxel).Type);
                                elementFilled = true;
                            }
                        }
                    }

                    if (!elementFilled && previousElement.ElementInRange((x, x + 1), (y, y + 1), (z, z + 1)))
                    {
                        voxelsTypes.Add(previousElement.GetTypeInSpecificPosition(this, x, y, z));
                    }
                }
            }
        }

        //Debug.Log("ggg " + voxelsTypes.FindAll(x => x == VoxelType.Undefined).Count);

        return voxelsTypes;
    }

    private VoxelType GetTypeInSpecificPosition(OctreeElement seekersElement, int x, int y, int z)
    {
        if(this is Voxel)
        {
            return (this as Voxel).Type;
        }
        else
        {
            for (int i = 0; i < nextElements.Length; i++)
            {
                if (nextElements[i] != null && nextElements[i] != seekersElement)
                {
                    if (nextElements[i].ElementInRange((x, x + 1), (y, y + 1), (z, z + 1)))
                    {
                        return nextElements[i].GetTypeInSpecificPosition(this, x, y ,z);
                    }
                }
            }
        }

        return VoxelType.Undefined;
    }

    public bool ElementInRange((int, int) rangeX, (int, int) rangeY, (int, int) rangeZ)
    {
        if ((rangeX.Item1 >= xRange.Item1 && rangeX.Item1 <= xRange.Item2)
         && (rangeX.Item2 >= xRange.Item1 && rangeX.Item2 <= xRange.Item2)
         && (rangeY.Item1 >= yRange.Item1 && rangeY.Item1 <= yRange.Item2)
         && (rangeY.Item2 >= yRange.Item1 && rangeY.Item2 <= yRange.Item2)
         && (rangeZ.Item1 >= zRange.Item1 && rangeZ.Item1 <= zRange.Item2)
         && (rangeZ.Item2 >= zRange.Item1 && rangeZ.Item2 <= zRange.Item2))
        {
            return true;
        }

        return false;
    }

    public void SetPrevious(OctreeBranch previousElement)
    {
        this.previousElement = previousElement;
    }
}
