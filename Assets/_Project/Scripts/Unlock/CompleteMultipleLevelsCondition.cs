using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class CompleteMultipleLevelsCondition : UnlockCondition
{
    [SerializeField] private LocalizedString beginSentenceLocalizedString, endSentenceLocalizedString;
    [SerializeField] private int requiredCompletedLevels;

    public override bool IsMet()
    {
        var allSaves = GameSaveManager.Instance.GetAllLevelSaves();
        int completedCount = allSaves.Values.Count(save => save.isDone);
        return completedCount >= requiredCompletedLevels;
    }

    public override string GetDescription() => GetDescriptionAsync().Result;

    private async Task<string> GetDescriptionAsync()
    {
#if UNITY_WEBGL
    string begin = await beginSentenceLocalizedString.GetLocalizedStringAsync().Task;
    string end = await endSentenceLocalizedString.GetLocalizedStringAsync().Task;
#else
        string begin = beginSentenceLocalizedString.GetLocalizedString();
        string end = endSentenceLocalizedString.GetLocalizedString();
#endif

        return $"{begin} {requiredCompletedLevels} {end}";
    }
}
