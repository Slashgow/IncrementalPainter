using System;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class MinimumCompletionRatioCondition : UnlockCondition
{
    [SerializeField] private LocalizedString beginSentenceLocalizedString, endSentenceLocalizedString;
    [SerializeField] private string requiredLevelAuthor;
    [SerializeField] private string requiredLevelTitle;
    [SerializeField] private float minimumRatio;

    public override bool IsMet()
    {
        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(requiredLevelAuthor, requiredLevelTitle);
        return saveData != null && saveData.completionRatio >= minimumRatio;
    }

    public override string GetDescription() => $"{beginSentenceLocalizedString.GetLocalizedString()} '{requiredLevelTitle}' {endSentenceLocalizedString.GetLocalizedString()} {minimumRatio * 100}%";
}