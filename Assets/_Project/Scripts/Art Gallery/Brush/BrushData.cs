using UnityEngine;


[CreateAssetMenu(fileName = "BrushData", menuName = "InkolorGames/Brush Data")]
public class BrushData : ScriptableObject, IUnlockableItem
{
    [Header("Identification")]
    [SerializeField] private string brushName;
    [SerializeField] private string brushId;

    [Header("Visual")]
    [SerializeField] private Texture brushTexture;
    [SerializeField] private Sprite brushSprite;

    [Header("Description")]
    [TextArea(2, 4)]
    [SerializeField] private string description;

    // IUnlockableItem implementation
    public string ItemId => !string.IsNullOrEmpty(brushId) ? brushId : brushName.Replace(" ", "_").ToLower();
    public string ItemName => brushName;
    public string ItemType => "Brush";


    public string BrushName => brushName;
    public string BrushId => ItemId;
    public Sprite BrushSprite => brushSprite;
    public Texture BrushTexture => brushTexture;
    public string Description => description;

    // Validation
    private void OnValidate()
    {
        // Auto-generate brush ID if empty
        if (string.IsNullOrEmpty(brushId) && !string.IsNullOrEmpty(brushName))
        {
            brushId = brushName.Replace(" ", "_").ToLower();
        }
    }
}
