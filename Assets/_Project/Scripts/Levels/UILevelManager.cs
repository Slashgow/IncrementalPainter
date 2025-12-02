using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textCurrentLevelTitle;
    [SerializeField] private TextMeshProUGUI textCurrentLevelAuthor;
    [SerializeField] private TextMeshProUGUI textCurrentLevelArtMovement;

    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject uiLevelPrefab;
    [SerializeField] private Button playButton;

    private void Start()
    {
        playButton.onClick.AddListener(LoadGameScene);
        InitializeLevelCards();
        UpdateCurrentLevelDisplay(LevelManager.Instance.CurrentLevelData);
        SubscribeToLevelSelections();
    }
    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(LoadGameScene);

        UILevel[] uiLevels = contentParent.GetComponentsInChildren<UILevel>();

        foreach (UILevel uiLevel in uiLevels)
        {
            uiLevel.OnSelectEvent -= OnLevelSelected;
        }
    }

    private void LoadGameScene() => SceneLoader.Instance.LoadNextSceneAsync();

    private void InitializeLevelCards()
    {
        var levels = LevelManager.Instance.UnlockableSortedLevels;

        foreach (var level in levels)
        {
            GameObject uiLevelGOInstance = Instantiate(uiLevelPrefab, contentParent);
            UILevel uiLevelInstance = uiLevelGOInstance.GetComponent<UILevel>();

            if (uiLevelInstance != null)
            {
                uiLevelInstance.Initialize(level.LevelData);
            }
        }
    }

    private void SubscribeToLevelSelections()
    {
        UILevel[] uiLevels = contentParent.GetComponentsInChildren<UILevel>();

        foreach (UILevel uiLevel in uiLevels)
        {
            uiLevel.OnSelectEvent += OnLevelSelected;
        }
    }

    private void OnLevelSelected(LevelData level)
    {
        UpdateCurrentLevelDisplay(level);
    }

    private void UpdateCurrentLevelDisplay(LevelData currentLevel)
    {
        if (currentLevel == null) 
            return;

        textCurrentLevelTitle.text = currentLevel.LevelTitle;
        textCurrentLevelAuthor.text = $"{currentLevel.LevelAuthor} - {currentLevel.LevelDate}";
        textCurrentLevelArtMovement.text = currentLevel.ArtMovement.ToString();
    }
}