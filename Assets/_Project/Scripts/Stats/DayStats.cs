using System;

[Serializable]
public class DayStats
{
    public int PaintBlobsDestroyed;
    public float AutoClickerDamage;
    public float BombDamage;
    public float BrushSwipeDamage;
    public float TotalDamageDealt;
    public float TimeAdded;
    public int TimesTimerIncreased;
    public int AdditionalPaintBlobsSpawned;
    public int CurrencyGained;

    public DayStats()
    {
        PaintBlobsDestroyed = 0;
        AutoClickerDamage = 0f;
        BombDamage = 0f;
        BrushSwipeDamage = 0f;
        TotalDamageDealt = 0f;
        TimeAdded = 0f;
        TimesTimerIncreased = 0;
        AdditionalPaintBlobsSpawned = 0;
        CurrencyGained = 0;
    }
}
