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
    public LevelRankThresholds rankThresholds = new LevelRankThresholds();
    public LevelRank GetRankForDays(int days) => rankThresholds.GetRankForDays(days);

    public string LevelTitle => levelTitle;
    public string LevelAuthor => levelAuthor;
    public string LevelDate => levelDate;
    public ArtMovement ArtMovement => artMovement;
    public Sprite LevelDrawing => levelDrawing;
    public GameObject LevelPrefab => levelPrefab;
}
