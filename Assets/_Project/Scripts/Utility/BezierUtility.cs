using UnityEngine;

public static class BezierUtility
{
    /// <summary>
    /// Calculate a point on a quadratic Bezier curve
    /// </summary>
    /// <param name="p0">Start point</param>
    /// <param name="p1">Control point</param>
    /// <param name="p2">End point</param>
    /// <param name="t">Progress along curve (0 to 1)</param>
    /// <returns>Position on the curve</returns>
    public static Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0; // (1-t)^2 * P0
        point += 2 * u * t * p1; // 2(1-t)t * P1
        point += tt * p2;        // t^2 * P2

        return point;
    }

    /// <summary>
    /// Calculate a point on a cubic Bezier curve
    /// </summary>
    /// <param name="p0">Start point</param>
    /// <param name="p1">First control point</param>
    /// <param name="p2">Second control point</param>
    /// <param name="p3">End point</param>
    /// <param name="t">Progress along curve (0 to 1)</param>
    /// <returns>Position on the curve</returns>
    public static Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;       // (1-t)^3 * P0
        point += 3 * uu * t * p1;       // 3(1-t)^2 * t * P1
        point += 3 * u * tt * p2;       // 3(1-t) * t^2 * P2
        point += ttt * p3;              // t^3 * P3

        return point;
    }

    /// <summary>
    /// Calculate an automatic control point for a quadratic Bezier curve
    /// Creates an arc perpendicular to the line between start and end
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="height">Height of the arc</param>
    /// <param name="direction">Direction of curve (1 = up/right, -1 = down/left)</param>
    /// <returns>Control point position</returns>
    public static Vector3 CalculateQuadraticControlPoint(Vector3 start, Vector3 end, float height, float direction = 1f)
    {
        Vector3 midpoint = (start + end) / 2f;
        Vector3 toEnd = end - start;

        // Get perpendicular direction (rotate 90 degrees)
        Vector3 perpendicular = new Vector3(-toEnd.y, toEnd.x, 0f).normalized;

        return midpoint + perpendicular * height * direction;
    }

    /// <summary>
    /// Calculate automatic control points for a cubic Bezier curve
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="height">Height of the arc</param>
    /// <param name="curveTension">How tight the curve is (0-1, higher = tighter)</param>
    /// <returns>Tuple of (control point 1, control point 2)</returns>
    public static (Vector3 cp1, Vector3 cp2) CalculateCubicControlPoints(Vector3 start, Vector3 end, float height, float curveTension = 0.5f)
    {
        Vector3 midpoint = (start + end) / 2f;
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0f);

        float distance = Vector3.Distance(start, end);
        float controlPointOffset = distance * curveTension;

        Vector3 cp1 = start + direction * controlPointOffset + perpendicular * height;
        Vector3 cp2 = end - direction * controlPointOffset + perpendicular * height;

        return (cp1, cp2);
    }

    /// <summary>
    /// Get the total approximate length of a quadratic Bezier curve
    /// </summary>
    public static float GetQuadraticBezierLength(Vector3 p0, Vector3 p1, Vector3 p2, int segments = 20)
    {
        float length = 0f;
        Vector3 previousPoint = p0;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 currentPoint = QuadraticBezier(p0, p1, p2, t);
            length += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        return length;
    }
}
