using System;
using inkolorgames;
using UnityEngine;

public class DayStatsTracker : MonoSingleton<DayStatsTracker>
{
    [SerializeField] private inkolorgames.Logger logger;

    private DayStats currentDayStats;
    public DayStats CurrentDayStats => currentDayStats;

    private LevelStats totalLevelStats;
    public LevelStats TotalLevelStats => totalLevelStats;

    public static event Action<DayStats> OnDayStatsUpdated;
    public static event Action<DayStats> OnDayEnded;

    protected override void Awake()
    {
        base.Awake();
        currentDayStats = new DayStats();
        totalLevelStats = new LevelStats();

        GameManager.OnStartGameState += HandleGameStateChange;
        LevelManager.OnStartLevel += OnLevelStart;

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
    }

    private void HandleGameStateChange(GameManager.GameState state)
    {
        if (state == GameManager.GameState.PAINT)
            StartNewDay();
        else if (state == GameManager.GameState.DAY_SUMMARY)
            EndCurrentDay();
    }

    private void OnLevelStart(Level level) => ResetLevelStats();
    private void StartNewDay() => currentDayStats = new DayStats();
    private void EndCurrentDay()
    {
        totalLevelStats.AddDayStats(currentDayStats);
        OnDayEnded?.Invoke(currentDayStats);
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

    public void RecordCurrencyGained(int amount)
    {
        currentDayStats.CurrencyGained += amount;
        OnDayStatsUpdated?.Invoke(currentDayStats);
    }
    public DayStats GetCurrentDayStats() => currentDayStats;
    public LevelStats GetTotalLevelStats() => totalLevelStats;
}
