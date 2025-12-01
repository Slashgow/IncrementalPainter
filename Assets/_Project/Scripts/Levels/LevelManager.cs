using inkolorgames;
using UnityEngine;

public class LevelManager : PersistentMonoSingleton<LevelManager>
{
    [SerializeField] private Level defaultLevel;
    [SerializeField] private Level[] sortedLevels;

    [SerializeField] private SpriteRenderer frameRenderer;

    public SpriteRenderer FrameRenderer => frameRenderer;
    public Bounds FrameBounds => frameRenderer.bounds;

    public Level CurrentLevel { get; set; }
    public Level[] SortedLevels => sortedLevels;

    protected override void Awake()
    {
        base.Awake();
        CurrentLevel = defaultLevel;
    }

}
