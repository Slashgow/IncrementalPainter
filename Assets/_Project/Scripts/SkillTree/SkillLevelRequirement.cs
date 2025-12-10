using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillLevelRequirement
{
    [SerializeField] private int level;
    public int Level => level;

    [Header("Skill Point Cost")]
    [Tooltip("Number of skill points required to unlock this level")]
    [SerializeField] private int skillPointCost = 0;
    public int SkillPointCost => skillPointCost;

    [SerializeField] private List<RequiredSkillWithLevel> requiredSkills;
    public List<RequiredSkillWithLevel> RequiredSkills => requiredSkills;
}

[System.Serializable]
public class RequiredSkillWithLevel
{
    [SerializeField] private SkillDataBase skillData;
    public SkillDataBase SkillData => skillData;

    [SerializeField, Range(0, 10)] private int requiredLevel = 1;
    public int RequiredLevel => requiredLevel;
}