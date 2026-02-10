using System;
using System.Threading.Tasks;
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

        return $"{begin} '{requiredLevelTitle}' {end} {minimumRatio * 100}%";
    }

}