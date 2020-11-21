using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class NoiseProvider : MonoBehaviour
{
    [SerializeField] private string NoiseResourcesFolderPath;
    [SerializeField] private List<string> NoiseTextureNames;

    public List<string> GetTexturesNoiseNames()
    {
        return NoiseTextureNames;
    }

    public float[,,] ReadNoise(string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, "Resources", NoiseResourcesFolderPath, fileName + ".txt");

        if (File.Exists(filePath))
        {
            List<string> fileLines = File.ReadAllLines(filePath).ToList().Where(x => x.Length > 0).ToList();

            if (fileLines.Count > 0)
            {
                List<string> yFirstLine = fileLines[0].Split(';').ToList().Where(x => x.Length > 0).ToList();
                int YSize = yFirstLine.Count;
                List<string> zFirstLine = yFirstLine[0].Split(',').ToList().Where(x => x.Length > 0).ToList();
                int ZSize = zFirstLine.Count;

                float[,,] noiseTexture3D = new float[fileLines.Count, YSize, ZSize];

                for (int x = 0; x < fileLines.Count; x++)
                {
                    List<string> yLine = fileLines[x].Split(';').ToList().Where(i => i.Length > 0).ToList();

                    for (int y = 0; y < yLine.Count; y++)
                    {
                        List<string> zLine = yLine[y].Split(',').ToList().Where(i => i.Length > 0).ToList();

                        for (int z = 0; z < zLine.Count; z++)
                        {
                            noiseTexture3D[x, y, z] = Mathf.Abs(Convert.ToInt32(zLine[z]) / 100f);
                        }
                    }
                }

                return noiseTexture3D;
            }
            else
            {
                Debug.Log("Noise: " + filePath + " is empty");
                return new float[0, 0, 0];
            }
        }
        else
        {
            throw new System.Exception("File is not exist");
        }
    }
}
