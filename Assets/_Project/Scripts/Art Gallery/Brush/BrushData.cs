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

    [Header("Properties")]
    [SerializeField] private float defaultSize = 10f;
    [SerializeField] private float minSize = 1f;
    [SerializeField] private float maxSize = 100f;
    [SerializeField] private float defaultOpacity = 1f;

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
    public float DefaultSize => defaultSize;
    public float MinSize => minSize;
    public float MaxSize => maxSize;
    public float DefaultOpacity => defaultOpacity;
    public string Description => description;

    // Validation
    private void OnValidate()
    {
        // Auto-generate brush ID if empty
        if (string.IsNullOrEmpty(brushId) && !string.IsNullOrEmpty(brushName))
        {
            brushId = brushName.Replace(" ", "_").ToLower();
        }

        // Ensure size constraints are valid
        if (minSize > maxSize)
            minSize = maxSize;

        if (defaultSize < minSize)
            defaultSize = minSize;
        else if (defaultSize > maxSize)
            defaultSize = maxSize;

        // Clamp opacity
        defaultOpacity = Mathf.Clamp01(defaultOpacity);
    }
}
