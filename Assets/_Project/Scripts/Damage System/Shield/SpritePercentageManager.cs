using System;
using UnityEngine;

[Serializable]
public class SpritePercentageManager
{
    [SerializeField] private SpriteByPercentage[] spriteByPercentages;
    public Sprite GetSpriteForPercentage(float percentage)
    {
        foreach (var entry in spriteByPercentages)
        {
            if (percentage >= entry.MinPercentage)
            {
                return entry.Sprite;
            }
        }
        return spriteByPercentages[0].Sprite;
    }
}