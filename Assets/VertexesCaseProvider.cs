using System;
using UnityEngine;

public class VertexesCaseProvider : MonoBehaviour
{
    private int[,,] verticesIndexes = new int[256, 5, 3];

    private int[,,] edgesBetweenVertexIndex = new int[,,]
    {
        {{0,0,0}, {0,1,0}},
        {{0,1,0}, {1,1,0}},
        {{1,1,0}, {1,0,0}},
        {{1,0,0}, {0,0,0}},
        {{0,0,1}, {0,1,1}},
        {{0,1,1}, {1,1,1}},
        {{1,1,1}, {1,0,1}},
        {{1,0,1}, {0,0,1}},
        {{0,0,0}, {0,0,1}},
        {{0,1,0}, {0,1,1}},
        {{1,1,0}, {1,1,1}},
        {{1,0,0}, {1,0,1}},
    };

    public bool ReadVerticesIndexesFromFile()
    {
        try
        {
            var textFile = Resources.Load<TextAsset>("EdgesTrisCases");
            string[] lines = textFile.text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            string[,] verticesIndexesStrings = new string[256, 5];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] temporaryArray = lines[i].Split(',');

                for (int j = 0; j < temporaryArray.Length; j++)
                {
                    if (temporaryArray[j] != "")
                    {
                        verticesIndexesStrings[i, j] = temporaryArray[j];
                    }
                }
            }

            string[,,] verticesIndexesStringSeparated = new string[256, 5, 3];

            for (int i = 0; i < verticesIndexes.GetLength(0); i++)
            {
                for (int j = 0; j < verticesIndexes.GetLength(1); j++)
                {
                    string[] temporaryArray = verticesIndexesStrings[i, j].Split(' ');

                    int trisIndex = 0;

                    for (int k = 0; k < temporaryArray.Length; k++)
                    {
                        if (temporaryArray[k] != "")
                        {
                            if (int.TryParse(temporaryArray[k], out verticesIndexes[i, j, trisIndex]))
                            {
                                trisIndex++;
                            }
                        }
                    }
                }
            }

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            return false;
        }
    }

    public int ReadCaseForCube(Vector4[,,] terrainDensity, float borderValue)
    {
        int wantedCase = 0;

        for (int x = 0; x < terrainDensity.GetLength(0); x++)
        {
            for (int y = 0; y < terrainDensity.GetLength(1); y++)
            {
                for (int z = 0; z < terrainDensity.GetLength(2); z++)
                {
                    if (terrainDensity[x, y, z].w >= borderValue)
                    {
                        int counter = 0;
                        counter += (y > 0 ? (x > 0 ? 2 : 1) : (x > 0 ? 3 : 0));
                        counter += (z > 0 ? 4 : 0);
                        wantedCase += (int)Math.Pow(2, counter);
                    }
                }
            }
        }

        return wantedCase;
    }

    public int[,] GetVerticesEdgesIndexes(int wantedCase)
    {
        int[,] edgesIndexes = new int[5, 3];

        for (int i = 0; i < verticesIndexes.GetLength(1); i++)
        {
            for (int j = 0; j < verticesIndexes.GetLength(2); j++)
            {
                edgesIndexes[i, j] = verticesIndexes[wantedCase, i, j];
            }
        }

        return edgesIndexes;
    }

    public int[,] GetVertexesIndexInArrayByEdge(int edgeIndex)
    {
        int[,] vertex = new int[2,3];

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                vertex[i, j] = edgesBetweenVertexIndex[edgeIndex, i, j];
            }
        }

        return vertex;
    }
}