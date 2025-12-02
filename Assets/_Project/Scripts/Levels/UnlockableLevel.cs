using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UnlockableLevel : IUnlockable
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private List<UnlockCondition> unlockConditions = new List<UnlockCondition>();

    public LevelData LevelData => levelData;
    public bool IsUnlocked => CheckUnlockCondition();
    public List<UnlockCondition> UnlockConditions => unlockConditions;

    private LevelSaveData saveData;
    public bool CheckUnlockCondition()
    {
        if (unlockConditions == null || unlockConditions.Count == 0)
            return true;

        if (saveData == null)
            saveData = GameSaveManager.Instance.LoadLevelData(levelData.LevelAuthor, levelData.LevelTitle);

        if (saveData.isUnlocked)
            return true;

        bool allMet = unlockConditions.All(condition => condition.IsMet());

        if (allMet)
            Unlock();

        return allMet;
    }

    public void Unlock()
    {
        if (saveData == null)
            saveData = GameSaveManager.Instance.LoadLevelData(levelData.LevelAuthor, levelData.LevelTitle);

        if (saveData.isUnlocked)
            return;

        saveData.isUnlocked = true;
        GameSaveManager.Instance.SaveLevelData(saveData.isDone, saveData.completionRatio,
            levelData.LevelAuthor, levelData.LevelTitle, true);
        
    }
}
