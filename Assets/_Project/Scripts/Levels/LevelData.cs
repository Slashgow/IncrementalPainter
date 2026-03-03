using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Level", menuName = "InkolorGames/Level")]
public class LevelData : ScriptableObject
{
    [SerializeField] private string levelTitle;
    [SerializeField] private string levelAuthor;
    [SerializeField] private string levelDate;
    [SerializeField] private ArtMovement artMovement;
    [SerializeField, Range(0f,1f)] private float percentCompletionCondition;
    [SerializeField, ShowAssetPreview(128, 128)] private Sprite levelDrawing;
    [SerializeField] private GameObject levelPrefab;
    [SerializeField, Range(0f, 25f)] private float cameraPaintZoom;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private bool isBossLevel = false;
    [SerializeField, ShowIf("isBossLevel")] private GameObject bossLevelPrefab;
    [SerializeField, ShowIf("isBossLevel")] private LocalizedString bossName;
    [SerializeField, ShowIf("isBossLevel")] private LocalizedString bossDescription;
    [SerializeField, ShowIf("isBossLevel")] private Sprite bossUISprite;
    [SerializeField, ShowIf("isBossLevel")] private AudioClip bossAnouncementAudioClip;

    [Header("Ranking System")]
    [SerializeField] private LevelRankThresholds rankThresholds = new LevelRankThresholds();
    [SerializeField] private RankRewardData rankRewards = new RankRewardData();
    public LevelRank GetRankForDays(int days) => rankThresholds.GetRankForDays(days);
    public int GetDaysRequiredForRank(LevelRank rank) => rankThresholds.GetDaysRequiredForRank(rank);
    public int GetSkillPointReward(LevelRank rank) => rankRewards.GetSkillPointReward(rank);
    public int GetCurrencyReward(LevelRank rank) => rankRewards.GetCurrencyReward(rank);

    public string LevelTitle => levelTitle;
    public string LevelAuthor => levelAuthor;
    public string LevelDate => levelDate;
    public ArtMovement ArtMovement => artMovement;
    public float PercentCompletionCondition => percentCompletionCondition;
    public Sprite LevelDrawing => levelDrawing;
    public GameObject LevelPrefab => levelPrefab;
    public float CameraPaintZoom => cameraPaintZoom;
    public Vector3 SpawnOffset => spawnOffset;
    public bool IsBossLevel => isBossLevel;
    public GameObject BossLevelPrefab => bossLevelPrefab;
    public LocalizedString BossName => bossName;
    public LocalizedString BossDescription => bossDescription;
    public Sprite BossUISprite => bossUISprite;
    public AudioClip BossAnouncementAudioClip => bossAnouncementAudioClip;
}
