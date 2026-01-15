using UnityEngine;
using UnityEngine.Localization;

public class RankUnlockCondition : UnlockCondition
{
    [SerializeField] private LocalizedString beginSentenceLocalizedString, midSentenceLocalizedString, endSentenceLocalizedString;

    [Header("Required Previous Level")]
    [SerializeField] private LevelData requiredLevel;

    [Header("Required Rank")]
    [SerializeField] private LevelRank minimumRank = LevelRank.C;

    public LevelData GetRequiredLevel() => requiredLevel;
    public LevelRank GetMinimumRank() => minimumRank;

    public override bool IsMet()
    {
        if (requiredLevel == null)
        {
            Debug.LogWarning("RankUnlockCondition: No required level set!");
            return true;
        }

        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(requiredLevel.LevelAuthor, requiredLevel.LevelTitle);

        if (saveData == null)
            return false;

        return saveData.bestRank >= minimumRank;
    }

    public override string GetDescription()
    {
        if (requiredLevel == null)
            return "No level requirement";

        return $"{beginSentenceLocalizedString.GetLocalizedString()} '{requiredLevel.LevelTitle}' " +
            $"{midSentenceLocalizedString.GetLocalizedString()} {minimumRank.GetDisplayName()} {endSentenceLocalizedString.GetLocalizedString()}";
    }

 
}