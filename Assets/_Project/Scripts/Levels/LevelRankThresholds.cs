using System;
using UnityEngine;

[Serializable]
public class LevelRankThresholds
{
    [Header("Days Required for Each Rank")]
    [Tooltip("Complete in this many days or less for S rank")]
    [SerializeField, Range(0,30)] private int sRankDays = 3;

    [Tooltip("Complete in this many days or less for A rank")]
    [SerializeField, Range(0, 30)] private int aRankDays = 5;

    [Tooltip("Complete in this many days or less for B rank")]
    [SerializeField, Range(0, 30)] private int bRankDays = 7;

    [Tooltip("Complete in this many days or less for C rank")]
    [SerializeField, Range(0, 30)] private int cRankDays = 10;

    // Anything above C rank days = D rank

    public LevelRank GetRankForDays(int daysToComplete)
    {
        if (daysToComplete <= 0)
            return LevelRank.None;

        if (daysToComplete <= sRankDays)
            return LevelRank.S;
        if (daysToComplete <= aRankDays)
            return LevelRank.A;
        if (daysToComplete <= bRankDays)
            return LevelRank.B;
        if (daysToComplete <= cRankDays)
            return LevelRank.C;

        return LevelRank.D;
    }

    public int GetDaysRequiredForRank(LevelRank rank)
    {
        return rank switch
        {
            LevelRank.S => sRankDays,
            LevelRank.A => aRankDays,
            LevelRank.B => bRankDays,
            LevelRank.C => cRankDays,
            _ => int.MaxValue
        };
    }
}