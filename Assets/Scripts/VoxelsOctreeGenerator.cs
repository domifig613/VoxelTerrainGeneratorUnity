using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class VoxelsOctreeGenerator : MonoBehaviour
{
    private OctreeElement root;
    private Vector3 startPointDensity;
    private Vector3 bigSize;

    public void SetDensitySize(Vector3 startPoint, Vector3 bigSize)
    {
        this.startPointDensity = startPoint;
        this.bigSize = bigSize;
    }

    public OctreeElement GetOctreeVoxel(Vector4[,,] density)
    {
        if (density.GetLength(0) > 1 && density.GetLength(1) > 1 && density.GetLength(2) > 1)
        {
            int treeSize = GetTreeSize(density.GetLength(0) - 1, density.GetLength(1) - 1, density.GetLength(2) - 1);
            // Debug.Log("treeSize: " + treeSize);

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
            SetVoxelsType(root);
            SetVoxelsType(root);
            SetVoxelsType(root);
            SetVoxelsType(root);
            SetVoxelsType(root);
            SetVoxelsType(root);
            SetVoxelsType(root);
            return root;
        }

        return null;
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
                            new Vector3(xDifferent * 2, yDifferent * 2, zDifferent * 2)))
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
            focusPoints.Add((int)((startPosition + different * i * 4) - (2 * different)));
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

        int startPointX = (int)density[0, 0, 0].x;
        int startPointY = (int)density[0, 0, 0].y;
        int startPointZ = (int)density[0, 0, 0].z;

        //int v = 0;

        for (int currentX = startPointX; currentX < xSize + startPointX; currentX += 2)
        {
            for (int currentY = startPointY; currentY < ySize + startPointY; currentY += 2)
            {
                for (int currentZ = startPointZ; currentZ < zSize + startPointZ; currentZ += 2)
                {
                    OctreeElement[] octreeElements = CreateVoxelsLeaf(density, new Vector3(xSize + startPointX, 
                        ySize + startPointY, zSize + startPointZ), new Vector3 (currentX, currentY, currentZ),
                        new Vector3(startPointX, startPointY, startPointZ));
                    OctreeBranch branchWithVoxels = new OctreeBranch(octreeElements,
                        (currentX, currentX + 2), (currentY, currentY + 2), (currentZ, currentZ + 2));
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

    private OctreeElement[] CreateVoxelsLeaf(Vector4[,,] density, Vector3 size, Vector3 currentPosition, Vector3 startPoint)
    {
        OctreeElement[] octreeElements = new OctreeElement[8];
        int elements = 0;

        for (int x = (int)currentPosition.x; x < (int)currentPosition.x + 2; x++)
        {
            for (int y = (int)currentPosition.y; y < (int)currentPosition.y + 2; y++)
            {
                for (int z = (int)currentPosition.z; z < (int)currentPosition.z + 2; z++)
                {
                    if (x <= (int)size.x - 1 && y <= (int)size.y - 1 && z <= (int)size.z - 1)
                    {
                        Voxel voxel = new Voxel((x, x + 1), (y, y + 1), (z, z + 1), new Vector2(x/bigSize.x, z/bigSize.z));
                        (float[,,] densityForVoxel, bool edge) = GetDensityForVoxel(density, new Vector3(x, y, z), startPoint);
                        voxel.SetDensity(densityForVoxel);
                        octreeElements[elements] = voxel;
                        
                        if(edge)
                        {
                            voxel.Edge = true;
                        }
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

    private (float[,,], bool) GetDensityForVoxel(Vector4[,,] density, Vector3 position, Vector3 startPoint)
    {
        float[,,] densityForVoxel = new float[2, 2, 2];
        bool edge = false;

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    if ((int)position.x + x == startPointDensity.x
                     || (int)position.y + y == startPointDensity.y
                     || (int)position.z + z == startPointDensity.z
                     || (int)position.x + x == bigSize.x
                     || (int)position.z + z == bigSize.z
                     )
                    {
                        densityForVoxel[x, y, z] = -0.01f;
                        edge = true;
                    }
                    else
                    {
                        densityForVoxel[x, y, z] = density[(int)position.x + x - (int)startPoint.x, 
                            (int)position.y + y - (int)startPoint.y, (int)position.z + z - (int)startPoint.z].w;
                    }
                }
            }
        }

        return (densityForVoxel, edge);
    }

    public void SetVoxelsType(OctreeElement root)
    {
        Stopwatch d = new Stopwatch();
        d.Restart();

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
                        voxel.SetType();
                    }
                    else
                    {
                        octreeBranches.AddRange(nextElements);
                    }
                }

                octreeBranches.RemoveAt(i);
            }
        }

        d.Stop();
        //Debug.Log("d: " + d.Elapsed.TotalMilliseconds);
    }
}
