using System;
using UnityEngine;

[Serializable]
public class SkillTreeVisualData
{
    [Header("Colors")]
    [SerializeField] private ColorId lockedColorID = ColorId.SKILL_NODE_LOCKED;
    [SerializeField] private ColorId lockedColorDarkerID = ColorId.SKILL_NODE_LOCKED_DARKER;
    public Color LockedColor => ThemeColorManager.Instance.GetColor(lockedColorID);
    public Color LockedColorDarker => ThemeColorManager.Instance.GetColor(lockedColorDarkerID);

    [SerializeField] private ColorId unlockedColorID = ColorId.SKILL_NODE_UNLOCKED;
    [SerializeField] private ColorId unlockedColorDarkerID = ColorId.SKILL_NODE_UNLOCKED_DARKER;
    public Color UnlockedColor => ThemeColorManager.Instance.GetColor(unlockedColorID);
    public Color UnlockedColorDarker => ThemeColorManager.Instance.GetColor(unlockedColorDarkerID);

    [SerializeField] private ColorId availableColorID = ColorId.SKILL_NODE_AVAILABLE;
    [SerializeField] private ColorId availableColorDarkerID = ColorId.SKILL_NODE_AVAILABLE_DARKER;
    public Color AvailableColor => ThemeColorManager.Instance.GetColor(availableColorID);
    public Color AvailableColorDarker => ThemeColorManager.Instance.GetColor(availableColorDarkerID);

    [SerializeField] private ColorId skillPointColorID = ColorId.SKILL_NODE_SKILL_POINT;
    public Color SkillPointColor => ThemeColorManager.Instance.GetColor(skillPointColorID);

    [SerializeField] private ColorId maxLevelColorID = ColorId.SKILL_NODE_MAX_LEVEL;
    public Color MaxLevelColor => ThemeColorManager.Instance.GetColor(maxLevelColorID);

    [Header("Sprites")]
    [SerializeField] private Sprite backgroundSpriteLocked;
    [SerializeField] private Sprite backgroundSpriteUnlocked;

    public Sprite BackgroundSpriteLocked => backgroundSpriteLocked;
    public Sprite BackgroundSpriteUnlocked => backgroundSpriteUnlocked;

}
