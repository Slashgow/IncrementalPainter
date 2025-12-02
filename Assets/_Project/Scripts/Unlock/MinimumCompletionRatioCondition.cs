using System;
using UnityEngine;

[Serializable]
public class MinimumCompletionRatioCondition : UnlockCondition
{
    [SerializeField] private string requiredLevelAuthor;
    [SerializeField] private string requiredLevelTitle;
    [SerializeField] private float minimumRatio;

    public MinimumCompletionRatioCondition(string author, string title, float ratio)
    {
        requiredLevelAuthor = author;
        requiredLevelTitle = title;
        minimumRatio = ratio;
    }

    public override bool IsMet()
    {
        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(requiredLevelAuthor, requiredLevelTitle);
        return saveData != null && saveData.completionRatio >= minimumRatio;
    }

    public override string GetDescription() => $"Complete '{requiredLevelTitle}' at {minimumRatio * 100}%";
}