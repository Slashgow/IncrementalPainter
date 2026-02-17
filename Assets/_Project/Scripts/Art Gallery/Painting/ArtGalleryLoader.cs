using System.Collections.Generic;
using UnityEngine;

public class ArtGalleryLoader : MonoBehaviour
{
    private void Start()
    {
        LoadArtGallery();
    }

    private void LoadArtGallery()
    {
        LevelManager.Instance.ClearArtGalleryInstances();

        Dictionary<string, ArtGalleryPaintingSaveData> layout = GameSaveManager.Instance.LoadArtGalleryLayout();

        foreach (ArtGalleryPaintingSaveData artGalleryPaintingSaveData in layout.Values)
        {
            LevelManager.Instance.SetCurrentLevel(LevelManager.Instance.GetLevelDataByAuthorAndTitle(
                artGalleryPaintingSaveData.author, 
                artGalleryPaintingSaveData.title));
            var currentLevelInstance = LevelManager.Instance.InstantiateLevelArtGallery();

            currentLevelInstance.LoadArtGalleryPaintingSaveData(artGalleryPaintingSaveData);     
        }
    }
}
