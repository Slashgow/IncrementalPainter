using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIThemeColorManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown themeDropdown;

    private void Start()
    {
        PopulateDropdown();
        LoadSettings();
    }
    private void OnEnable() => ColorThemeUnlockManager.OnThemeUnlocked += OnThemeUnlocked;
    private void OnDisable() => ColorThemeUnlockManager.OnThemeUnlocked -= OnThemeUnlocked;
    private void OnThemeUnlocked(string themeId) => PopulateDropdown();

    public void PopulateDropdown()
    {
        themeDropdown.ClearOptions();
        var themeOptions = new List<string>();
        var unlockedThemes = ThemeColorManager.Instance.GetUnlockedThemes();
        foreach (var theme in unlockedThemes)
        {
            themeOptions.Add(theme.ColorTheme.ThemeName);
        }
        themeDropdown.AddOptions(themeOptions);
    }

    public void LoadSettings()
    {
        int currentThemeIndex = ThemeColorManager.Instance.GetCurrentThemeIndex();
        themeDropdown.value = currentThemeIndex >= 0 ? currentThemeIndex : 0;
    }

    public void SetTheme(int index)
    {
        ThemeColorManager.Instance.SetThemeByIndex(index);
    }
}
