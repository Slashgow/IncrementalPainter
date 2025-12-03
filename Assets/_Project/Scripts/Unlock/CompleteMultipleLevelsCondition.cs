using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class CompleteMultipleLevelsCondition : UnlockCondition
{
    [SerializeField] private int requiredCompletedLevels;

    public override bool IsMet()
    {
        var allSaves = GameSaveManager.Instance.GetAllLevelSaves();
        int completedCount = allSaves.Values.Count(save => save.isDone);
        return completedCount >= requiredCompletedLevels;
    }

    public override string GetDescription() => $"Complete {requiredCompletedLevels} levels";
}
