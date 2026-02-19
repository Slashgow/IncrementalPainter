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

    public static void DestroyAllChildrenWithComponent<T>(this Transform transform) where T : Component
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if(transform.GetChild(i).GetComponent<T>())
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
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

    /// <summary>
    /// Returns a safe anchored position (in Screen Space - Overlay canvas space) for a UI panel
    /// that avoids screen edges and doesn't overlap the world object it originates from.
    /// </summary>
    /// <param name="worldPosition">World position of the source object.</param>
    /// <param name="panelRectTransform">RectTransform of the panel prefab (center-pivoted).</param>
    /// <param name="canvas">The overlay canvas the panel will be placed on.</param>
    /// <param name="desiredOffset">Preferred offset direction/distance from the object (e.g. new Vector2(0, 150)).</param>
    /// <param name="avoidanceRadius">Screen-space radius of the source object to avoid overlapping.</param>
    /// <returns>Clamped anchored position safe to assign to panel.anchoredPosition.</returns>
    public static Vector2 GetSafeUIPosition(
        Vector3 worldPosition,
        RectTransform panelRectTransform,
        Canvas canvas,
        Vector2 desiredOffset,
        float avoidanceRadius = 40f)
    {
        // Step 1 — world → screen position
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        // Step 2 — screen → canvas local position
        // On a Screen Space - Overlay canvas the canvas rect matches the screen exactly,
        // so we only need to account for the canvas scaler if one is present.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            null, // null camera for Overlay canvas
            out Vector2 localPoint);

        // Step 3 — apply desired offset
        Vector2 candidate = localPoint + desiredOffset;

        // Step 4 — compute half-size of panel (accounts for canvas scaler automatically)
        Vector2 halfSize = panelRectTransform.rect.size * 0.5f;

        // Step 5 — compute canvas bounds in local space
        Rect canvasRect = canvas.GetComponent<RectTransform>().rect;
        float minX = canvasRect.xMin + halfSize.x;
        float maxX = canvasRect.xMax - halfSize.x;
        float minY = canvasRect.yMin + halfSize.y;
        float maxY = canvasRect.yMax - halfSize.y;

        // Step 6 — clamp to screen edges
        candidate.x = Mathf.Clamp(candidate.x, minX, maxX);
        candidate.y = Mathf.Clamp(candidate.y, minY, maxY);

        // Step 7 — if the clamped panel still overlaps the source object, flip the offset
        // so the panel moves to the opposite side rather than sitting on top of the object.
        float overlapDist = Vector2.Distance(candidate, localPoint);
        float minSafeDistance = avoidanceRadius + Mathf.Max(halfSize.x, halfSize.y);

        if (overlapDist < minSafeDistance)
        {
            Vector2 flippedCandidate = localPoint - desiredOffset;
            flippedCandidate.x = Mathf.Clamp(flippedCandidate.x, minX, maxX);
            flippedCandidate.y = Mathf.Clamp(flippedCandidate.y, minY, maxY);

            // Only accept the flip if it's actually further from the object
            if (Vector2.Distance(flippedCandidate, localPoint) > overlapDist)
                candidate = flippedCandidate;
        }

        return candidate;
    }
}
