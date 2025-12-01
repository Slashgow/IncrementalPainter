using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "InkolorGames/Level")]
public class Level : ScriptableObject
{
    [SerializeField] private string levelTitle;
    [SerializeField] private string levelAuthor;
    [SerializeField] private string levelDate;
    [SerializeField] private ArtMovement artMovement;
    [SerializeField, ShowAssetPreview(128, 128)] private Sprite levelDrawing;
    [SerializeField] private GameObject levelPrefab;

    public string LevelTitle => levelTitle;
    public string LevelAuthor => levelAuthor;
    public string LevelDate => levelDate;
    public ArtMovement ArtMovement => artMovement;
    public Sprite LevelDrawing => levelDrawing;
}
