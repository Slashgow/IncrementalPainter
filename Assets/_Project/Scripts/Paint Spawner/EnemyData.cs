using System;
using UnityEngine;

[Serializable]
public class EnemyData
{
    [SerializeField] private EnemyDifficulty difficulty;
    [SerializeField, Range(0f,100f)] private float maxHealth;
    [SerializeField, Range(0, 100)] private int cost;
    [SerializeField, Range(0f, 5f)] private float scale;


    [Header("Sigmoid Weight Settings")]
    [SerializeField, Range(0f, 50f)] private float baseWeight;
    [SerializeField, Range(0f, 2f)] private float upgradeInfluenceCoefficient;
    [SerializeField, Range(0, 50)] private int levelThreshold;
    public float MaxHealth => maxHealth;
    public int Cost => cost;
    public float Scale => scale;
    public float BaseWeight => baseWeight;
    public float UpgradeInfluenceCoefficient => upgradeInfluenceCoefficient;
    public int LevelThreshold => levelThreshold;
    public EnemyDifficulty Difficulty => difficulty;
}
