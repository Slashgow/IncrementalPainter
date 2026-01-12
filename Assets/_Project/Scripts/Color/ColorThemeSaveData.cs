using System;
using System.Collections.Generic;

[Serializable]
public class ColorThemeSaveData
{
    public List<string> unlockedThemeIds = new List<string>();
    public string currentThemeId;

    public ColorThemeSaveData()
    {
        unlockedThemeIds = new List<string>();
        currentThemeId = string.Empty;
    }

    public bool IsThemeUnlocked(string themeId)
    {
        return unlockedThemeIds.Contains(themeId);
    }

    public void UnlockTheme(string themeId)
    {
        if (!unlockedThemeIds.Contains(themeId))
        {
            unlockedThemeIds.Add(themeId);
        }
    }

    public void LockTheme(string themeId)
    {
        unlockedThemeIds.Remove(themeId);
    }

    public void SetCurrentTheme(string themeId)
    {
        currentThemeId = themeId;
    }
}
