using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OctreeBranch : OctreeElement
{
    public OctreeBranch() :base(false)
    {
    }

    public OctreeBranch(OctreeElement previousElement)
        : base(previousElement, false)
    {
    }

    public OctreeBranch(OctreeElement[] nextElements,
    (int, int) xRange, (int, int) yRange, (int, int) zRange)
        : base(nextElements)
    {
        base.xRange = xRange;
        base.yRange = yRange;
        base.zRange = zRange;

        foreach (var element in nextElements)
        {
            if (element != null)
            {
                element.SetPrevious(this);
            }
        }
    }

    public OctreeBranch(OctreeElement[] nextElements) : base(nextElements)
    {
        foreach (var element in nextElements)
        {
            element.SetPrevious(this);
        }

        List<int> xSize = new List<int>();
        List<int> ySize = new List<int>();
        List<int> zSize = new List<int>();

        foreach (var element in nextElements)
        {
            xSize.Add(element.XRange.Item1);
            xSize.Add(element.XRange.Item2);
            ySize.Add(element.YRange.Item1);
            ySize.Add(element.YRange.Item2);
            zSize.Add(element.ZRange.Item1);
            zSize.Add(element.ZRange.Item2);
        }

        if (xSize.Count > 0)
        {
            xRange.Item1 = xSize.Min();
            xRange.Item2 = xSize.Max();
            yRange.Item1 = ySize.Min();
            yRange.Item2 = ySize.Max();
            zRange.Item1 = zSize.Min();
            zRange.Item2 = zSize.Max();
        }
    }
}
