using System.Net;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;

public static class Helper
{
    public static Vector2 CalculateOffset(bool useRandomOffset, Vector2 offset, Vector2 randomOffsetRange)
    {
        Vector2 finalOffset = offset;
        if (useRandomOffset)
        {
            finalOffset += new Vector2(
                Random.Range(-randomOffsetRange.x, randomOffsetRange.x),
                Random.Range(-randomOffsetRange.y, randomOffsetRange.y)
            );
        }
        return finalOffset;
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }

    public static Vector3 CalculateBezierPoint(float t, Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint)
    {
        // Quadratic Bezier curve formula: B(t) = (1-t)²P₀ + 2(1-t)tP₁ + t²P₂
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * startPoint; // (1-t)² * P0
        point += 2 * u * t * controlPoint; // 2(1-t)t * P1
        point += tt * endPoint; // t² * P2

        return point;
    }
}
