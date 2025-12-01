using inkolorgames;
using UnityEngine;
public class LevelManager : PersistentMonoSingleton<LevelManager>
{
    [SerializeField] private LevelData defaultLevel;
    [SerializeField] private LevelData[] sortedLevelsData;
    public Bounds FrameBounds => currentLevel.FrameCollider.bounds;

    public LevelData CurrentLevelData { get; set; }
    public LevelData[] SortedLevelsData => sortedLevelsData;

    public Level CurrentLevel => currentLevel;
    private Level currentLevel;

    protected override void Awake()
    {
        base.Awake();
        CurrentLevelData = defaultLevel;
    }

    public void LoadCurrentLevel()
    {
        currentLevel = null;
        this.transform.DestroyAllChildren();

        GameObject levelGOInstance = Instantiate(CurrentLevelData.LevelPrefab, Vector3.zero, Quaternion.identity, this.transform);
        currentLevel = levelGOInstance.GetComponent<Level>();
        currentLevel.Initialize(CurrentLevelData);
    }

}
