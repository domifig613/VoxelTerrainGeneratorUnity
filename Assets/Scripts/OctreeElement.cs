using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OctreeElement
{
    private OctreeElement previousElement;
    protected OctreeElement[] nextElements;
    protected (int, int) xRange;
    protected (int, int) yRange;
    protected (int, int) zRange;

    private bool leaf;

    public OctreeElement PreviousElement => previousElement;
    public OctreeElement[] NextElements => nextElements;

    public (int, int) XRange => xRange;
    public (int, int) YRange => yRange;
    public (int, int) ZRange => zRange;

    public OctreeElement(bool leaf)
    {
        this.leaf = leaf;
    }

    public OctreeElement(OctreeElement previousElement, bool leaf)
    {
        this.previousElement = previousElement;
        this.leaf = leaf;
    }

    public OctreeElement(OctreeElement[] nextElements)
    {
        this.leaf = false;
        this.nextElements = nextElements;
    }

    public bool IsPointInFocusPoint(Vector3 point, Vector3 different)
    {
        return    ((xRange.Item1 >= point.x - different.x && xRange.Item1 <= point.x + different.x)
                && (xRange.Item2 >= point.x - different.x && xRange.Item2 <= point.x + different.x)
                && (yRange.Item1 >= point.y - different.y && yRange.Item1 <= point.y + different.y)
                && (yRange.Item2 >= point.y - different.y && yRange.Item2 <= point.y + different.y)
                && (zRange.Item1 >= point.z - different.z && zRange.Item1 <= point.z + different.z)
                && (zRange.Item2 >= point.z - different.z && zRange.Item2 <= point.z + different.z));
    }

    public void SetPrevious(OctreeBranch previousElement)
    {
        this.previousElement = previousElement;
    }
}
