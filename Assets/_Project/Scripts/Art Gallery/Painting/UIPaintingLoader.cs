using System.Collections.Generic;
using UnityEngine;

public class UIPaintingLoader : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private UILevelArtGallery levelUIPrefab;

    private List<UILevelArtGallery> uiLevels = new List<UILevelArtGallery>();

    public void Start()
    {
        InitializeLevelsUI();
    }

    private void InitializeLevelsUI()
    {
        parent.DestroyAllChildren();

        Dictionary<string, LevelSaveData> levelsSaveData = GameSaveManager.Instance.GetAllLevelSaves();

        foreach (LevelSaveData levelSaveData in levelsSaveData.Values)
        {
            if(!levelSaveData.isUnlocked)
                continue;

            LevelData level = LevelManager.Instance.GetLevelDAtaByAuthorAndTitle(levelSaveData.author, levelSaveData.title);
            UILevelArtGallery uiLevelInstance = Instantiate(levelUIPrefab, parent);
            uiLevelInstance.Initialize(level);
            uiLevels.Add(uiLevelInstance);
            uiLevelInstance.OnSelectEvent += OnSelectBrush;
        }
    }

    private void OnDestroy()
    {
        foreach (UILevelArtGallery uiLevel in uiLevels)
        {
            uiLevel.OnSelectEvent -= OnSelectBrush;
        }
    }

    private void OnSelectBrush(LevelData levelData)
    {
        //TamponManager.Instance.SetTampon(levelData);
    }
}
