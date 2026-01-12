using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorTheme", menuName = "InkolorGames/Color Theme")]
public class ColorTheme : ScriptableObject
{
    [SerializeField] private string themeName;
    [SerializeField] private List<ColorEntry> colors = new List<ColorEntry>();

    public string ThemeName => themeName;
    public string ThemeId => themeName.Replace(" ", "_").ToLower();

    [Serializable]
    public class ColorEntry
    {
        public ColorId colorId;
        public Color color;
    }

    public Color GetColor(ColorId id)
    {
        foreach (var entry in colors)
        {
            if (entry.colorId == id)
                return entry.color;
        }

        Debug.LogWarning($"Color with ID {id} not found in theme {themeName}. Returning magenta.");
        return Color.magenta;
    }

    public bool TryGetColor(ColorId id, out Color color)
    {
        foreach (var entry in colors)
        {
            if (entry.colorId == id)
            {
                color = entry.color;
                return true;
            }
        }

        color = Color.magenta;
        return false;
    }
}
