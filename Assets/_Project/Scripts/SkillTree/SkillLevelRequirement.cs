using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillLevelRequirement
{
    [SerializeField] private int level;
    public int Level => level;

    [SerializeField] private List<SkillDataBase> requiredSkills;
    public List<SkillDataBase> RequiredSkills => requiredSkills;

    [SerializeField] private int currencyCost = 0;
    public int CurrencyCost => currencyCost;
}
