using System;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class CompletePreviousLevelCondition : UnlockCondition
{
    [SerializeField] private LocalizedString beginSentenceLocalizedString, endSentenceLocalizedString;
    [SerializeField] private LevelData requiredLevelData;

    public override bool IsMet()
    {
        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(requiredLevelData.LevelAuthor, requiredLevelData.LevelTitle);
        return saveData != null && saveData.bestRank != LevelRank.None;
    }

    public override string GetDescription() => $"{beginSentenceLocalizedString.GetLocalizedString()} '{requiredLevelData.LevelTitle}' {endSentenceLocalizedString.GetLocalizedString()} {requiredLevelData.LevelAuthor}";
}
