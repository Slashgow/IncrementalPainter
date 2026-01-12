using System;
using UnityEngine;

[Serializable]
public class RankColorsId
{
    [SerializeField] private ColorId sRankColorID = ColorId.RANK_S;
    [SerializeField] private ColorId aRankColorID = ColorId.RANK_A;
    [SerializeField] private ColorId bRankColorID = ColorId.RANK_B;
    [SerializeField] private ColorId cRankColorID = ColorId.RANK_C;
    [SerializeField] private ColorId dRankColorID = ColorId.RANK_D;
    public Color GetColorForRank(LevelRank rank)
    {
        return rank switch
        {
            LevelRank.S => ThemeColorManager.Instance.GetColor(sRankColorID),
            LevelRank.A => ThemeColorManager.Instance.GetColor(aRankColorID),
            LevelRank.B => ThemeColorManager.Instance.GetColor(bRankColorID),
            LevelRank.C => ThemeColorManager.Instance.GetColor(cRankColorID),
            LevelRank.D => ThemeColorManager.Instance.GetColor(dRankColorID),
            _ => Color.white,
        };
    }
}
