using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillLevelRequirement
{
    [SerializeField] private int level;
    public int Level => level;

    [SerializeField] private List<SkillDataBase> requiredSkills;
    public List<SkillDataBase> RequiredSkills => requiredSkills;

    [SerializeField] private int skillPointCost = 1;
    public int SkillPointCost => skillPointCost;

    [SerializeField] private long currencyCost = 0;
    public long CurrencyCost => currencyCost;
}
