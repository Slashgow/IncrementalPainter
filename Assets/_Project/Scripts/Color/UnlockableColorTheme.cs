using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class UnlockableColorTheme : IUnlockable
{
    [SerializeField] private ColorTheme colorTheme;
    [SerializeField] private List<UnlockCondition> unlockConditions = new List<UnlockCondition>();

    public ColorTheme ColorTheme => colorTheme;
    public bool IsUnlocked => CheckUnlockCondition();
    public List<UnlockCondition> UnlockConditions => unlockConditions;

    public bool CheckUnlockCondition()
    {
        if (colorTheme == null)
        {
            Debug.LogWarning("ColorTheme is null in UnlockableColorTheme!");
            return false;
        }

        if (unlockConditions == null || unlockConditions.Count == 0)
            return true;

        // Check if already unlocked in save data
        if (ThemeColorManager.Instance.IsThemeUnlocked(colorTheme.ThemeId))
            return true;

        // Check if all conditions are met
        bool allMet = unlockConditions.All(condition => condition.IsMet());

        if (allMet)
            Unlock();

        return allMet;
    }

    public void Unlock()
    {
        if (colorTheme == null)
        {
            Debug.LogWarning("Cannot unlock - ColorTheme is null!");
            return;
        }

        if (ThemeColorManager.Instance.IsThemeUnlocked(colorTheme.ThemeId))
            return;

        ThemeColorManager.Instance.UnlockTheme(colorTheme.ThemeId);
        Debug.Log($"Color theme '{colorTheme.ThemeName}' unlocked!");
    }

    public void Lock()
    {
        if (colorTheme == null)
            return;

        ThemeColorManager.Instance.LockTheme(colorTheme.ThemeId);
    }

    public string GetUnlockDescription()
    {
        if (unlockConditions == null || unlockConditions.Count == 0)
            return "Already unlocked";

        if (IsUnlocked)
            return "Unlocked";

        string description = "Unlock conditions:\n";
        for (int i = 0; i < unlockConditions.Count; i++)
        {
            description += $"- {unlockConditions[i].GetDescription()}";
            if (i < unlockConditions.Count - 1)
                description += "\n";
        }

        return description;
    }
}