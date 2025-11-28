using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillLevelRequirement
{
    [SerializeField] private int level;
    public int Level => level;

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