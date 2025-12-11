using System;

[Serializable]
public class LevelStats
{
    public int TotalDaysPlayed;
    public int TotalPaintBlobsDestroyed;
    public float TotalDamageDealt;
    public float TotalTimeAdded;
    public int TotalCurrencyGained;

    public LevelStats()
    {
        TotalDaysPlayed = 0;
        TotalPaintBlobsDestroyed = 0;
        TotalDamageDealt = 0f;
        TotalTimeAdded = 0f;
        TotalCurrencyGained = 0;
    }

    public void AddDayStats(DayStats dayStats)
    {
        TotalDaysPlayed++;
        TotalPaintBlobsDestroyed += dayStats.PaintBlobsDestroyed;
        TotalDamageDealt += dayStats.TotalDamageDealt;
        TotalTimeAdded += dayStats.TimeAdded;
        TotalCurrencyGained += dayStats.CurrencyGained;
    }
}