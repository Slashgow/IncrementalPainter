using UnityEngine;
using UnityEngine.UI;

public class SaveArtGalleryButton : MonoBehaviour
{
       [SerializeField] private Button saveArtGalleryButton;
    private void OnEnable()
    {
        saveArtGalleryButton.onClick.AddListener(SaveArtGallery);
        ArtGaleryInput.OnSavePerformed += SaveArtGallery;
    }

    private void OnDisable()
    {
        saveArtGalleryButton.onClick.RemoveListener(SaveArtGallery);
        ArtGaleryInput.OnSavePerformed -= SaveArtGallery;
    }

    private void SaveArtGallery()
    {
        LevelManager.Instance.SaveArtGalleryLayout();
    }
}