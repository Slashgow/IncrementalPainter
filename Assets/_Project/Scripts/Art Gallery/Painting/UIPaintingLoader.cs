using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPaintingLoader : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private UILevelArtGallery levelUIPrefab;

    private List<UILevelArtGallery> uiLevels = new List<UILevelArtGallery>();

    private void OnEnable()
    {
        DestroyPaintingCommand.OnDestroyPainting += OnDestroyPainting;
        DestroyPaintingCommand.OnUndoDestroyPainting += DestroyUILevel;
    }

    private void OnDisable()
    {
        DestroyPaintingCommand.OnDestroyPainting -= OnDestroyPainting;
        DestroyPaintingCommand.OnUndoDestroyPainting -= DestroyUILevel;
    }

    public void Start()
    {
        InitializeLevelsUI();
    }

    private void InitializeLevelsUI()
    {
        parent.DestroyAllChildren();

        Dictionary<string, LevelSaveData> levelsSaveData = GameSaveManager.Instance.GetAllLevelSaves();
        Dictionary<string, ArtGalleryPaintingSaveData> levelsAlreadySpawned = GameSaveManager.Instance.LoadArtGalleryLayout();

        foreach (LevelSaveData levelSaveData in levelsSaveData.Values)
        {
            if (!levelSaveData.isUnlocked)
                continue;

            if(levelsAlreadySpawned.ContainsKey(levelSaveData.ID))
                continue;

            SpawnUILevel(levelSaveData.author, levelSaveData.title);
        }
    }

    private void SpawnUILevel(string author, string title)
    {
        LevelData level = LevelManager.Instance.GetLevelDataByAuthorAndTitle(author, title);
        UILevelArtGallery uiLevelInstance = Instantiate(levelUIPrefab, parent);
        uiLevelInstance.Initialize(level);
        uiLevels.Add(uiLevelInstance);

        uiLevelInstance.OnSelectEvent -= OnSelectPainting;
        uiLevelInstance.OnSelectEvent += OnSelectPainting;
    }


    private void OnSelectPainting(LevelData levelData)
    {
        LevelManager.Instance.SetCurrentLevel(levelData);
        LevelManager.Instance.InstantiateLevelArtGallery();

        DestroyUILevel(levelData);
    }

    private void DestroyUILevel(LevelData levelData)
    {
        UILevelArtGallery selectedLevel = GetUILevelArtByLevelData(levelData);
        if (selectedLevel != null)
        {
            uiLevels.Remove(selectedLevel);
            GameObject.Destroy(selectedLevel.gameObject);
        }
    }

    private void OnDestroyPainting(LevelData levelData) => SpawnUILevel(levelData.LevelAuthor, levelData.LevelTitle);
    private UILevelArtGallery GetUILevelArtByLevelData(LevelData levelData) => uiLevels.FirstOrDefault(uiLevel => uiLevel.LevelData == levelData);
}
