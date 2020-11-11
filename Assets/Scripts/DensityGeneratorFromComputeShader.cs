using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DensityGeneratorFromComputeShader : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;

    private string ComputeFunctionName = "GenerateDensity";
    private ComputeBuffer buffer;

    private ComputeBuffer octavesOffsetsBuffer;
    private int cachedOctavesCount = 0;

    public Vector4[] GetDensity(Vector3 densitySize, float pointsSpace, Vector3 startPoint,
        float noiseScale, float noiseWeight, int octavesCount)
    {
        if(octavesOffsetsBuffer == null || cachedOctavesCount != octavesCount)
        {
            cachedOctavesCount = octavesCount;
            octavesOffsetsBuffer = GetOctavesOffsets(octavesCount);
        }

        Vector3 realDensitySize = new Vector3((int)densitySize.x / pointsSpace, (int)densitySize.y / pointsSpace, (int)densitySize.z / pointsSpace);
        int bufferSize = (int)(realDensitySize.x * realDensitySize.y * realDensitySize.z); 
        buffer = new ComputeBuffer(bufferSize, sizeof(float) * 4);
        GenerateDensity(buffer, realDensitySize, startPoint, noiseScale, noiseWeight, octavesCount, octavesOffsetsBuffer);
        Vector4[] density = new Vector4[bufferSize];
        buffer.GetData(density);
        buffer.Release();
        return density;
    }

    private void GenerateDensity(ComputeBuffer pointsBuffer, Vector3 densitySize, Vector3 startPoint,
        float noiseScale, float noiseWeight, int octavesCount, ComputeBuffer octavesOffsetsBuffer)
    {
        int kernelId = computeShader.FindKernel(ComputeFunctionName);

        computeShader.SetBuffer(kernelId, "points", pointsBuffer);
        computeShader.SetVector("startPoint", new Vector4(startPoint.x, startPoint.y, startPoint.z));
        computeShader.SetVector("densitySize", new Vector4(densitySize.x, densitySize.y, densitySize.z));
        computeShader.SetFloat("noiseScale", noiseScale);
        computeShader.SetFloat("noiseWeight", noiseWeight);
        computeShader.SetInt("octavesCount", octavesCount);
        computeShader.SetBuffer(kernelId, "octavesOffsets", octavesOffsetsBuffer);

        computeShader.Dispatch(kernelId, (int)densitySize.x, (int)densitySize.y, (int)densitySize.z);
    }

    private ComputeBuffer GetOctavesOffsets(int octavesCount)
    {
        ComputeBuffer buffer = new ComputeBuffer(octavesCount, sizeof(float) * 3);
        Vector3[] array = new Vector3[octavesCount];

        for (int i = 0; i < octavesCount; i++)
        {
            array[i] = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        buffer.SetData(array);
        return buffer;
    }
}
