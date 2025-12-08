using System;
using UnityEngine;

[Serializable]
public class EnemyData
{
    [SerializeField] private EnemyDifficulty difficulty;
    [SerializeField, Range(0f,20f)] private float maxHealth;
    [SerializeField, Range(0, 100)] private int cost;
    [SerializeField, Range(0f, 5f)] private float scale;
    [SerializeField, Range(0f, 50f)] private float baseWeigth;
    [SerializeField, Range(0f, 3f)] private float upgradeWeightExponent;
    [SerializeField, Range(0f, 3f)] private float levelWeightExponent;
    public float MaxHealth => maxHealth;
    public int Cost => cost;
    public float Scale => scale;
    public float BaseWeigth => baseWeigth;
    public float UpgradeWeightExponent => upgradeWeightExponent;
    public float LevelWeightExponent => levelWeightExponent;
    public EnemyDifficulty Difficulty => difficulty;
}
