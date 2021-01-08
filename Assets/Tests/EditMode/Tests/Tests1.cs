using NUnit.Framework;
using UnityEngine;

public class Tests
{
    [Test]
    public void GetVerticesEdgesIndexes1()
    {
        Assert.AreEqual(VertexesCaseProvider.GetVertexesIndexInArrayByEdge(0)[0, 0], 0);
    }

    [Test]
    public void GetVerticesEdgesIndexes2()
    {
        Assert.AreEqual(VertexesCaseProvider.GetVertexesIndexInArrayByEdge(5)[0, 1], 1);
    }

    [Test]
    public void GetVerticesEdgesIndexes3()
    {
        Assert.AreEqual(VertexesCaseProvider.GetVertexesIndexInArrayByEdge(1)[1, 1], 1);
    }


    [Test]
    public void GetSumDensity1()
    {
        Voxel voxel = new Voxel((0, 0), (0, 0), (0, 0), Vector2.zero);
        voxel.SetDensity(new float[0,0,0]);
        Assert.AreEqual(voxel.GetSumDensity(), 0f);
    }

    [Test]
    public void GetSumDensity2()
    {
        Voxel voxel = new Voxel((0, 0), (0, 0), (0, 0), Vector2.zero);

        float[,,] floatArray = new float[2, 2, 2];

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    floatArray[i, j, k] = 1;
                }
            }
        }

        voxel.SetDensity(floatArray);
        Assert.AreEqual(voxel.GetSumDensity(), 8f);
    }
}