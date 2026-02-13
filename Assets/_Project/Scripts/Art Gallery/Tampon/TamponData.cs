using UnityEngine;

[CreateAssetMenu(fileName = "TamponData", menuName = "InkolorGames/Tampon Data")]
public class TamponData : ScriptableObject, IUnlockableItem
{
    [Header("Identification")]
    [SerializeField] private string tamponName;
    [SerializeField] private string tamponId;

    [Header("Visual")]
    [SerializeField] private Texture tamponTexture;
    [SerializeField] private Sprite tamponSprite;

    public string ItemId => !string.IsNullOrEmpty(tamponId) ? tamponId : tamponName.Replace(" ", "_").ToLower();
    public string ItemName => tamponName;
    public string ItemType => "Tampon";

    public Texture TamponTexture => tamponTexture;
    public Sprite TamponSprite => tamponSprite;

    private void OnValidate()
    {
        // Auto-generate brush ID if empty
        if (string.IsNullOrEmpty(tamponId) && !string.IsNullOrEmpty(tamponName))
        {
            tamponId = tamponName.Replace(" ", "_").ToLower();
        }
    }

}
