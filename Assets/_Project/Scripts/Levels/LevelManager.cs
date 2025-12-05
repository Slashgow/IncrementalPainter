using System;
using System.Linq;
using inkolorgames;
using UnityEngine;
public class LevelManager : PersistentMonoSingleton<LevelManager>
{
    [SerializeField] private LevelData defaultLevel;

    [SerializeField] private UnlockableLevel[] unlockableSortedLevels;
    public Bounds FrameBounds => currentLevelInstance.FrameCollider.bounds;
    public UnlockableLevel[] UnlockableSortedLevels => unlockableSortedLevels;

    private Level currentLevelInstance;
    public Level CurrentLevelInstance => currentLevelInstance;

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
    }

    public void LoadCurrentLevel()
    {
        currentLevelInstance = null;
        this.transform.DestroyAllChildren();

        GameObject levelGOInstance = Instantiate(CurrentLevelData.LevelPrefab, Vector3.zero, Quaternion.identity, this.transform);
        currentLevelInstance = levelGOInstance.GetComponent<Level>();
        currentLevelInstance.Initialize(CurrentLevelData);
        OnStartLevel?.Invoke(currentLevelInstance);

        currentLevelInstance.OnEndLevel -= CurrentLevelInstance_OnEndLevel;
        currentLevelInstance.OnEndLevel += CurrentLevelInstance_OnEndLevel;
    }

    private void CurrentLevelInstance_OnEndLevel()
    {
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
}
