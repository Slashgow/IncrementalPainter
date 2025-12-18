using System;
using UnityEngine;

[Serializable]
public class RankRewardData
{
    [Header("Skill Point Rewards")]
    [Tooltip("Skill points awarded for S rank")]
    [SerializeField, Range(0, 10)] private int sRankSkillPoints = 5;

    [Tooltip("Skill points awarded for A rank")]
    [SerializeField, Range(0, 10)] private int aRankSkillPoints = 3;

    [Tooltip("Skill points awarded for B rank")]
    [SerializeField, Range(0, 10)] private int bRankSkillPoints = 2;

    [Tooltip("Skill points awarded for C rank")]
    [SerializeField, Range(0, 10)] private int cRankSkillPoints = 1;

    [Tooltip("Skill points awarded for D rank")]
    [SerializeField, Range(0, 10)] private int dRankSkillPoints = 0;

    [Header("Currency Rewards (Optional)")]
    [Tooltip("Bonus currency for S rank")]
    [SerializeField, Range(0, 100000)] private int sRankCurrency = 1000;

    [Tooltip("Bonus currency for A rank")]
    [SerializeField, Range(0, 100000)] private int aRankCurrency = 500;

    [Tooltip("Bonus currency for B rank")]
    [SerializeField, Range(0, 100000)] private int bRankCurrency = 250;

    [Tooltip("Bonus currency for C rank")]
    [SerializeField, Range(0, 100000)] private int cRankCurrency = 100;

    [Tooltip("Bonus currency for D rank")]
    [SerializeField, Range(0, 100000)] private int dRankCurrency = 0;

    public int GetSkillPointReward(LevelRank rank)
    {
        return rank switch
        {
            LevelRank.S => sRankSkillPoints,
            LevelRank.A => aRankSkillPoints,
            LevelRank.B => bRankSkillPoints,
            LevelRank.C => cRankSkillPoints,
            LevelRank.D => dRankSkillPoints,
            _ => 0
        };
    }

    public int GetCurrencyReward(LevelRank rank)
    {
        return rank switch
        {
            LevelRank.S => sRankCurrency,
            LevelRank.A => aRankCurrency,
            LevelRank.B => bRankCurrency,
            LevelRank.C => cRankCurrency,
            LevelRank.D => dRankCurrency,
            _ => 0
        };
    }
}