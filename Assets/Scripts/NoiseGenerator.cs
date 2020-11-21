using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] private string NoiseTextureName;
    [SerializeField] private string NoiseResourcesFolderPath;
    [SerializeField] private Vector3 noiseSize;

    private void Start()
    {
        GenerateNoise();
    }

    private void GenerateNoise()
    {
        string filePath = Path.Combine(Application.dataPath, "Resources", NoiseResourcesFolderPath, NoiseTextureName + ".txt");

        if (!File.Exists(filePath))
        {
            int[,,] noiseTexture = new int[(int)noiseSize.x, (int)noiseSize.y, (int)noiseSize.z];
            List<string> fileLines = new List<string>();

            for (int x = 0; x < noiseSize.x; x++)
            {
                string fileLine = string.Empty;

                for (int y = 0; y < noiseSize.y; y++)
                {
                    for (int z = 0; z < noiseSize.z; z++)
                    {
                        fileLine += (Random.Range(-100, 101).ToString() + ",");
                    }

                    fileLine += ";";
                }

                fileLines.Add(fileLine);
            }

            File.WriteAllLines(filePath, fileLines.ToArray());
        }
        else
        {
            Debug.LogError("File already exist");
        }

        Debug.Log("NoiseGenerationFinished");
    }
}
