using UnityEngine;
using UnityEngine.UI;

public class SaveArtGalleryButton : MonoBehaviour
{
       [SerializeField] private Button saveArtGalleryButton;
    private void OnEnable() => saveArtGalleryButton.onClick.AddListener(SaveArtGallery);
    private void OnDisable() => saveArtGalleryButton.onClick.RemoveListener(SaveArtGallery);
    private void SaveArtGallery()
    {
        LevelManager.Instance.SaveArtGalleryLayout();
    }
}