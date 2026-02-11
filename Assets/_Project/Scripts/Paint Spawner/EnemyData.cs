using System;
using UnityEngine;

[Serializable]
public class EnemyData
{
    [SerializeField] private EnemyDifficulty difficulty;
    [SerializeField, Range(0f,100f)] private float maxHealth;
    [SerializeField, Range(0, 100)] private int cost;
    [SerializeField, Range(0f, 5f)] private float scale;

    [Header("Shield Value")]
    [SerializeField, Range(0f, 100f)] private float chancePercentageToHaveShield;
    [SerializeField, Range(0f, 300f)] private float maxShield;
    [SerializeField, Range(0f, 1f)] private float shieldArmor;
    [SerializeField] private bool canRegenerateShield;
    [SerializeField, Range(0f, 10f)] private float shieldRegenerationRate;
    [SerializeField, Range(0f, 15f)] private float shieldRegenerationDelay;

    [Header("Sigmoid Weight Settings")]
    [SerializeField, Range(0f, 50f)] private float baseWeight;
    [SerializeField, Range(0f, 2f)] private float upgradeInfluenceCoefficient;
    [SerializeField, Range(0, 50)] private int levelThreshold;

    public float MaxHealth => maxHealth;
    public int Cost => cost;
    public float Scale => scale;
    public float ChancePercentageToHaveShield => chancePercentageToHaveShield;
    public float MaxShield => maxShield;
    public float ShieldArmor => shieldArmor;
    public bool CanRegenerateShield => canRegenerateShield;
    public float ShieldRegenerationRate => shieldRegenerationRate;
    public float ShieldRegenerationDelay => shieldRegenerationDelay;
    public float BaseWeight => baseWeight;
    public float UpgradeInfluenceCoefficient => upgradeInfluenceCoefficient;
    public int LevelThreshold => levelThreshold;
    public EnemyDifficulty Difficulty => difficulty;
}
