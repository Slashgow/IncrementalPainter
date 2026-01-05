using System;
using System.Linq;
using inkolorgames;
using UnityEngine;
public class LevelManager : PersistentMonoSingleton<LevelManager>
{
    [SerializeField] private LevelCompletionTracker completionTracker;
    [SerializeField] private Color sRankColor, aRankColor, bRankColor, cRankColor, dRankColor;

    [SerializeField] private LevelData defaultLevel;

    [SerializeField] private UnlockableLevel[] unlockableSortedLevels;
    public Bounds FrameBounds => currentLevelInstance.FrameCollider.bounds;
    public UnlockableLevel[] UnlockableSortedLevels => unlockableSortedLevels;

    private Level currentLevelInstance;
    public Level CurrentLevelInstance => currentLevelInstance;

    private SimpleDamageable currentBossDamageableInstance;
    public SimpleDamageable CurrentBossDamageableInstance => currentBossDamageableInstance;
    public static int CurrentLevelIndex { get; private set; } = -1;

    private UnlockableLevel currentUnlockableLevel;
    public UnlockableLevel CurrentUnlockableLevel => currentUnlockableLevel;
    public LevelData CurrentLevelData => currentUnlockableLevel.LevelData;

    public static event Action OnEndLevel;
    public static event Action<Level> OnStartLevel;

    public bool IsNextLevelUnlocked => CurrentLevelIndex < unlockableSortedLevels.Length - 1 && 
        unlockableSortedLevels[CurrentLevelIndex + 1].IsUnlocked;

    protected override void Awake()
    {
        base.Awake();
       
        SetCurrentLevel(defaultLevel);
    }

    private void Start()
    {
        TryUnlockLevels();
    }

    public void SetCurrentLevel(LevelData levelData)
    {
        currentUnlockableLevel = unlockableSortedLevels.FirstOrDefault(level => level.LevelData == levelData);
        CurrentLevelIndex = currentUnlockableLevel != null ? Array.IndexOf(unlockableSortedLevels, currentUnlockableLevel) : -1;
    }

    public void SetCurrentLevel(int levelIndex)
    {
        currentUnlockableLevel = unlockableSortedLevels[levelIndex];
        CurrentLevelIndex = levelIndex;
    }

    public void SetNextLevelAsCurrentLevel()
    {
        if (CurrentLevelIndex >= unlockableSortedLevels.Length - 1)
            return;

        SetCurrentLevel(CurrentLevelIndex + 1);
    }

    public void LoadCurrentLevel()
    {
        currentLevelInstance = null;
        this.transform.DestroyAllChildrenWithComponent<Level>();

        GameObject levelGOInstance = Instantiate(CurrentLevelData.LevelPrefab, CurrentLevelData.SpawnOffset, Quaternion.identity, this.transform);
        currentLevelInstance = levelGOInstance.GetComponent<Level>();
        currentLevelInstance.Initialize(CurrentLevelData);

        if(CurrentLevelData.IsBossLevel && CurrentLevelData.BossLevelPrefab != null)
        {
            GameObject bossInstance = Instantiate(CurrentLevelData.BossLevelPrefab, currentLevelInstance.BossSpawnTransform.position,
               currentLevelInstance.BossSpawnTransform.rotation);

            if (bossInstance.TryGetComponent<SimpleDamageable>(out var damageable))
                currentBossDamageableInstance = damageable;
        }

        completionTracker.StartTracking(CurrentLevelData);

        OnStartLevel?.Invoke(currentLevelInstance);

        currentLevelInstance.OnEndLevel -= CurrentLevelInstance_OnEndLevel;
        currentLevelInstance.OnEndLevel += CurrentLevelInstance_OnEndLevel;
    }

    private void CurrentLevelInstance_OnEndLevel()
    {
        completionTracker.StopTrackingAndSaveRank(true);
        OnEndLevel?.Invoke();
        TryUnlockLevels();
    }

    private void TryUnlockLevels()
    {
        foreach (var unlockableLevel in unlockableSortedLevels)
        {
            unlockableLevel.CheckUnlockCondition();
        }
    }
    public LevelRank GetCurrentLevelBestRank()
    {
        if (CurrentLevelData == null)
            return LevelRank.None;

        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(CurrentLevelData.LevelAuthor, CurrentLevelData.LevelTitle);
        return saveData?.bestRank ?? LevelRank.None;
    }
    public LevelRank GetCurrentProjectedRank() => completionTracker.GetCurrentProjectedRank();
    public Color GetCurrentProjectedRankColor()
    {
        LevelRank projectedRank = GetCurrentProjectedRank();
        return projectedRank switch
        {
            LevelRank.S => sRankColor,
            LevelRank.A => aRankColor,
            LevelRank.B => bRankColor,
            LevelRank.C => cRankColor,
            LevelRank.D => dRankColor,
            _ => Color.white,
        };
    }

    public int GetRemainingCurrencyReward(UnlockableLevel unlockableLevel)
    {
        if (unlockableLevel == null || unlockableLevel.LevelData == null)
            return 0;

        LevelData levelData = unlockableLevel.LevelData;
        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(levelData.LevelAuthor, levelData.LevelTitle);

        int maxCurrencyReward = levelData.GetCurrencyReward(LevelRank.S);
        int claimedCurrency = levelData.GetCurrencyReward(saveData.claimedRewardRank);
        return Mathf.Max(0, maxCurrencyReward - claimedCurrency);
    }

    public int GetRemainingSkillPointReward(UnlockableLevel unlockableLevel)
    {
        if (unlockableLevel == null || unlockableLevel.LevelData == null)
            return 0;

        LevelData levelData = unlockableLevel.LevelData;
        LevelSaveData saveData = GameSaveManager.Instance.LoadLevelData(levelData.LevelAuthor, levelData.LevelTitle);

        int maxSkillPointReward = levelData.GetSkillPointReward(LevelRank.S);
        int claimedSkillPoints = levelData.GetSkillPointReward(saveData.claimedRewardRank);
        return Mathf.Max(0, maxSkillPointReward - claimedSkillPoints);
    }
}