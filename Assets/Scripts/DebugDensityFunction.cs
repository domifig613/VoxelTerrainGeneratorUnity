using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugDensityFunction : IDensityFunction
{
    (Vector3, float)[,,] IDensityFunction.GenerateTerrainDensity(Vector3 gridSize, bool reverse = false, int count = 0, int generateCase = 0)
    {
        (Vector3, float)[,,] terrainDensity = new (Vector3, float)[(int)gridSize.x + 1, (int)gridSize.y + 1, (int)gridSize.z + 1];
        int pointsCount = terrainDensity.GetLength(0) * terrainDensity.GetLength(1) * terrainDensity.GetLength(2);

        switch (count)
        {
            case 1:
                terrainDensity = GenerateDebugDensityWithOnePoint(terrainDensity, reverse, pointsCount);
                break;
            case 2:
                terrainDensity = GenerateDebugDensityWithTwoPoint(terrainDensity, reverse, pointsCount, generateCase);
                break;
            case 3:
                terrainDensity = GenerateDebugDensityWithThreePoint(terrainDensity, reverse, pointsCount, generateCase);
                break;
            case 4:
                terrainDensity = GenerateDebugDensityWithFourPoint(terrainDensity, reverse, pointsCount, generateCase);
                break;
            default:
                break;
        }

        (Vector3, float)[,,] terrainDensityBigger = new (Vector3, float)[(int)gridSize.x + 2, (int)gridSize.y + 2, (int)gridSize.z + 2];

        for (int x = 0; x < terrainDensityBigger.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensityBigger.GetLength(0); y++)
            {
                for (int z = 0; z < terrainDensityBigger.GetLength(0); z++)
                {
                    terrainDensityBigger[x, y, z] = (new Vector3(x, y, z), 0);
                }
            }
        }

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    terrainDensityBigger[x + 1, y + 1, z + 1].Item2 = terrainDensity[x, y, z].Item2;
                }
            }
        }

        return terrainDensityBigger;
    }
    private (Vector3, float)[,,] FillTerrainArray((Vector3, float)[,,] terrainDensity, List<int> pointsInsideTerrain)
    {
        int indexInArray = 0;

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    terrainDensity[x, y, z] = (new Vector3(x, y, z), Convert.ToInt32(pointsInsideTerrain.Contains(indexInArray)));
                    indexInArray++;
                }
            }
        }

        return terrainDensity;
    }

    #region Points

    #region OnePoint
    private (Vector3, float)[,,] GenerateDebugDensityWithOnePoint((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();

        if (reverse)
        {
            for (int i = 0; i < pointsCount; i++)
            {
                if (i != randomlySelectedIndex)
                {
                    pointsInsideTerrain.Add(i);
                }
            }
        }
        else
        {
            pointsInsideTerrain.Add(randomlySelectedIndex);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }
    #endregion

    #region TwoPoint
    private (Vector3, float)[,,] GenerateDebugDensityWithTwoPoint((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount, int generateCase)
    {
        switch (generateCase)
        {
            case 1:
                terrainDensity = GenerateTwoPointCase1(terrainDensity, reverse, pointsCount);
                break;
            case 2:
                terrainDensity = GenerateTwoPointCase2(terrainDensity, reverse, pointsCount);
                break;
            case 3:
                terrainDensity = GenerateTwoPointCase3(terrainDensity, reverse, pointsCount);
                break;
        }

        return terrainDensity;
    }

    private (Vector3, float)[,,] GenerateTwoPointCase1((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);

        Vector3 positionInArray = TerrainGeneratorHelper.GetPositionInArray(randomlySelectedIndex, terrainDensity);

        int randomlySelectedIndexOfSecondPoint = UnityEngine.Random.Range(0, 3);
        List<Vector3> possiblePointsOfSecondSelectedPoint = new List<Vector3>();

        {
            if (positionInArray.x == 0)
            {
                possiblePointsOfSecondSelectedPoint.Add(new Vector3(1, positionInArray.y, positionInArray.z));
            }
            else
            {
                possiblePointsOfSecondSelectedPoint.Add(new Vector3(0, positionInArray.y, positionInArray.z));
            }

            if (positionInArray.y == 0)
            {
                possiblePointsOfSecondSelectedPoint.Add(new Vector3(positionInArray.x, 1, positionInArray.z));
            }
            else
            {
                possiblePointsOfSecondSelectedPoint.Add(new Vector3(positionInArray.x, 0, positionInArray.z));
            }

            if (positionInArray.z == 0)
            {
                possiblePointsOfSecondSelectedPoint.Add(new Vector3(positionInArray.x, positionInArray.y, 1));
            }
            else
            {
                possiblePointsOfSecondSelectedPoint.Add(new Vector3(positionInArray.x, positionInArray.y, 0));
            }
        }//fill possible array

        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(possiblePointsOfSecondSelectedPoint[randomlySelectedIndexOfSecondPoint], terrainDensity));

        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }

    private (Vector3, float)[,,] GenerateTwoPointCase2((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);
        List<int> potentialSecondPointIndexes = new List<int>();

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(new Vector3(x, y, z), TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity)) == 1)
                    {
                        potentialSecondPointIndexes.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                }
            }
        }

        pointsInsideTerrain.Add(potentialSecondPointIndexes[UnityEngine.Random.Range(0, potentialSecondPointIndexes.Count)]);

        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }

    private (Vector3, float)[,,] GenerateTwoPointCase3((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);

        Vector3 randomlySelectedPoint = TerrainGeneratorHelper.GetPositionInArray(randomlySelectedIndex, terrainDensity);

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (randomlySelectedPoint.x != x && randomlySelectedPoint.y != y && randomlySelectedPoint.z != z)
                    {
                        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                        break;
                    }
                }
            }
        }

        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }
    #endregion

    #region ThreePoint
    private (Vector3, float)[,,] GenerateDebugDensityWithThreePoint((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount, int generateCase)
    {
        switch (generateCase)
        {
            case 1:
                terrainDensity = GenerateThreePointCase1(terrainDensity, reverse, pointsCount);
                break;
            case 2:
                terrainDensity = GenerateThreePointCase2(terrainDensity, reverse, pointsCount);
                break;
            case 3:
                terrainDensity = GenerateThreePointCase3(terrainDensity, reverse, pointsCount);
                break;
        }

        return terrainDensity;
    }

    private (Vector3, float)[,,] GenerateThreePointCase3((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);
        List<int> potentialSecondPointIndexes = new List<int>();

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(new Vector3(x, y, z), TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity)) == 1)
                    {
                        potentialSecondPointIndexes.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                }
            }
        }

        pointsInsideTerrain.Add(potentialSecondPointIndexes[UnityEngine.Random.Range(0, potentialSecondPointIndexes.Count)]);

        potentialSecondPointIndexes = new List<int>();

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(new Vector3(x, y, z), TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity)) < 2 && TerrainGeneratorHelper.CalculateCommonPlanesCount(new Vector3(x, y, z), TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[1], terrainDensity)) < 2)
                    {
                        potentialSecondPointIndexes.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                }
            }
        }

        pointsInsideTerrain.Add(potentialSecondPointIndexes[UnityEngine.Random.Range(0, potentialSecondPointIndexes.Count)]);


        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }

    private (Vector3, float)[,,] GenerateThreePointCase2((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        terrainDensity = GenerateTwoPointCase1(terrainDensity, false, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        List<int> potentialSecondPointIndexes = new List<int>();

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (terrainDensity[x, y, z].Item2 > 0)
                    {
                        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(terrainDensity[x, y, z].Item1, terrainDensity));
                    }
                }
            }
        }

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity), terrainDensity[x, y, z].Item1) <= 1
                     && TerrainGeneratorHelper.CalculateCommonPlanesCount(TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[1], terrainDensity), terrainDensity[x, y, z].Item1) <= 1)
                    {
                        potentialSecondPointIndexes.Add(TerrainGeneratorHelper.GetIndexInArray(terrainDensity[x, y, z].Item1, terrainDensity));
                    }
                }
            }
        }

        pointsInsideTerrain.Add(potentialSecondPointIndexes[UnityEngine.Random.Range(0, potentialSecondPointIndexes.Count)]);

        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }

    private (Vector3, float)[,,] GenerateThreePointCase1((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);
        List<int> potentialSecondPointIndexes = new List<int>();

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(new Vector3(x, y, z), TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity)) == 2)
                    {
                        potentialSecondPointIndexes.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                }
            }
        }

        pointsInsideTerrain.Add(potentialSecondPointIndexes[UnityEngine.Random.Range(0, potentialSecondPointIndexes.Count)]);

        potentialSecondPointIndexes = new List<int>();

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(new Vector3(x, y, z), TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity)) == 2
                     && TerrainGeneratorHelper.CalculateCommonPlanesCount(new Vector3(x, y, z), TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[1], terrainDensity)) == 1)
                    {
                        potentialSecondPointIndexes.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                }
            }
        }

        pointsInsideTerrain.Add(potentialSecondPointIndexes[UnityEngine.Random.Range(0, potentialSecondPointIndexes.Count)]);


        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }
    #endregion

    #region FourPoint
    private (Vector3, float)[,,] GenerateDebugDensityWithFourPoint((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount, int generateCase)
    {
        switch (generateCase)
        {
            case 1:
                terrainDensity = GenerateFourPointCase1(terrainDensity, reverse, pointsCount);
                break;
            case 2:
                terrainDensity = GenerateFourPointCase2(terrainDensity, reverse, pointsCount);
                break;
            case 3:
                terrainDensity = GenerateFourPointCase3(terrainDensity, reverse, pointsCount);
                break;
            case 4:
                terrainDensity = GenerateFourPointCase4(terrainDensity, reverse, pointsCount);
                break;
            case 5:
                terrainDensity = GenerateFourPointCase5(terrainDensity, reverse, pointsCount);
                break;
            case 6:
                terrainDensity = GenerateFourPointCase6(terrainDensity, reverse, pointsCount);
                break;
            case 7:
                terrainDensity = GenerateFourPointCase7(terrainDensity, reverse, pointsCount);
                break;
        }

        return terrainDensity;
    }

    private (Vector3, float)[,,] GenerateFourPointCase1((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);
        int randomlySelectedAxis = UnityEngine.Random.Range(0, 3);
        Vector3 firstSelectedPoint = TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity);

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (randomlySelectedAxis == 0 && firstSelectedPoint.x == x)
                    {
                        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                    else if (randomlySelectedAxis == 1 && firstSelectedPoint.y == y)
                    {
                        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                    else if (randomlySelectedAxis == 2 && firstSelectedPoint.z == z)
                    {
                        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                }
            }
        }

        pointsInsideTerrain = pointsInsideTerrain.Distinct().ToList();

        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }

    private (Vector3, float)[,,] GenerateFourPointCase2((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);
        Vector3 firstSelectedPoint = TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity);

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(firstSelectedPoint, new Vector3(x, y, z)) == 2)
                    {
                        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                }
            }
        }

        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }
  
    private (Vector3, float)[,,] GenerateFourPointCase3((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);
        Vector3 firstSelectedPoint = TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity);

        List<Vector3> potentialPointsWithSecoundIndex = new List<Vector3>();
        List<Vector3> potentialPointsWithThirdIndex = new List<Vector3>();

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    int sameAxisCount = TerrainGeneratorHelper.CalculateCommonPlanesCount(firstSelectedPoint, new Vector3(x, y, z));

                    if (sameAxisCount == 0)
                    {
                        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                    else if (sameAxisCount == 1)
                    {
                        potentialPointsWithSecoundIndex.Add(new Vector3(x, y, z));
                    }
                    else if (sameAxisCount == 2)
                    {
                        potentialPointsWithThirdIndex.Add(new Vector3(x, y, z));
                    }
                }
            }
        }

        Vector3 secondPoint = potentialPointsWithSecoundIndex[UnityEngine.Random.Range(0, potentialPointsWithSecoundIndex.Count)];
        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(secondPoint, terrainDensity));
        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(potentialPointsWithThirdIndex.Find(x => TerrainGeneratorHelper.CalculateCommonPlanesCount(secondPoint, x) == 0), terrainDensity));

        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }
    
    private (Vector3, float)[,,] GenerateFourPointCase4((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);
        Vector3 firstSelectedPoint = TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity);
        List<Vector3> potencialSelectedPoints = new List<Vector3>();
        Vector3 lastVector = new Vector3();

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(firstSelectedPoint, new Vector3(x, y, z)) == 2)
                    {
                        potencialSelectedPoints.Add(new Vector3(x, y, z));
                    }
                    else if (TerrainGeneratorHelper.CalculateCommonPlanesCount(firstSelectedPoint, new Vector3(x, y, z)) == 0)
                    {
                        lastVector = new Vector3(x, y, z);
                    }
                }
            }
        }

        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(potencialSelectedPoints[UnityEngine.Random.Range(0, potencialSelectedPoints.Count)], terrainDensity));
        potencialSelectedPoints.Clear();
        Vector3 secondSelectedPoint = TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[1], terrainDensity);

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(firstSelectedPoint, new Vector3(x, y, z)) == 1
                     && TerrainGeneratorHelper.CalculateCommonPlanesCount(secondSelectedPoint, new Vector3(x, y, z)) == 2)
                    {
                        potencialSelectedPoints.Add(new Vector3(x, y, z));
                    }
                }
            }
        }

        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(potencialSelectedPoints[UnityEngine.Random.Range(0, potencialSelectedPoints.Count)], terrainDensity));
        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(lastVector, terrainDensity));

        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }
    
    private (Vector3, float)[,,] GenerateFourPointCase5((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);
        int randomlySelectedAxis = UnityEngine.Random.Range(0, 3);
        Vector3 firstSelectedPoint = TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity);

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (randomlySelectedAxis == 0 && firstSelectedPoint.x == x)
                    {
                        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                    else if (randomlySelectedAxis == 1 && firstSelectedPoint.y == y)
                    {
                        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                    else if (randomlySelectedAxis == 2 && firstSelectedPoint.z == z)
                    {
                        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                }
            }
        }

        pointsInsideTerrain = pointsInsideTerrain.Distinct().ToList();
        List<Vector3> potentialLastPoints = new List<Vector3>();

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(new Vector3(x, y, z), TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[3], terrainDensity)) == 2)
                    {
                        potentialLastPoints.Add(new Vector3(x, y, z));
                    }
                }
            }
        }

        pointsInsideTerrain.RemoveAt(3);

        foreach (var point in potentialLastPoints)
        {
            int index = TerrainGeneratorHelper.GetIndexInArray(point, terrainDensity);

            if (!pointsInsideTerrain.Contains(index))
            {
                pointsInsideTerrain.Add(index);
                break;
            }
        }


        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }

    private (Vector3, float)[,,] GenerateFourPointCase6((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);
        int randomlySelectedAxis = UnityEngine.Random.Range(0, 3);
        Vector3 firstSelectedPoint = TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity);

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(firstSelectedPoint, new Vector3(x,y,z)) == 1)
                    {
                        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(new Vector3(x, y, z), terrainDensity));
                    }
                }
            }
        }

        pointsInsideTerrain = pointsInsideTerrain.Distinct().ToList();

        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }

    private (Vector3, float)[,,] GenerateFourPointCase7((Vector3, float)[,,] terrainDensity, bool reverse, int pointsCount)
    {
        int randomlySelectedIndex = UnityEngine.Random.Range(0, pointsCount);
        List<int> pointsInsideTerrain = new List<int>();
        pointsInsideTerrain.Add(randomlySelectedIndex);
        Vector3 firstSelectedPoint = TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[0], terrainDensity);
        List<Vector3> potencialSelectedPoints = new List<Vector3>();
        Vector3 lastVector = new Vector3();

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(firstSelectedPoint, new Vector3(x, y, z)) == 2)
                    {
                        potencialSelectedPoints.Add(new Vector3(x, y, z));
                    }
                    else if (TerrainGeneratorHelper.CalculateCommonPlanesCount(firstSelectedPoint, new Vector3(x, y, z)) == 0)
                    {
                        lastVector = new Vector3(x, y, z);
                    }
                }
            }
        }

        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(potencialSelectedPoints[UnityEngine.Random.Range(0, potencialSelectedPoints.Count)], terrainDensity));
        potencialSelectedPoints.Clear();
        Vector3 secondSelectedPoint = TerrainGeneratorHelper.GetPositionInArray(pointsInsideTerrain[1], terrainDensity);

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (TerrainGeneratorHelper.CalculateCommonPlanesCount(firstSelectedPoint, new Vector3(x, y, z)) == 1
                     && TerrainGeneratorHelper.CalculateCommonPlanesCount(secondSelectedPoint, new Vector3(x, y, z)) == 2)
                    {
                        potencialSelectedPoints.Add(new Vector3(x, y, z));
                    }
                }
            }
        }

        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(potencialSelectedPoints[UnityEngine.Random.Range(0, potencialSelectedPoints.Count)], terrainDensity));
        pointsInsideTerrain.Add(TerrainGeneratorHelper.GetIndexInArray(lastVector, terrainDensity));

        if (reverse)
        {
            pointsInsideTerrain = TerrainGeneratorHelper.ReverseListOfPoints(pointsCount, pointsInsideTerrain);
        }

        return FillTerrainArray(terrainDensity, pointsInsideTerrain);
    }

    #endregion

    #endregion
}
