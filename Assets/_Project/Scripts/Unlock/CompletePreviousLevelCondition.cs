using System;
using System.Threading.Tasks;
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

    public override string GetDescription() => GetDescriptionAsync().Result;

    private async Task<string> GetDescriptionAsync()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    string begin = await beginSentenceLocalizedString.GetLocalizedStringAsync();
    string end = await endSentenceLocalizedString.GetLocalizedStringAsync();
#else
        string begin = beginSentenceLocalizedString.GetLocalizedString();
        string end = endSentenceLocalizedString.GetLocalizedString();
#endif

        return $"{begin} '{requiredLevelData.LevelTitle}' {end} {requiredLevelData.LevelAuthor}";
    }

}
