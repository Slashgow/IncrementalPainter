using UnityEngine;

public static class LuckUtility
{
    public static bool RollLuck(float chancePercentage)
    {
        if (chancePercentage <= 0f)
            return false;

        if (chancePercentage >= 100f)
            return true;

        float randomValue = Random.Range(0f, 100f);
        return randomValue <= chancePercentage;
    }

    public static bool TryLuck(float chancePercentage, System.Action onSuccess)
    {
        bool success = RollLuck(chancePercentage);
        if (success)
            onSuccess?.Invoke();

        return success;
    }

    public static bool TryLuck(float chancePercentage, System.Action onSuccess, System.Action onFailure)
    {
        bool success = RollLuck(chancePercentage);
        if (success)
            onSuccess?.Invoke();
        else
            onFailure?.Invoke();

        return success;
    }

    public static float GetRandomFloatMinMax(float minValue, float maxValue) => Random.Range(minValue, maxValue);
}