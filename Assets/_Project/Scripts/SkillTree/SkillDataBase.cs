using System.Collections.Generic;
using UnityEngine;

public abstract class SkillDataBase : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string skillID;
    public string SkillID => skillID;
    [SerializeField] private string skillName;
    public string SkillName => skillName;
    [TextArea, SerializeField] private string description;
    public string Description => description;
    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;

    [Header("Level Configuration")]
    [SerializeField, Range(0, 10)] private int startingLevel = 0;
    public int StartingLevel => startingLevel;
    [SerializeField, Range(0, 10)] private int maxLevel = 10;
    public int MaxLevel => maxLevel;

    [Header("Per-Level Requirements")]
    [SerializeField] private List<SkillLevelRequirement> levelRequirements = new List<SkillLevelRequirement>();
    public List<SkillLevelRequirement> LevelRequirements => levelRequirements;
    public abstract float GetEffectValueAtLevel(int level);
    public SkillLevelRequirement GetRequirementsForLevel(int level) => levelRequirements.Find(req => req.Level == level);
}
