using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseHelper : MonoBehaviour
{
    private static Texture2D texture1;
    private static Texture2D texture2;

    private void Start()
    {
        texture1 = Resources.Load<Texture2D>("Perlin1");
        texture2 = Resources.Load<Texture2D>("Perlin2");
    }

    public static Color GetColor1(float x, float z)
    {
        return texture1.GetPixelBilinear(x,z);
    }
    public static Color GetColor2(float x, float z)
    {
        return texture2.GetPixelBilinear(x, z);
    }
}
