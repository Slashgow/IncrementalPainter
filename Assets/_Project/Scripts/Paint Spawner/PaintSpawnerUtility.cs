using UnityEngine;

public static class PaintSpawnerUtility
{
    public static float CalculateUpgradeInfluence(int upgrades, float upgradeInfluence, float ennemyUpgradeCoefficient)
    {
        return 1f + upgradeInfluence * Mathf.Log(1 + ennemyUpgradeCoefficient * upgrades);
    }

    public static float Sigmoid(float level, float threshold, float steepness)
    {
        return 1f / (1f + Mathf.Exp(-steepness * (level - threshold)));
    }

    // S(t-1) * (1 - S(t))
    public static float CalculateSigmoidGate(int level, float thresholdBefore, float currentThreshold, float steepness, bool isLast = false)
    {
        if(isLast)
            return Sigmoid(level, thresholdBefore, steepness);

        return Sigmoid(level, thresholdBefore, steepness) * (1f - Sigmoid(level, currentThreshold, steepness));
    }

    // Wi = baseWeight * S(t-1) * (1 - S(t)) * f(U)
    public static float CalculateWeight(float baseWeight, float sigmoidGate, float upgradeInfluence)
    {
        return baseWeight * sigmoidGate * upgradeInfluence;
    }
}