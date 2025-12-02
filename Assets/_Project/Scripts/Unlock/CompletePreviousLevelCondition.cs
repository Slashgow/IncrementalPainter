using System;
using UnityEngine;

[Serializable]
public class CompletePreviousLevelCondition : UnlockCondition
{
    [SerializeField] private string requiredLevelAuthor;
    [SerializeField] private string requiredLevelTitle;

    public CompletePreviousLevelCondition(string author, string title)
    {
        requiredLevelAuthor = author;
        requiredLevelTitle = title;
    }

    public override bool IsMet()
    {
        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(requiredLevelAuthor, requiredLevelTitle);
        return saveData != null && saveData.isDone;
    }

    public override string GetDescription() => $"Complete '{requiredLevelTitle}' by {requiredLevelAuthor}";
}
