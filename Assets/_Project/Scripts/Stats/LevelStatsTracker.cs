using System;
using inkolorgames;
using UnityEngine;

public class LevelStatsTracker : MonoSingleton<LevelStatsTracker>, ISavable, ILoadable<int>
{
    [SerializeField] private inkolorgames.Logger logger;

    private DayStats currentDayStats;
    public DayStats CurrentDayStats => currentDayStats;

    private LevelStats totalLevelStats;
    public LevelStats TotalLevelStats => totalLevelStats;

    public static event Action<DayStats> OnDayStatsUpdated;
    public static event Action<DayStats, int> OnDayEnded;

    protected override void Awake()
    {
        base.Awake();
        currentDayStats = new DayStats();
        totalLevelStats = new LevelStats();

        GameManager.OnStartGameState += HandleGameStateChange;
        LevelManager.OnStartLevel += OnLevelStart;
        CurrencyManager.OnCurrencyGained += RecordCurrencyGained;

        SimpleDamageable.OnAnyDamageableDie += RecordEnemyDestroyed;
        PaintStateManager.OnAddedTimeToCountdown += RecordTimeAddedToCountdown;
        PaintSpawner.OnAdditionalSpawn += RecordAdditionalPaintBlobSpawned;
        AutoClickerDamageor.OnAutoClickerDamage += RecordAutoClickerDamage;
        BombDamageor.OnBombDamage += RecordBombDamage;
        BrushSwipeDamageor.OnBrushSwipeDamage += RecordBrushSwipeDamage;
    }

    private void OnDestroy()
    {
        SimpleDamageable.OnAnyDamageableDie -= RecordEnemyDestroyed;
        PaintStateManager.OnAddedTimeToCountdown -= RecordTimeAddedToCountdown;
        PaintSpawner.OnAdditionalSpawn -= RecordAdditionalPaintBlobSpawned;
        AutoClickerDamageor.OnAutoClickerDamage -= RecordAutoClickerDamage;
        BombDamageor.OnBombDamage -= RecordBombDamage;
        BrushSwipeDamageor.OnBrushSwipeDamage -= RecordBrushSwipeDamage;

        if (GameManager.HasInstance)
            GameManager.OnStartGameState -= HandleGameStateChange;

        if (LevelManager.HasInstance)
            LevelManager.OnStartLevel -= OnLevelStart;

        if(CurrencyManager.HasInstance)
            CurrencyManager.OnCurrencyGained -= RecordCurrencyGained;
    }

    private void HandleGameStateChange(GameManager.GameState state)
    {
        if (state == GameManager.GameState.PAINT)
            StartNewDay();
        else if (state == GameManager.GameState.DAY_SUMMARY)
            EndCurrentDay();
    }

    private void OnLevelStart(Level level)
    {
        ResetLevelStats();
        totalLevelStats.CurrentDay = LoadCurrentDayElapsed(level);
    }

    private void StartNewDay() => currentDayStats = new DayStats();
    private void EndCurrentDay()
    {
        totalLevelStats.AddDayStats(currentDayStats);
        Save();
        OnDayEnded?.Invoke(currentDayStats, totalLevelStats.CurrentDay);
    }

    private void ResetLevelStats()
    {
        currentDayStats = new DayStats();
        totalLevelStats = new LevelStats();
    }

    private void RecordEnemyDestroyed(Vector3 worldPosition, Color color, float scale)
    {
        currentDayStats.PaintBlobsDestroyed++;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }

    public void RecordTimeAddedToCountdown(float timeAdded)
    {
        currentDayStats.TimeAdded += timeAdded;
        currentDayStats.TimesTimerIncreased++;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }

    public void RecordAutoClickerDamage(float damage)
    {
        currentDayStats.AutoClickerDamage += damage;
        currentDayStats.TotalDamageDealt += damage;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }

    public void RecordBombDamage(float damage)
    {
        currentDayStats.BombDamage += damage;
        currentDayStats.TotalDamageDealt += damage;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }

    public void RecordBrushSwipeDamage(float damage)
    {
        currentDayStats.BrushSwipeDamage += damage;
        currentDayStats.TotalDamageDealt += damage;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }

    public void RecordAdditionalPaintBlobSpawned()
    {
        currentDayStats.AdditionalPaintBlobsSpawned++;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }

    public void RecordCurrencyGained(int amount, int newCurrency)
    {
        currentDayStats.CurrencyGained += amount;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }
    public DayStats GetCurrentDayStats() => currentDayStats;
    public LevelStats GetTotalLevelStats() => totalLevelStats;

    public void Save()
    {
        if (!LevelManager.HasInstance)
            return;

        LevelData levelData = LevelManager.Instance.CurrentLevelData;
        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(levelData.LevelAuthor, levelData.LevelTitle);

        GameSaveManager.Instance.SaveLevelData(
            saveData.isDone,
            saveData.completionRatio,
            levelData.LevelAuthor,
            levelData.LevelTitle,
            saveData.isUnlocked,
            saveData.daysToComplete,
            saveData.bestRank,
            saveData.hasClaimedReward,
            totalLevelStats.CurrentDay 
        );
    }

    public int Load() => throw new NotImplementedException();

    public int LoadCurrentDayElapsed(Level level)
    {
        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(level.LevelData.LevelAuthor, level.LevelData.LevelTitle);

        if (saveData.currentDayElapsed > 0)
        {
            logger.Log($"Loaded level progress: Day {totalLevelStats.CurrentDay}", this);
            return saveData.currentDayElapsed; 
        }

        return 0;
    }
}
