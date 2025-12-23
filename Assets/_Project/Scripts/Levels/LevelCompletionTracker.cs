using System;
using UnityEngine;

public class LevelCompletionTracker : MonoBehaviour
{
    [SerializeField] private inkolorgames.Logger logger;

    private LevelData currentLevelData;
    private bool isTracking;
    public bool IsTracking => isTracking;

    public static event Action<int, int> OnGiveReward;

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

        AwardRankRewardDifference(saveData.claimedRewardRank, newRank);

        bool isNewBest = newRank > saveData.bestRank;
        if (isNewBest)
        {
            logger.Log($"NEW BEST! Level '{currentLevelData.LevelTitle}' completed in {finalDays} days. Rank: {newRank.GetDisplayName()}", this);
            saveData.bestRank = newRank;
        }
        else
        {
            logger.Log($"Level '{currentLevelData.LevelTitle}' completed in {finalDays} days. Rank: {newRank.GetDisplayName()} (Best: {saveData.bestRank.GetDisplayName()})", this);
        }

        saveData.isDone = true;
        saveData.daysToComplete = finalDays;
        saveData.claimedRewardRank = newRank;

        GameSaveManager.Instance.SaveLevelData(
            saveData.isDone,
            saveData.completionRatio,
            saveData.author,
            saveData.title,
            saveData.isUnlocked,
            saveData.daysToComplete,
            saveData.bestRank,
            false,
            saveData.levelStats
        );

        isTracking = false;
    }

    public LevelRank GetCurrentProjectedRank()
    {
        if (!isTracking || currentLevelData == null)
            return LevelRank.None;

        return currentLevelData.GetRankForDays(Mathf.Max(1, LevelStatsTracker.Instance.TotalLevelStats.TotalDaysPlayed));
    }
    private void AwardRankRewardDifference(LevelRank previousClaimedRank, LevelRank newRank)
    {
        if (currentLevelData == null)
            return;

        if (newRank <= previousClaimedRank)
        {
            logger.Log($"No new rewards to claim. Already claimed rewards for {previousClaimedRank.GetDisplayName()} rank or better.", this);
            return;
        }

        int newRankSkillPoints = currentLevelData.GetSkillPointReward(newRank);
        int previousRankSkillPoints = currentLevelData.GetSkillPointReward(previousClaimedRank);
        int skillPointDifference = Mathf.Max(0, newRankSkillPoints - previousRankSkillPoints);

        int newRankCurrency = currentLevelData.GetCurrencyReward(newRank);
        int previousRankCurrency = currentLevelData.GetCurrencyReward(previousClaimedRank);
        int currencyDifference = Mathf.Max(0, newRankCurrency - previousRankCurrency);

        if (skillPointDifference > 0)
        {
            SkillPointManager.Instance.AddSkillPoints(skillPointDifference);

            if (previousClaimedRank == LevelRank.None)
                logger.Log($"Awarded {skillPointDifference} skill points for {newRank.GetDisplayName()} rank!", this);
            else
                logger.Log($"Awarded {skillPointDifference} skill points for improving from {previousClaimedRank.GetDisplayName()} to {newRank.GetDisplayName()} rank! (Total earned from this level: {newRankSkillPoints})", this);
        }

        if (currencyDifference > 0)
        {
            CurrencyManager.Instance.AddCurrency(currencyDifference);

            if (previousClaimedRank == LevelRank.None)
                logger.Log($"Awarded {currencyDifference} bonus currency for {newRank.GetDisplayName()} rank!", this);
            else
                logger.Log($"Awarded {currencyDifference} bonus currency for improving rank! (Total earned from this level: {newRankCurrency})", this);
        }

        OnGiveReward?.Invoke(skillPointDifference, currencyDifference);
    }

}