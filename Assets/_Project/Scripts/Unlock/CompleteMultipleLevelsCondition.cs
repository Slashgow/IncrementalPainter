using System;
using System.Linq;
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

    public override string GetDescription() => $"{beginSentenceLocalizedString.GetLocalizedString()} {requiredCompletedLevels} {endSentenceLocalizedString.GetLocalizedString()}";
}
