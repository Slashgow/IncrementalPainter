using System;
using System.Collections.Generic;
using System.Linq;
using inkolorgames;
using SaveUtility;
using UnityEngine;

public class LevelManager : PersistentMonoSingleton<LevelManager>
{
    [SerializeField] private LevelCompletionTracker completionTracker;
    [SerializeField] private RankColorsId rankColorsId;

    [SerializeField] private LevelData defaultLevel;

    [SerializeField] private UnlockableLevel[] unlockableSortedLevels;
    public Bounds FrameBounds => currentLevelInstance.FrameCollider.bounds;
    public UnlockableLevel[] UnlockableSortedLevels => unlockableSortedLevels;

    private Level currentLevelInstance;
    public Level CurrentLevelInstance => currentLevelInstance;

    private List<Level> artGalleryInstances = new List<Level>();
    public IReadOnlyList<Level> ArtGalleryInstances => artGalleryInstances;

    private SimpleDamageable currentBossDamageableInstance;
    public SimpleDamageable CurrentBossDamageableInstance => currentBossDamageableInstance;
    public static int CurrentLevelIndex { get; private set; } = -1;

    private UnlockableLevel currentUnlockableLevel;
    public UnlockableLevel CurrentUnlockableLevel => currentUnlockableLevel;
    public LevelData CurrentLevelData => currentUnlockableLevel.LevelData;

    public static event Action OnMidLevel;
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

    public void CleanLevelGameScene()
    {
        if (currentLevelInstance != null && !currentLevelInstance.GetComponent<Draggable>().enabled) // destroy level from game
            Destroy(CurrentLevelInstance.gameObject);
    }

    public Level InstantiateLevelArtGallery()
    {
        string levelID = SavePath.GetLevelID(CurrentLevelData.LevelAuthor, CurrentLevelData.LevelTitle);
        if (artGalleryInstances.Exists(l => l != null && SavePath.GetLevelID(l.LevelData.LevelAuthor, l.LevelData.LevelTitle) == levelID))
            return null;

        GameObject levelGOInstance = Instantiate(CurrentLevelData.LevelPrefab, CurrentLevelData.SpawnOffset, Quaternion.identity);
        currentLevelInstance = levelGOInstance.GetComponent<Level>();
        currentLevelInstance.Initialize(CurrentLevelData, true);

        artGalleryInstances.Add(currentLevelInstance);

        return currentLevelInstance;
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
        currentLevelInstance.Initialize(CurrentLevelData, false);

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

        currentLevelInstance.OnMidLevel -= CurrentLevelInstance_OnMidLevel;
        currentLevelInstance.OnMidLevel += CurrentLevelInstance_OnMidLevel;
    }

    private void CurrentLevelInstance_OnMidLevel() => OnMidLevel?.Invoke();

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
        return rankColorsId.GetColorForRank(projectedRank);
    }
    public Color GetColorForRank(LevelRank rank) => rankColorsId.GetColorForRank(rank);

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

        return completionTracker.GetRemainingSkillReward(saveData.claimedRewardRank, levelData);

        //int maxSkillPointReward = levelData.GetSkillPointReward(LevelRank.S);
        //int claimedSkillPoints = levelData.GetSkillPointReward(saveData.claimedRewardRank);
        //return Mathf.Max(0, maxSkillPointReward - claimedSkillPoints);
    }

    public LevelData GetLevelDataByAuthorAndTitle(string author, string title)
    {
        UnlockableLevel unlockableLevel = unlockableSortedLevels.FirstOrDefault(level => 
        level.LevelData.LevelAuthor == author && 
        level.LevelData.LevelTitle == title);
        return unlockableLevel != null ? unlockableLevel.LevelData : null;
    }


    public void SaveArtGalleryLayout()
    {
        Dictionary<string, ArtGalleryPaintingSaveData> layout = new Dictionary<string, ArtGalleryPaintingSaveData>();

        foreach (Level level in artGalleryInstances)
        {
            if (level == null) 
                continue;

            string levelID = SavePath.GetLevelID(level.LevelData.LevelAuthor, level.LevelData.LevelTitle);
            layout[levelID] = level.GetArtGalleryTransformData();
        }

        GameSaveManager.Instance.SaveArtGalleryLayout(layout);
    }

    public void ClearArtGalleryInstances() => artGalleryInstances.Clear();
}