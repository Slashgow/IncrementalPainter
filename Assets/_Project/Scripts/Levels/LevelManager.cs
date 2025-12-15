using System;
using System.Linq;
using inkolorgames;
using UnityEngine;
public class LevelManager : PersistentMonoSingleton<LevelManager>
{
    [SerializeField] private LevelCompletionTracker completionTracker;

    [SerializeField] private LevelData defaultLevel;

    [SerializeField] private UnlockableLevel[] unlockableSortedLevels;
    public Bounds FrameBounds => currentLevelInstance.FrameCollider.bounds;
    public UnlockableLevel[] UnlockableSortedLevels => unlockableSortedLevels;

    private Level currentLevelInstance;
    public Level CurrentLevelInstance => currentLevelInstance;

    public static int CurrentLevelIndex { get; private set; } = -1;

    private UnlockableLevel currentUnlockableLevel;
    public UnlockableLevel CurrentUnlockableLevel => currentUnlockableLevel;
    public LevelData CurrentLevelData => currentUnlockableLevel.LevelData;

    public static event Action OnEndLevel;
    public static event Action<Level> OnStartLevel;

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
        this.transform.DestroyAllChildren();

        GameObject levelGOInstance = Instantiate(CurrentLevelData.LevelPrefab, CurrentLevelData.SpawnOffset, Quaternion.identity, this.transform);
        currentLevelInstance = levelGOInstance.GetComponent<Level>();
        currentLevelInstance.Initialize(CurrentLevelData);

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
    public int GetCurrentLevelDaysElapsed() => completionTracker.GetDaysElapsed();
    public LevelRank GetCurrentProjectedRank() => completionTracker.GetCurrentProjectedRank();
}