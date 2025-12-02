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

    protected override void Awake()
    {
        base.Awake();
        SetCurrentLevel(defaultLevel);
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
    }
}
