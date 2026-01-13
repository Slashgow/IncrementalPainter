using System.Collections.Generic;
using UnityEngine;

public class ColorThemeUnlockManager : MonoBehaviour
{
    [SerializeField] private List<UnlockableColorTheme> unlockableThemes = new List<UnlockableColorTheme>();
    [SerializeField] private bool checkUnlocksOnStart = true;
    [SerializeField] private bool autoUnlockThemes = true;

    public static event System.Action<string> OnThemeUnlocked;
    public static event System.Action<string> OnThemeLocked;

    private void OnEnable() => LevelManager.OnEndLevel += CheckAllUnlocks;
    private void OnDisable() => LevelManager.OnEndLevel -= CheckAllUnlocks;
    private void Start()
    {
        if (checkUnlocksOnStart)
            CheckAllUnlocks();
    }

    public void CheckAllUnlocks()
    {
        foreach (var unlockableTheme in unlockableThemes)
        {
            if (autoUnlockThemes)
            {
                unlockableTheme.CheckUnlockCondition();
            }
        }
    }

    public void CheckThemeUnlock(string themeId)
    {
        UnlockableColorTheme unlockableTheme = GetUnlockableTheme(themeId);

        if (unlockableTheme != null)
        {
            unlockableTheme.CheckUnlockCondition();
        }
    }

    public void CheckThemeUnlock(ColorTheme theme)
    {
        if (theme != null && !string.IsNullOrEmpty(theme.ThemeId))
        {
            CheckThemeUnlock(theme.ThemeId);
        }
    }

    public UnlockableColorTheme GetUnlockableTheme(string themeId)
    {
        return unlockableThemes.Find(ut => ut.ColorTheme != null && ut.ColorTheme.ThemeId == themeId);
    }

    public UnlockableColorTheme GetUnlockableTheme(ColorTheme theme)
    {
        if (theme == null)
            return null;

        return GetUnlockableTheme(theme.ThemeId);
    }

    public List<UnlockableColorTheme> GetAllUnlockableThemes()
    {
        return new List<UnlockableColorTheme>(unlockableThemes);
    }

    public List<UnlockableColorTheme> GetLockedUnlockableThemes()
    {
        List<UnlockableColorTheme> locked = new List<UnlockableColorTheme>();

        foreach (var unlockableTheme in unlockableThemes)
        {
            if (!unlockableTheme.IsUnlocked)
            {
                locked.Add(unlockableTheme);
            }
        }

        return locked;
    }

    public List<UnlockableColorTheme> GetUnlockedUnlockableThemes()
    {
        List<UnlockableColorTheme> unlocked = new List<UnlockableColorTheme>();

        foreach (var unlockableTheme in unlockableThemes)
        {
            if (unlockableTheme.IsUnlocked)
            {
                unlocked.Add(unlockableTheme);
            }
        }

        return unlocked;
    }

    public bool IsThemeUnlockable(string themeId)
    {
        UnlockableColorTheme unlockableTheme = GetUnlockableTheme(themeId);
        return unlockableTheme != null;
    }

    public bool IsThemeUnlockable(ColorTheme theme)
    {
        if (theme == null)
            return false;

        return IsThemeUnlockable(theme.ThemeId);
    }

    public string GetUnlockDescription(string themeId)
    {
        UnlockableColorTheme unlockableTheme = GetUnlockableTheme(themeId);

        if (unlockableTheme != null)
        {
            return unlockableTheme.GetUnlockDescription();
        }

        return "Theme not found";
    }

    public string GetUnlockDescription(ColorTheme theme)
    {
        if (theme == null)
            return "Theme is null";

        return GetUnlockDescription(theme.ThemeId);
    }

    // Theme unlock/lock with save system
    public void UnlockTheme(string themeId)
    {
        ColorThemeSaveData saveData = GameSaveManager.Instance.LoadColorThemes();

        if (!saveData.IsThemeUnlocked(themeId))
        {
            saveData.UnlockTheme(themeId);
            GameSaveManager.Instance.SaveColorThemes(saveData);
            OnThemeUnlocked?.Invoke(themeId);
            Debug.Log($"Theme '{themeId}' unlocked!");
        }
    }

    public void UnlockTheme(ColorTheme theme)
    {
        if (theme != null && !string.IsNullOrEmpty(theme.ThemeId))
        {
            UnlockTheme(theme.ThemeId);
        }
    }

    public void LockTheme(string themeId)
    {
        ColorThemeSaveData saveData = GameSaveManager.Instance.LoadColorThemes();

        if (saveData.IsThemeUnlocked(themeId))
        {
            saveData.LockTheme(themeId);
            GameSaveManager.Instance.SaveColorThemes(saveData);
            OnThemeLocked?.Invoke(themeId);

            // If current theme is locked, switch to default
            if (ThemeColorManager.Instance.CurrentTheme != null &&
                ThemeColorManager.Instance.CurrentTheme.ThemeId == themeId)
            {
                if (ThemeColorManager.Instance.DefaultTheme != null)
                {
                    ThemeColorManager.Instance.SetTheme(ThemeColorManager.Instance.DefaultTheme);
                }
            }

            Debug.Log($"Theme '{themeId}' locked!");
        }
    }

    public void LockTheme(ColorTheme theme)
    {
        if (theme != null && !string.IsNullOrEmpty(theme.ThemeId))
        {
            LockTheme(theme.ThemeId);
        }
    }

    public bool IsThemeUnlocked(string themeId)
    {
        ColorThemeSaveData saveData = GameSaveManager.Instance.LoadColorThemes();
        return saveData.IsThemeUnlocked(themeId);
    }

    public bool IsThemeUnlocked(ColorTheme theme)
    {
        if (theme == null || string.IsNullOrEmpty(theme.ThemeId))
            return false;

        return IsThemeUnlocked(theme.ThemeId);
    }

    public List<ColorTheme> GetUnlockedThemes()
    {
        ColorThemeSaveData saveData = GameSaveManager.Instance.LoadColorThemes();
        List<ColorTheme> unlockedThemes = new List<ColorTheme>();

        foreach (var unlockableTheme in unlockableThemes)
        {
            if (unlockableTheme.ColorTheme != null &&
                saveData.IsThemeUnlocked(unlockableTheme.ColorTheme.ThemeId))
            {
                unlockedThemes.Add(unlockableTheme.ColorTheme);
            }
        }

        return unlockedThemes;
    }

    public List<ColorTheme> GetLockedThemes()
    {
        ColorThemeSaveData saveData = GameSaveManager.Instance.LoadColorThemes();
        List<ColorTheme> lockedThemes = new List<ColorTheme>();

        foreach (var unlockableTheme in unlockableThemes)
        {
            if (unlockableTheme.ColorTheme != null &&
                !saveData.IsThemeUnlocked(unlockableTheme.ColorTheme.ThemeId))
            {
                lockedThemes.Add(unlockableTheme.ColorTheme);
            }
        }

        return lockedThemes;
    }

    public bool CanSetTheme(string themeId)
    {
        return IsThemeUnlocked(themeId);
    }

    public bool CanSetTheme(ColorTheme theme)
    {
        if (theme == null)
            return false;

        return CanSetTheme(theme.ThemeId);
    }

    public bool TrySetTheme(string themeId)
    {
        if (CanSetTheme(themeId))
        {
            ThemeColorManager.Instance.SetThemeById(themeId);
            return true;
        }

        Debug.LogWarning($"Cannot set theme '{themeId}' - it is locked!");
        return false;
    }

    public bool TrySetTheme(ColorTheme theme)
    {
        if (theme == null)
            return false;

        return TrySetTheme(theme.ThemeId);
    }

    // Force unlock a theme (for testing or admin purposes)
    public void ForceUnlock(string themeId)
    {
        UnlockableColorTheme unlockableTheme = GetUnlockableTheme(themeId);

        if (unlockableTheme != null)
        {
            unlockableTheme.Unlock();
        }
    }

    public void ForceUnlock(ColorTheme theme)
    {
        if (theme != null)
        {
            ForceUnlock(theme.ThemeId);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Check All Unlocks Now")]
    private void CheckAllUnlocksEditor()
    {
        CheckAllUnlocks();
    }

    [ContextMenu("Force Unlock All Themes")]
    private void ForceUnlockAllEditor()
    {
        foreach (var unlockableTheme in unlockableThemes)
        {
            unlockableTheme.Unlock();
        }
    }
#endif
}
