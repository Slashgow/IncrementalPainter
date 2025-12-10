using UnityEngine;

public class LevelCompletionTracker : MonoBehaviour
{
    [SerializeField] private inkolorgames.Logger logger;

    private int daysElapsed;
    private LevelData currentLevelData;
    private bool isTracking;

    public int GetDaysElapsed() => daysElapsed;
    public bool IsTracking => isTracking;

    private void Awake()
    {
        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }

    private void OnDestroy()
    {
        if (GameManager.HasInstance)
            GameManager.OnStartGameState -= GameManager_OnStartGameState;
    }

    private void GameManager_OnStartGameState(GameManager.GameState state)
    {
        if (state == GameManager.GameState.DAY_SUMMARY && isTracking)
        {
            daysElapsed++;
            logger.Log($"Day {daysElapsed} completed for level '{currentLevelData.LevelTitle}'", this);
        }
    }

    public void StartTracking(LevelData levelData)
    {
        if (levelData == null)
        {
            logger.LogError("Cannot start tracking: LevelData is null", this);
            return;
        }

        currentLevelData = levelData;
        daysElapsed = 0;
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
            daysElapsed = 0;
            return;
        }

        int finalDays = Mathf.Max(1, daysElapsed);
        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(currentLevelData.LevelAuthor, currentLevelData.LevelTitle);
        LevelRank newRank = currentLevelData.GetRankForDays(finalDays);
        bool shouldUpdateRank = saveData.bestRank < newRank;

        if (shouldUpdateRank)
        {
            saveData.daysToComplete = finalDays;
            saveData.bestRank = newRank;
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
        daysElapsed = 0;
    }

    public LevelRank GetCurrentProjectedRank()
    {
        if (!isTracking || currentLevelData == null)
            return LevelRank.None;

        return currentLevelData.GetRankForDays(Mathf.Max(1, daysElapsed));
    }

}