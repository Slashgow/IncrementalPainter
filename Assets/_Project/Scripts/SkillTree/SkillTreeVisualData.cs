using System;
using UnityEngine;

[Serializable]
public class SkillTreeVisualData
{
    [Header("Colors")]
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color lockedColorDarker = Color.gray;
    public Color LockedColor => lockedColor;
    public Color LockedColorDarker => lockedColorDarker;

    [SerializeField] private Color unlockedColor = Color.green;
    [SerializeField] private Color unlockedColorDarker = Color.green;
    public Color UnlockedColor => unlockedColor;
    public Color UnlockedColorDarker => unlockedColorDarker;

    [SerializeField] private Color availableColor = Color.yellow;
    [SerializeField] private Color availableColorDarker = Color.yellow;
    public Color AvailableColor => availableColor;
    public Color AvailableColorDarker => availableColorDarker;

    [SerializeField] private Color skillPointColor = Color.white;
    public Color SkillPointColor => skillPointColor;

    [SerializeField] private Color maxLevelColor = Color.white;
    public Color MaxLevelColor => maxLevelColor;

    [Header("Sprites")]
    [SerializeField] private Sprite backgroundSpriteLocked;
    [SerializeField] private Sprite backgroundSpriteUnlocked;

    public Sprite BackgroundSpriteLocked => backgroundSpriteLocked;
    public Sprite BackgroundSpriteUnlocked => backgroundSpriteUnlocked;

}
