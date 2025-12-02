using System;
using UnityEngine;

[Serializable]
public class CompleteMultipleLevelsCondition : UnlockCondition
{
    [SerializeField] private int requiredCompletedLevels;

    public CompleteMultipleLevelsCondition(int count)
    {
        requiredCompletedLevels = count;
    }

    public override bool IsMet()
    {
        //var allSaves = GameSaveManager.Instance.GetAllLevelSaves();
        //int completedCount = allSaves.FindAll(save => save.isDone).Count;
        //return completedCount >= requiredCompletedLevels;
        return true;
    }

    public override string GetDescription() => $"Complete {requiredCompletedLevels} levels";
}
