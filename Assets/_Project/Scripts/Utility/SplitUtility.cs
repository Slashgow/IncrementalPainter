using UnityEngine;

public static class SplitUtility
{
    public static bool IsValidSplittable(ISplittable splittable)
    {
        if (splittable == null)
            return false;

        if (splittable.Transform == null)
            return false;

        if (splittable is MonoBehaviour mono && mono == null)
            return false;

        return true;
    }

    public static Vector3[] CalculateCircleSpawnPositions(Vector3 center, int count, float radius)
    {
        Vector3[] positions = new Vector3[count];
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            positions[i] = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );
        }

        return positions;
    }

    public static Vector3[] CalculateRandomSpawnPositions(Vector3 center, int count, float minRadius, float maxRadius)
    {
        Vector3[] positions = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float randomRadius = Random.Range(minRadius, maxRadius);
            positions[i] = center + new Vector3(
                randomCircle.x * randomRadius,
                randomCircle.y * randomRadius,
                0f
            );
        }

        return positions;
    }

    public static float CalculateSplitScale(float baseScale, int generation, float scaleMultiplier = 0.7f)
    {
        return baseScale * Mathf.Pow(scaleMultiplier, generation);
    }

    public static bool CanSplitGeneration(int currentGeneration, int maxGenerations)
    {
        return currentGeneration < maxGenerations;
    }
}