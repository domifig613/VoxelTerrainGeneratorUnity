using System;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class VisualTerrainController : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    [SerializeField] private Material material;
    [SerializeField] private float yMax;
    [SerializeField] private float yMin;
    [SerializeField] private GameObject water;
    [SerializeField] private float waterLevel;

    private const int textureWidth = 100;
    private const int textureHeight = 1;

    private Texture2D texture;

    public void SetWaterPosition(Vector3 startPoint, Vector2 terrainSize)
    {
        water.transform.localScale = new Vector3(terrainSize.x/10f, 1, terrainSize.y/10f);
        water.transform.position = 
            new Vector3(startPoint.x + terrainSize.x/2f,water.transform.position.y, startPoint.z + terrainSize.y/2f);

    }

    void Update()
    {
        RefreshWater();
        RefreshMaterialValues();
    }

    private void RefreshWater()
    {
        water.transform.position = new Vector3(water.transform.position.x, waterLevel, water.transform.position.z);
    }

    private void RefreshMaterialValues()
    {
        RefreshTexture();

        material.SetFloat("yMax", yMax);
        material.SetFloat("yMin", yMin);
        material.SetTexture("rampTexture", texture);
    }

    private void RefreshTexture()
    {
        texture = new Texture2D(textureWidth, textureHeight);
        Color[] gradientColors = new Color[textureWidth];

        for (int i = 0; i < textureWidth; i++)
        {
            gradientColors[i] = gradient.Evaluate((float)i / (float)textureWidth);
        }

        texture.SetPixels(gradientColors);
        texture.Apply();
    }

    //debug
    private void WriteTextureForTest()
    {
        if (!File.Exists(@"D:\Texture.png"))
        {
            File.WriteAllBytes(@"D:\Texture.png", texture.EncodeToPNG());
        }
    }
}
