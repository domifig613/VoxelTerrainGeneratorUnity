using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainGeneratorHelper
{
    public static List<int> ReverseListOfPoints(int pointsCount, List<int> pointsInsideTerrain)
    {
        List<int> reverseInsideTerrain = new List<int>();

        for (int i = 0; i < pointsCount; i++)
        {
            reverseInsideTerrain.Add(i);
        }

        foreach (var point in pointsInsideTerrain)
        {
            reverseInsideTerrain.Remove(point);
        }

        return reverseInsideTerrain;
    }

    public static int GetIndexInArray(Vector3 wantedArrayPosition, (Vector3, float)[,,] terrainDensity)
    {
        int index = 0;

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (wantedArrayPosition == new Vector3(x, y, z))
                    {
                        return index;
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }

        return 0;
    }

    public static Vector3 GetPositionInArray(int wantedIndex, (Vector3, float)[,,] terrainDensity)
    {
        int index = 0;

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (index == wantedIndex)
                    {
                        return new Vector3(x, y, z);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }

        return new Vector3(0, 0, 0);
    }

    public static int CalculateCommonPlanesCount(Vector3 firstVector, Vector3 secondVector)
    {
        int commonPlanesCount = 0;

        if (firstVector.x == secondVector.x)
        {
            commonPlanesCount++;
        }

        if (firstVector.y == secondVector.y)
        {
            commonPlanesCount++;
        }

        if (firstVector.z == secondVector.z)
        {
            commonPlanesCount++;
        }

        return commonPlanesCount;
    }

    public static Vector3 GetCommonPlanes(Vector3 firstVector, Vector3 secondVector)
    {
        int x = (firstVector.x == secondVector.x ? 1 : 0);
        int y = (firstVector.y == secondVector.y ? 1 : 0);
        int z = (firstVector.z == secondVector.z ? 1 : 0);

        return new Vector3(x, y, z);
    }
}
