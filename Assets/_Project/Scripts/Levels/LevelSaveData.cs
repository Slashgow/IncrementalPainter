using System;
using SaveUtility;

[Serializable]
public class LevelSaveData
{
    public string ID => SavePath.GetLevelID(author, title);

    public bool isDone;
    public float completionRatio;
    public string author;
    public string title;
    public bool isUnlocked;
    public int daysToComplete;
    public LevelRank bestRank;
    public LevelRank claimedRewardRank;
    public int currentDay;

    public LevelSaveData(bool isDone, float completionRatio, string author, string title,
                         bool isUnlocked = false, int daysToComplete = 0, LevelRank bestRank = LevelRank.None, 
                         bool hasClaimedReward = false, int currentDay = 0)
    {
        this.isDone = isDone;
        this.completionRatio = completionRatio;
        this.author = author;
        this.title = title;
        this.isUnlocked = isUnlocked;
        this.daysToComplete = daysToComplete;
        this.bestRank = bestRank;
        this.claimedRewardRank = hasClaimedReward ? bestRank : LevelRank.None;
        this.currentDay = currentDay;
    }

    public void UpdateRank(int newDaysToComplete, LevelRankThresholds thresholds)
    {
        if (newDaysToComplete <= 0)
            return;

        daysToComplete = newDaysToComplete;
        LevelRank newRank = thresholds.GetRankForDays(newDaysToComplete);

        if (newRank > bestRank)
            bestRank = newRank;
    }
}
