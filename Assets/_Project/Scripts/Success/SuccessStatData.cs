using System;

[Serializable]
public class SuccessStatData
{
    public int countColorPaletteUnlocked;
    public int countTamponUnlocked;
    public int paintRankSFinish;
    public int upgradeBought;
    public int bestDamageSingleHit;
    public int bestDamageOneSession;
    public int bestCurrencyGainedOneSession;
    public int bestNumberOfBlobAtSessionStart;

    public SuccessStatData()
    {
        countColorPaletteUnlocked = 0;
        countTamponUnlocked = 0;
        paintRankSFinish = 0;
        upgradeBought = 0;
        bestDamageSingleHit = 0;
        bestDamageOneSession = 0;
        bestCurrencyGainedOneSession = 0;
        bestNumberOfBlobAtSessionStart = 0;
    }

    public SuccessStatData(SuccessStatData successStatData)
    {
        this.countColorPaletteUnlocked = successStatData.countColorPaletteUnlocked;
        this.countTamponUnlocked = successStatData.countTamponUnlocked;
        this.paintRankSFinish = successStatData.paintRankSFinish;
        this.upgradeBought = successStatData.upgradeBought;
        this.bestDamageSingleHit = successStatData.bestDamageSingleHit;
        this.bestDamageOneSession = successStatData.bestDamageOneSession;
        this.bestCurrencyGainedOneSession = successStatData.bestCurrencyGainedOneSession;
        this.bestNumberOfBlobAtSessionStart = successStatData.bestNumberOfBlobAtSessionStart;
    }
}
