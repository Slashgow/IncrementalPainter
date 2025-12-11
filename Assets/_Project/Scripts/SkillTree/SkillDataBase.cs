using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public abstract class SkillDataBase : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string skillID;
    public string SkillID => skillID;
    [SerializeField] private string skillName;
    public string SkillName => skillName;
    [SerializeField] private LocalizedString description;
    public LocalizedString Description => description;
    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;

    [Header("Currency Cost Configuration")]
    [SerializeField] private FunctionPower costFunction;
    public FunctionPower CostFunction => costFunction;

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
    public int GetCostForLevel(int level) => Mathf.RoundToInt(costFunction.Evaluate(level));
    public int GetSkillPointCostForLevel(int level)
    {
        var levelRequirement = GetRequirementsForLevel(level);

        if(levelRequirement != null)
            return levelRequirement.SkillPointCost;

        return -1;
    }
}
