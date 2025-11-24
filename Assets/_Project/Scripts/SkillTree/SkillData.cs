using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Incremental Game/Skill")]
public class SkillData : ScriptableObject
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

    [Header("Costs")]
    [SerializeField] private int skillPointCost = 1;
    public int SkillPointCost => skillPointCost;
    [SerializeField] private long currencyCost = 0; 
    public long CurrencyCost => currencyCost;

    [Header("Requirements")]
    [SerializeField] private List<SkillData> requiredSkills; 
    public List<SkillData> RequiredSkills => requiredSkills;
    [SerializeField] private int requiredPlayerLevel = 0;
    public int RequiredPlayerLevel => requiredPlayerLevel;

    [Header("Effects")]
    [SerializeField] private SkillEffectType effectType;
    public SkillEffectType EffectType => effectType;
    [SerializeField] private float effectValue;
    public float EffectValue => effectValue;

    [SerializeField] private string effectTarget; // What this skill affects (e.g., "ClickPower", "IdleIncome")
    public string EffectTarget => effectTarget;

    [Header("Visual")]
    [SerializeField] private Color lockedColor = Color.gray;
    public Color LockedColor => lockedColor;
    [SerializeField] private Color unlockedColor = Color.green;
    public Color UnlockedColor => unlockedColor;
    [SerializeField] private Color availableColor = Color.yellow;
    public Color AvailableColor => availableColor;
}
