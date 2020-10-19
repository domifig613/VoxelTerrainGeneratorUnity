using System;
using UnityEngine;

public class VertexesCaseProvider : MonoBehaviour
{
    private int[,,] verticesIndexes = new int[256, 5, 3];

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
        catch(Exception exception)
        {
            Debug.LogError(exception.Message);
            return false;
        }
    }

    //public int ReadCaseForCube((Vector3, float)[,,] terrainDensity)
    //{

    //}
}