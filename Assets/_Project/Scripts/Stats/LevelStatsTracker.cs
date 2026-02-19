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
        ResetLevelStats();
        LoadLevelProgress(LevelManager.Instance.CurrentLevelData);

        GameManager.OnStartGameState += HandleGameStateChange;
        CurrencyManager.OnCurrencyGained += RecordCurrencyGained;
        SimpleDamageable.OnAnyDamageableDie += RecordEnemyDestroyed;
        PaintStateManager.OnAddedTimeToCountdown += RecordTimeAddedToCountdown;
        PaintSpawner.OnAdditionalSpawn += RecordAdditionalPaintBlobSpawned;
        AutoClickerDamageor.OnAutoClickerDamage += RecordAutoClickerDamage;
        BombDamageor.OnBombDamage += RecordBombDamage;
        BrushSwipe.OnAnyBrushSwipeAttack += RecordBrushSwipeDamage;
        PoisonZone.OnAnyPoisonAttack += RecordPoisonDamage;
        NukeDamageor.OnNukeDamage += RecordNukeDamage;
        TurretDamageor.OnTurretDamage += RecordTurretDamage;
        ItemSpawner.OnSpawnMagnetItem += RecordMagnetItemSpawn;
        ItemSpawner.OnSpawnNukeItem += RecordNukeItemSpawn;
    }

    private void OnDestroy()
    {
        SimpleDamageable.OnAnyDamageableDie -= RecordEnemyDestroyed;
        PaintStateManager.OnAddedTimeToCountdown -= RecordTimeAddedToCountdown;
        PaintSpawner.OnAdditionalSpawn -= RecordAdditionalPaintBlobSpawned;
        AutoClickerDamageor.OnAutoClickerDamage -= RecordAutoClickerDamage;
        BombDamageor.OnBombDamage -= RecordBombDamage;
        BrushSwipe.OnAnyBrushSwipeAttack -= RecordBrushSwipeDamage;
        PoisonZone.OnAnyPoisonAttack -= RecordPoisonDamage;
        GameManager.OnStartGameState -= HandleGameStateChange;
        CurrencyManager.OnCurrencyGained -= RecordCurrencyGained;
        NukeDamageor.OnNukeDamage -= RecordNukeDamage;
        TurretDamageor.OnTurretDamage -= RecordTurretDamage;
        ItemSpawner.OnSpawnMagnetItem -= RecordMagnetItemSpawn;
        ItemSpawner.OnSpawnNukeItem -= RecordNukeItemSpawn;
    }

    private void HandleGameStateChange(GameManager.GameState state)
    {
        if (state == GameManager.GameState.PAINT)
            StartNewDay();
        else if (state == GameManager.GameState.DAY_SUMMARY)
            EndCurrentDay();
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

    private void RecordEnemyDestroyed(Vector3 worldPosition, Color color, float scale, bool paint)
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

    public void RecordBrushSwipeDamage(float damage, Vector3 position, bool isCritical)
    {
        currentDayStats.BrushSwipeDamage += damage;
        currentDayStats.TotalDamageDealt += damage;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }
    private void RecordPoisonDamage(float damage, Vector3 position, bool isCritical)
    {
        currentDayStats.PoisonDamage += damage;
        currentDayStats.TotalDamageDealt += damage;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }

    private void RecordNukeDamage(float damage)
    {
        currentDayStats.PaintBlobsDestroyedByNuke++;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }

    private void RecordTurretDamage(float damage)
    {
        currentDayStats.TurretDamage += damage;
        currentDayStats.TotalDamageDealt += damage;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }

    private void RecordNukeItemSpawn()
    {
        currentDayStats.NukeItemSpawned++;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }

    private void RecordMagnetItemSpawn()
    {
        currentDayStats.MagnetItemSpawned++;
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
            false,
            totalLevelStats
        );
    }

    public int Load() => throw new NotImplementedException();

    public void LoadLevelProgress(LevelData levelData)
    {
        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(levelData.LevelAuthor, levelData.LevelTitle);

        if (saveData.isDone)
        {
            GameSaveManager.Instance.ClearLevelProgress(levelData.LevelAuthor, levelData.LevelTitle);
            totalLevelStats.CurrentDay = 1;
            logger.Log($"Level was completed, starting fresh from Day 1", this);
        }
        else if (saveData.levelStats != null && saveData.levelStats.CurrentDay > 0)
        {
            totalLevelStats = saveData.levelStats;
            logger.Log($"Loaded level progress: Day {totalLevelStats.CurrentDay}", this);
        }
    }
}
