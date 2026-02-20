using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIThemeColorManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown themeDropdown;

    private List<string> unlockedThemesID = new List<string>();
    private void Start()
    {
        ThemeColorManager.Instance.UnlockManager.OnItemUnlocked += OnThemeUnlocked;
        PopulateDropdown();
        LoadSettings();
    }
    private void OnDisable()
    {
        if(ThemeColorManager.HasInstance)
            ThemeColorManager.Instance.UnlockManager.OnItemUnlocked -= OnThemeUnlocked;
    }

    private void OnThemeUnlocked(string themeId) => PopulateDropdown();

    public void PopulateDropdown()
    {
        themeDropdown.ClearOptions();
        unlockedThemesID.Clear();
        var themeOptions = new List<string>();
        var unlockedThemes = ThemeColorManager.Instance.GetUnlockedThemes();
        foreach (var theme in unlockedThemes)
        {
            themeOptions.Add(theme.Item.ThemeName);
            unlockedThemesID.Add(theme.Item.ThemeId);
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
        if(index < 0 || index >= unlockedThemesID.Count)
        {
            Debug.LogError($"Invalid theme index: {index}");
            return;
        }

        string id = unlockedThemesID[index];
        ThemeColorManager.Instance.SetThemeById(id);
    }
}
