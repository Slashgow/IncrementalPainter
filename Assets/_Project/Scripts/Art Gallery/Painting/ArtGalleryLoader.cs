using System.Collections;
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

        StartCoroutine(DisableDragAndGizmo());
    }

    private IEnumerator DisableDragAndGizmo()
    {
        yield return new WaitForSeconds(0.5f);

        Draggable.DisableDrag();
        var transformControllers = GameObject.FindObjectsByType<TransformController>(FindObjectsSortMode.None);
        foreach (var transformController in transformControllers)
        {
            transformController.SetGizmosVisible(false);
        }
    }
}
