using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityGizmosDrawer : MonoBehaviour
{
    private Vector4[] pointsToDraw;

    public void SetPoints(Vector4[] newPoints)
    {
        pointsToDraw = newPoints;
    }

    public void ClearPoints()
    {
        pointsToDraw = null;
    }

    private void OnDrawGizmos()
    {
        if (pointsToDraw != null)
        {
            foreach (var point in pointsToDraw)
            {
                float colorValue = Mathf.InverseLerp(-16f, 3f, point.w);
                Gizmos.color = new Color(colorValue, colorValue, colorValue);
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}
