using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "InkolorGames/Level")]
public class LevelData : ScriptableObject
{
    [SerializeField] private string levelTitle;
    [SerializeField] private string levelAuthor;
    [SerializeField] private string levelDate;
    [SerializeField] private ArtMovement artMovement;
    [SerializeField, ShowAssetPreview(128, 128)] private Sprite levelDrawing;
    [SerializeField] private GameObject levelPrefab;

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
    public Sprite LevelDrawing => levelDrawing;
    public GameObject LevelPrefab => levelPrefab;
}
