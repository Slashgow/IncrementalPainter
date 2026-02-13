using UnityEngine;
using UnityEngine.UI;

public class UIColorPreviewer : MonoBehaviour
{
    [SerializeField] private Image image;

    private void OnEnable()
    {
        ArtGalleryColorManager.OnColorChanged += ArtGalleryColorManager_OnColorChanged;
    }

    private void OnDisable()
    {
        ArtGalleryColorManager.OnColorChanged -= ArtGalleryColorManager_OnColorChanged;
    }

    private void ArtGalleryColorManager_OnColorChanged(Color color) => image.color = color;
}
