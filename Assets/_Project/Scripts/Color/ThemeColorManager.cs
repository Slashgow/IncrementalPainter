using System;
using System.Collections.Generic;
using System.Linq;
using inkolorgames;
using UnityEngine;

public class ThemeColorManager : PersistentMonoSingleton<ThemeColorManager>
{
    [Header("Unlock System")]
    [SerializeField] private ColorThemeUnlockManager unlockManager;
    public ColorThemeUnlockManager UnlockManager => unlockManager;

    [Header("Color Themes")]
    [SerializeField] private List<ColorTheme> availableThemes;
    [SerializeField] private ColorTheme currentTheme;
    [SerializeField] private ColorTheme defaultTheme;

    public ColorTheme DefaultTheme => defaultTheme;

    public static event Action<ColorTheme> OnThemeChanged;
    public static event Action<ColorId, Color> OnColorChanged;

    public ColorTheme CurrentTheme => currentTheme;

    private static readonly ColorId[] paintColorsIDs = new ColorId[]
    {
        ColorId.PRIMARY,
        ColorId.SECONDARY,
        ColorId.THIRD,
        ColorId.FOURTH,
        ColorId.FITH,
        ColorId.SIXTH
    };

    private void Start()
    {
        LoadThemeData();
    }
    private void LoadThemeData()
    {
        ColorThemeSaveData saveData = GameSaveManager.Instance.LoadColorThemes();

        if (defaultTheme != null && !string.IsNullOrEmpty(defaultTheme.ThemeId))
        {
            if (!saveData.IsThemeUnlocked(defaultTheme.ThemeId))
            {
                saveData.UnlockTheme(defaultTheme.ThemeId);
                GameSaveManager.Instance.SaveColorThemes(saveData);
            }
        }

        if (!string.IsNullOrEmpty(saveData.currentThemeId))
        {
            ColorTheme savedTheme = GetThemeById(saveData.currentThemeId);
            if (savedTheme != null && saveData.IsThemeUnlocked(saveData.currentThemeId))
            {
                SetTheme(savedTheme);
                //currentTheme = savedTheme;
            }
        }

        if (currentTheme == null)
        {
           // currentTheme = defaultTheme != null ? defaultTheme : (availableThemes.Count > 0 ? availableThemes[0] : null);
           if(defaultTheme != null)
                SetTheme(defaultTheme);
            else
            {
                if (availableThemes.Count > 0)
                    SetTheme(availableThemes[0]);
            }
        }
    }

    public Color GetColor(ColorId id)
    {
        if (currentTheme == null)
        {
            Debug.LogError("No current theme set!");
            return Color.magenta;
        }

        return currentTheme.GetColor(id);
    }

    public Color GetRandomPaintColor()
    {
        ColorId randomId = paintColorsIDs[UnityEngine.Random.Range(0, paintColorsIDs.Length)];
        return GetColor(randomId);
    }

    public Color GetRandomColor()
    {
        if (currentTheme == null)
        {
            Debug.LogError("No current theme set!");
            return Color.magenta;
        }

        var colors = Enum.GetValues(typeof(ColorId));
        ColorId randomId = (ColorId)colors.GetValue(UnityEngine.Random.Range(0, colors.Length));
        return currentTheme.GetColor(randomId);
    }

    public bool TryGetColor(ColorId id, out Color color)
    {
        if (currentTheme == null)
        {
            color = Color.magenta;
            return false;
        }

        return currentTheme.TryGetColor(id, out color);
    }

    public void SetTheme(ColorTheme theme)
    {
        if (theme == null)
        {
            Debug.LogError("Cannot set null theme!");
            return;
        }

        if (!IsThemeUnlocked(theme))
        {
            Debug.LogWarning($"Theme '{theme.ThemeName}' is locked — cannot set.");
            return;
        }

        currentTheme = theme;

        ColorThemeSaveData saveData = GameSaveManager.Instance.LoadColorThemes();
        saveData.SetCurrentTheme(theme.ThemeId);
        GameSaveManager.Instance.SaveColorThemes(saveData);

        OnThemeChanged?.Invoke(currentTheme);
    }

    public void SetThemeByName(string themeName)
    {
        ColorTheme theme = availableThemes.Find(t => t.ThemeName == themeName);

        if (theme != null)
        {
            SetTheme(theme);
        }
        else
        {
            Debug.LogError($"Theme with name '{themeName}' not found!");
        }
    }

    public void SetThemeById(string themeId)
    {
        ColorTheme theme = GetThemeById(themeId);
        if (theme != null)
        {
            SetTheme(theme);
        }
        else
        {
            Debug.LogError($"Theme with ID '{themeId}' not found!");
        }
    }

    public void SetThemeByIndex(int index)
    {
        if (index >= 0 && index < availableThemes.Count)
        {
            SetTheme(availableThemes[index]);
        }
        else
        {
            Debug.LogError($"Theme index {index} out of range!");
        }
    }
    public ColorTheme GetThemeById(string themeId) => availableThemes.Find(t => t.ThemeId == themeId);
    public int GetThemeIndex(ColorTheme theme)
    {
        var unlockedThemes = GetUnlockedThemes();
        Unlockable<ColorTheme> colorTheme = unlockedThemes.FirstOrDefault(item => item.Item == theme);
        return unlockedThemes.IndexOf(colorTheme);
    }

    public int GetCurrentThemeIndex() => GetThemeIndex(currentTheme);
    public List<ColorTheme> GetAvailableThemes() => new List<ColorTheme>(availableThemes);
    public void OverrideColor(ColorId id, Color color) => OnColorChanged?.Invoke(id, color);
    public bool IsThemeUnlocked(string themeId) => unlockManager != null && unlockManager.IsItemUnlocked(themeId);
    public bool IsThemeUnlocked(ColorTheme theme) => theme != null && IsThemeUnlocked(theme.ThemeId);
    public void UnlockTheme(string themeId)
    {
        if (unlockManager != null)
            unlockManager.UnlockItem(themeId);
    }
    public void UnlockTheme(ColorTheme theme)
    {
        if (theme != null)
            UnlockTheme(theme.ThemeId);
    }

    public void LockTheme(string themeId)
    {
        if (unlockManager != null)
            unlockManager.LockItem(themeId);
    }

    public string GetUnlockDescription(string themeId)
    {
        return unlockManager != null ?
               unlockManager.GetUnlockDescription(themeId) :
               "Unlock system not available";
    }

    public List<Unlockable<ColorTheme>> GetLockedUnlockableThemes()
    {
        return unlockManager != null ?
               unlockManager.GetLockedUnlockables() :
               new List<Unlockable<ColorTheme>>();
    }

    public List<Unlockable<ColorTheme>> GetUnlockedThemes()
    {
        return unlockManager != null ?
               unlockManager.GetUnlockedUnlockables() :
               new List<Unlockable<ColorTheme>>();
    }

#if UNITY_EDITOR
    [ContextMenu("Change to next theme")]
    private void ChangeToNextTheme()
    {
        SetThemeByIndex((GetThemeIndex(CurrentTheme) + 1 )% availableThemes.Count );
    }
#endif
}
