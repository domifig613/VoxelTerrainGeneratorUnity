using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoxelsOctreeGenerator : MonoBehaviour
{
    private OctreeElement root;

    public OctreeElement GetOctreeVoxel(Vector4[,,] density)
    {
        if (density.GetLength(0) > 1 && density.GetLength(1) > 1 && density.GetLength(2) > 1)
        {
            int treeSize = GetTreeSize(density.GetLength(0) - 1, density.GetLength(1) - 1, density.GetLength(2) - 1);
            Debug.Log("treeSize: " + treeSize);

            List<OctreeElement> branchesWithVoxels = InitVoxels(density);

            float xDifferent = density[1, 0, 0].x - density[0, 0, 0].x;
            float yDifferent = density[0, 1, 0].y - density[0, 0, 0].y;
            float zDifferent = density[0, 0, 1].z - density[0, 0, 0].z;

            Vector3 size = new Vector3(density.GetLength(0), density.GetLength(1), density.GetLength(2));
            Vector3 startPoint = new Vector3(density[0, 0, 0].x, density[0, 0, 0].y, density[0, 0, 0].z);

            List<OctreeElement> branchesChildren = branchesWithVoxels;
            List<OctreeElement> biggerBranches = new List<OctreeElement>();

            while (branchesChildren.Count > 1)
            {
                FillNextLevelOfTree(branchesChildren, biggerBranches, xDifferent,
                    yDifferent, zDifferent, size, startPoint);
                xDifferent *= 2;
                yDifferent *= 2;
                zDifferent *= 2;

                branchesChildren = biggerBranches;
                biggerBranches = new List<OctreeElement>();
            }

            root = branchesChildren[0];
            CalculateDensity();
            return root;
        }

        return null;
    }

    private void CalculateDensity()
    {
        List<OctreeElement> octreeBranches = root.NextElements.ToList();
        int voxelCount = 0;
        float density = 0;

        while(octreeBranches.Count != 0)
        {
            int count = octreeBranches.Count;

            for (int i = count -1 ; i >= 0; i--)
            {
                if (octreeBranches[i] != null)
                {
                    OctreeElement[] nextElements = octreeBranches[i].NextElements;

                    if (nextElements == null)
                    {
                        Voxel voxel = octreeBranches[i] as Voxel;
                        voxel.SetType();
                        voxelCount++;
                        density += voxel.GetSumDensity();
                    }
                    else
                    {
                        octreeBranches.AddRange(nextElements);
                    }
                }

                octreeBranches.RemoveAt(i);
            }
        }

        Debug.Log("voxelCount " + voxelCount);
        Debug.Log("denisty " + density / voxelCount);
    }

    private void FillNextLevelOfTree(List<OctreeElement> branchesChildren, List<OctreeElement> biggerBranches, float xDifferent, float yDifferent, float zDifferent, Vector3 size, Vector3 startPoint)
    {
        List<int> focusPointsX = GetSpecificPoints((int)size.x, xDifferent, startPoint.x);
        List<int> focusPointsY = GetSpecificPoints((int)size.y, yDifferent, startPoint.y);
        List<int> focusPointsZ = GetSpecificPoints((int)size.z, zDifferent, startPoint.z);

        //foreach (var item in focusPointsX)
        //{
        //    Debug.Log(item);
        //}

        foreach (var itemX in focusPointsX)
        {
            foreach (var itemY in focusPointsY)
            {
                foreach (var itemZ in focusPointsZ)
                {
                    List<OctreeElement> branchesToAdd = new List<OctreeElement>();

                    for (int i = branchesChildren.Count - 1; i >= 0; i--)
                    {
                        if (branchesChildren[i].IsPointInFocusPoint(new Vector3(itemX, itemY, itemZ),
                            new Vector3(xDifferent*2, yDifferent*2, zDifferent*2)))
                        {
                            branchesToAdd.Add(branchesChildren[i]);
                            branchesChildren.RemoveAt(i);
                        }
                    }

                    if (branchesToAdd.Count > 0)
                    {
                        OctreeBranch branch = new OctreeBranch(branchesToAdd.ToArray());
                        biggerBranches.Add(branch);
                    }
                }
            }
        }
  
        if (branchesChildren.Count > 0)
        {
            Debug.LogError("old branches > 0 count" + branchesChildren.Count);
        }
    }

    public void TestSpecific()
    {
        int b = 50;

        GetSpecificPoints(b, 1, 0);
        GetSpecificPoints(b, 2, 0);
        GetSpecificPoints(b, 4, 0);
        GetSpecificPoints(b, 8, 0);
    }

    private List<int> GetSpecificPoints(int densityLenght, float different, float startPosition)
    {
        List<int> focusPoints = new List<int>();

        int count = densityLenght / 4;

        for (int i = 1; i <= count; i++)
        {
            focusPoints.Add((int)((startPosition + different * i * 4) - (2*different)));
        }

        if (densityLenght % 4 != 0)
        {
            if (focusPoints.Count != 0)
            {
                focusPoints.Add((int)(focusPoints[focusPoints.Count - 1] + (4 * different)));
            }
            else
            {
                focusPoints.Add((int)(startPosition + different * 2));
            }
        }

        //foreach (var item in focusPoints)
        //{
        //    Debug.Log("xx " + item);
        //}

        //Debug.Log("//");

        return focusPoints;
    }

    private (int, int) CalculateSizeForDimension(Vector4[,,] density, int dimension, int treeSize)
    {
        int sizeDensity = density.GetLength(dimension) - 1;
        int firstElementSize = (int)Mathf.Pow(2, treeSize - 1);
        int secondElementSize = sizeDensity - firstElementSize;
        return (firstElementSize, secondElementSize);
    }

    private int GetTreeSize(int xSize, int ySize, int zSize)
    {
        int maxDensitySize = Mathf.Max(xSize, ySize, zSize);
        int treeSize = 1;

        while (Mathf.Pow(2, treeSize) < maxDensitySize)
        {
            treeSize++;
        }

        return treeSize;
    }

    private List<OctreeElement> InitVoxels(Vector4[,,] density)
    {
        List<OctreeElement> octreeElementsWithVoxels = new List<OctreeElement>();

        int xSize = density.GetLength(0) - 1;
        int ySize = density.GetLength(1) - 1;
        int zSize = density.GetLength(2) - 1;

        //int v = 0;

        for (int currentX = 0; currentX < xSize; currentX += 2)
        {
            for (int currentY = 0; currentY < ySize; currentY += 2)
            {
                for (int currentZ = 0; currentZ < zSize; currentZ += 2)
                {
                    OctreeElement[] octreeElements = CreateVoxelsLeaf(density, xSize, ySize, zSize, currentX, currentY, currentZ);
                    OctreeBranch branchWithVoxels = new OctreeBranch(octreeElements,
                        (currentX, currentX + 2), (currentY, currentY + 2), (currentZ, currentZ + 2)); ;

                    octreeElementsWithVoxels.Add(branchWithVoxels);
                    //v++;
                }
            }
        }

        //Debug.Log(v);

        //foreach (var item in octreeElementsWithVoxels)
        //{
        //    if(!item.NextElements.ToList().Exists(x => x != null))
        //    {
        //        Debug.LogError("only null");
        //    }
        //}

        return octreeElementsWithVoxels;
    }

    private OctreeElement[] CreateVoxelsLeaf(Vector4[,,] density, int xSize, int ySize, int zSize, int currentX, int currentY, int currentZ)
    {
        OctreeElement[] octreeElements = new OctreeElement[8];
        int elements = 0;

        for (int x = currentX; x < currentX + 2; x++)
        {
            for (int y = currentY; y < currentY + 2; y++)
            {
                for (int z = currentZ; z < currentZ + 2; z++)
                {
                    if (x <= xSize - 1 && y <= ySize - 1 && z <= zSize - 1)
                    {
                        Voxel voxel = new Voxel((x, x+1),(y,y+1),(z, z+1));
                        float[,,] densityForVoxel = GetDensityForVoxel(density, new Vector3(x, y, z));
                        voxel.SetDensity(densityForVoxel);
                        octreeElements[elements] = voxel;
                    }
                    else
                    {
                        octreeElements[elements] = null;
                    }

                    elements++;
                }
            }
        }

        return octreeElements;
    }

    private float[,,] GetDensityForVoxel(Vector4[,,] density, Vector3 position)
    {
        float[,,] densityForVoxel = new float[2, 2, 2];

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    densityForVoxel[x, y, z] = density[(int)position.x + x, (int)position.y + y, (int)position.z + z].w;
                }
            }
        }

        return densityForVoxel;
    }
}
