using UnityEngine;

/// <summary>
/// Color Theme specific unlock manager - inherits from generic UnlockManager
/// </summary>
public class ColorThemeUnlockManager : UnlockManager<ColorTheme>
{
    private const string SAVE_KEY = "ColorThemes";

    protected override void OnEnable()
    {
        base.OnEnable();
        LevelManager.OnEndLevel += CheckAllUnlocks;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LevelManager.OnEndLevel -= CheckAllUnlocks;
    }

    protected override string GetSaveKey()
    {
        return SAVE_KEY;
    }

    protected override UnlockableSaveData LoadSaveData()
    {
        // Load from your save system
        ColorThemeSaveData oldSaveData = GameSaveManager.Instance.LoadColorThemes();

        // Convert to generic save data
        UnlockableSaveData saveData = new UnlockableSaveData();
        saveData.unlockedItemIds = new System.Collections.Generic.List<string>(oldSaveData.unlockedThemeIds);

        return saveData;
    }

    protected override void SaveData(UnlockableSaveData saveData)
    {
        // Convert back to ColorThemeSaveData for compatibility
        ColorThemeSaveData oldSaveData = GameSaveManager.Instance.LoadColorThemes();
        oldSaveData.unlockedThemeIds = new System.Collections.Generic.List<string>(saveData.unlockedItemIds);

        GameSaveManager.Instance.SaveColorThemes(oldSaveData);
    }

    protected override void OnItemLockedCallback(string itemId)
    {
        base.OnItemLockedCallback(itemId);

        // If current theme is locked, switch to default
        if (ThemeColorManager.Instance.CurrentTheme != null &&
            ThemeColorManager.Instance.CurrentTheme.ItemId == itemId)
        {
            if (ThemeColorManager.Instance.DefaultTheme != null)
            {
                ThemeColorManager.Instance.SetTheme(ThemeColorManager.Instance.DefaultTheme);
            }
        }
    }

    // Convenience methods that maintain backward compatibility
    public bool CanSetTheme(string themeId) => IsItemUnlocked(themeId);
    public bool CanSetTheme(ColorTheme theme) => theme != null && CanSetTheme(theme.ItemId);

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

        return TrySetTheme(theme.ItemId);
    }
}