using System;
using UnityEngine;

public class LevelCompletionTracker : MonoBehaviour
{
    [SerializeField] private inkolorgames.Logger logger;

    private LevelData currentLevelData;
    private bool isTracking;
    public bool IsTracking => isTracking;

    public void StartTracking(LevelData levelData)
    {
        if (levelData == null)
        {
            logger.LogError("Cannot start tracking: LevelData is null", this);
            return;
        }

        currentLevelData = levelData;
        isTracking = true;
        logger.Log($"Started tracking level '{levelData.LevelTitle}'", this);
    }

    public void StopTrackingAndSaveRank(bool levelCompleted)
    {
        if (!isTracking || currentLevelData == null)
            return;

        if (!levelCompleted)
        {
            isTracking = false;
            return;
        }

        int finalDays = Mathf.Max(1, LevelStatsTracker.Instance.TotalLevelStats.TotalDaysPlayed);
        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(currentLevelData.LevelAuthor, currentLevelData.LevelTitle);
        LevelRank newRank = currentLevelData.GetRankForDays(finalDays);
        bool shouldUpdateRank = saveData.bestRank < newRank;

        if (shouldUpdateRank)
        {
            LevelRank previousBestRank = saveData.bestRank;

            saveData.daysToComplete = finalDays;
            saveData.bestRank = newRank;

            AwardRankRewardDifference(previousBestRank, newRank);
            logger.Log($"NEW BEST! Level '{currentLevelData.LevelTitle}' completed in {finalDays} days. Rank: {newRank.GetDisplayName()}", this);
        }
        else
        {
            logger.Log($"Level '{currentLevelData.LevelTitle}' completed in {finalDays} days. Rank: {newRank.GetDisplayName()} (Best: {saveData.bestRank.GetDisplayName()})", this);
        }

        saveData.isDone = true;

        GameSaveManager.Instance.SaveLevelData(
            saveData.isDone,
            saveData.completionRatio,
            saveData.author,
            saveData.title,
            saveData.isUnlocked,
            saveData.daysToComplete,
            saveData.bestRank
        );

        isTracking = false;
    }

    public LevelRank GetCurrentProjectedRank()
    {
        if (!isTracking || currentLevelData == null)
            return LevelRank.None;

        return currentLevelData.GetRankForDays(Mathf.Max(1, LevelStatsTracker.Instance.TotalLevelStats.TotalDaysPlayed));
    }
    private void AwardRankRewardDifference(LevelRank previousRank, LevelRank newRank)
    {
        if (currentLevelData == null)
            return;

        int newRankSkillPoints = currentLevelData.GetSkillPointReward(newRank);
        int previousRankSkillPoints = currentLevelData.GetSkillPointReward(previousRank);
        int skillPointDifference = newRankSkillPoints - previousRankSkillPoints;

        if (skillPointDifference > 0)
        {
            SkillPointManager.Instance.AddSkillPoints(skillPointDifference);

            if (previousRank == LevelRank.None)
                logger.Log($"Awarded {skillPointDifference} skill points for {newRank.GetDisplayName()} rank!", this);
            else
                logger.Log($"Awarded {skillPointDifference} skill points for improving from {previousRank.GetDisplayName()} to {newRank.GetDisplayName()} rank! (Total from this level: {newRankSkillPoints})", this);
        }

        int newRankCurrency = currentLevelData.GetCurrencyReward(newRank);
        int previousRankCurrency = currentLevelData.GetCurrencyReward(previousRank);
        int currencyDifference = newRankCurrency - previousRankCurrency;

        if (currencyDifference > 0)
        {
            CurrencyManager.Instance.AddCurrency(currencyDifference);

            if (previousRank == LevelRank.None)
                logger.Log($"Awarded {currencyDifference} bonus currency for {newRank.GetDisplayName()} rank!", this);
            else
                logger.Log($"Awarded {currencyDifference} bonus currency for improving rank! (Total from this level: {newRankCurrency})", this);
        }
    }

}